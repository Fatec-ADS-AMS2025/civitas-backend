using AutoMapper;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Servico responsavel pela gestao do planejamento orcamentario e tetos de gastos das instituicoes.
    /// </summary>
    public class OrcamentoService : GenericService<Orcamento, OrcamentoDTO>, IOrcamentoService
    {
        private const int AnoMinimo = 2000;
        private const int AnosAdiantePermitidos = 10;

        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrcamentoService(
            IOrcamentoRepository orcamentoRepository,
            IInstituicaoRepository instituicaoRepository,
            AppDbContext context,
            IMapper mapper)
            : base(orcamentoRepository, mapper)
        {
            _orcamentoRepository = orcamentoRepository;
            _instituicaoRepository = instituicaoRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ExisteDespesaVinculada(int idOrcamento)
        {
            return await _orcamentoRepository.ExisteDespesaVinculada(idOrcamento);
        }

        public override async Task Create(OrcamentoDTO entityDTO)
        {
            await ExecuteEmTransacaoAsync(async () =>
            {
                await ValidarOrcamentoAsync(entityDTO, existente: null);

                entityDTO.ValorOrcamento = Math.Round(entityDTO.ValorOrcamento, 2, MidpointRounding.AwayFromZero);
                entityDTO.IdOrcamento = 0;

                var entity = _mapper.Map<Orcamento>(entityDTO);
                await _orcamentoRepository.Add(entity);
                entityDTO.IdOrcamento = entity.IdOrcamento;
            });
        }

        public override async Task Update(OrcamentoDTO entityDTO, int id)
        {
            await ExecuteEmTransacaoAsync(async () =>
            {
                var existente = await _orcamentoRepository.GetById(id);
                if (existente is null)
                {
                    throw new KeyNotFoundException($"Orcamento com id {id} nao encontrado.");
                }

                await ValidarOrcamentoAsync(entityDTO, existente);

                entityDTO.ValorOrcamento = Math.Round(entityDTO.ValorOrcamento, 2, MidpointRounding.AwayFromZero);
                entityDTO.IdOrcamento = id;

                var entity = _mapper.Map<Orcamento>(entityDTO);
                await _orcamentoRepository.Update(entity);
            });
        }

        public async Task RemoverAsync(int id)
        {
            await ExecuteEmTransacaoAsync(async () =>
            {
                var existente = await _orcamentoRepository.GetById(id);
                if (existente is null)
                {
                    throw new KeyNotFoundException($"Orcamento com id {id} nao encontrado.");
                }

                if (await _orcamentoRepository.ExisteDespesaVinculada(id))
                {
                    throw new OrcamentoValidationException(
                        ["Nao e possivel excluir um orcamento que possui despesas vinculadas."]);
                }

                await _orcamentoRepository.Remove(existente);
            });
        }

        private async Task ExecuteEmTransacaoAsync(Func<Task> acao)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                await acao();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ValidarOrcamentoAsync(OrcamentoDTO entityDTO, Orcamento? existente)
        {
            if (entityDTO is null)
            {
                throw new OrcamentoValidationException(["O corpo da requisicao e obrigatorio."]);
            }

            var errors = new List<string>();

            ValidarAno(entityDTO.AnoOrcamento, errors);
            ValidarValor(entityDTO.ValorOrcamento, errors);

            if (entityDTO.IdInstituicao <= 0)
            {
                errors.Add("O campo IdInstituicao e obrigatorio e deve ser maior que zero.");
            }
            else
            {
                var instituicao = await _instituicaoRepository.GetById(entityDTO.IdInstituicao);
                if (instituicao is null)
                {
                    errors.Add("A instituicao informada nao foi encontrada.");
                }
            }

            if (existente is not null && entityDTO.ValorOrcamento > 0)
            {
                var totalComprometido = await _orcamentoRepository.SumConsumoByOrcamentoAsync(existente.IdOrcamento);
                var novoValor = Math.Round(entityDTO.ValorOrcamento, 2, MidpointRounding.AwayFromZero);
                if (novoValor < totalComprometido)
                {
                    errors.Add(
                        $"O novo valor do orcamento ({novoValor:F2}) nao pode ser inferior ao total ja comprometido em despesas ({totalComprometido:F2}).");
                }
            }

            if (errors.Count > 0)
            {
                throw new OrcamentoValidationException(errors);
            }
        }

        private static void ValidarAno(int ano, ICollection<string> errors)
        {
            if (ano <= 0)
            {
                errors.Add("O campo AnoOrcamento e obrigatorio e deve ser maior que zero.");
                return;
            }

            if (ano < 1000 || ano > 9999)
            {
                errors.Add("O AnoOrcamento deve conter exatamente 4 digitos.");
                return;
            }

            if (ano < AnoMinimo)
            {
                errors.Add($"O AnoOrcamento deve ser maior ou igual a {AnoMinimo}.");
            }

            var anoAtual = DateTime.UtcNow.Year;
            if (ano > anoAtual + AnosAdiantePermitidos)
            {
                errors.Add($"O AnoOrcamento nao pode ser superior a {anoAtual + AnosAdiantePermitidos}.");
            }
        }

        private static void ValidarValor(decimal valor, ICollection<string> errors)
        {
            if (valor <= 0)
            {
                errors.Add("O campo ValorOrcamento e obrigatorio e deve ser maior que zero.");
            }
        }
    }
}
