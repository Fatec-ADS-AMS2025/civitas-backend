using AutoMapper;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Servico responsavel pelas regras de negocio e operacoes de gerenciamento de Despesas.
    /// </summary>
    public class DespesaService : GenericService<Despesa, DespesaDTO>, IDespesaService
    {
        private readonly IDespesaRepository _despesaRepository;
        private readonly IUnidadeConsumidoraRepository _unidadeConsumidoraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITipoDespesaRepository _tipoDespesaRepository;
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instancia do servico de Despesas com as dependencias necessarias.
        /// </summary>
        public DespesaService(
            IDespesaRepository despesaRepository,
            IUnidadeConsumidoraRepository unidadeConsumidoraRepository,
            IUsuarioRepository usuarioRepository,
            ITipoDespesaRepository tipoDespesaRepository,
            IOrcamentoRepository orcamentoRepository,
            IInstituicaoRepository instituicaoRepository,
            IFornecedorRepository fornecedorRepository,
            AppDbContext context,
            IMapper mapper)
            : base(despesaRepository, mapper)
        {
            _despesaRepository = despesaRepository;
            _unidadeConsumidoraRepository = unidadeConsumidoraRepository;
            _usuarioRepository = usuarioRepository;
            _tipoDespesaRepository = tipoDespesaRepository;
            _orcamentoRepository = orcamentoRepository;
            _instituicaoRepository = instituicaoRepository;
            _fornecedorRepository = fornecedorRepository;
            _context = context;
            _mapper = mapper;
        }

        public override async Task Create(DespesaDTO entityDTO)
        {
            if (entityDTO is null)
            {
                throw new DespesaValidationException(["O corpo da requisicao e obrigatorio."]);
            }

            entityDTO.Status = Status.A_PAGAR;

            await ExecuteEmTransacaoAsync(async () =>
            {
                await ValidarCadastroAsync(entityDTO);
                entityDTO.Id = 0;

                var entity = _mapper.Map<Despesa>(entityDTO);
                await _despesaRepository.Add(entity);
                entityDTO.Id = entity.Id;
            });
        }

        public override async Task Update(DespesaDTO entityDTO, int id)
        {
            if (entityDTO is null)
            {
                throw new DespesaValidationException(["O corpo da requisicao e obrigatorio."]);
            }

            await ExecuteEmTransacaoAsync(async () =>
            {
                var existingEntity = await _despesaRepository.GetById(id);
                if (existingEntity is null)
                {
                    throw new KeyNotFoundException($"Despesa com id {id} nao encontrada.");
                }

                entityDTO.Id = id;
                await ValidarCadastroAsync(entityDTO, id);

                var entity = _mapper.Map<Despesa>(entityDTO);
                entity.Id = id;
                await _despesaRepository.Update(entity);
            });
        }

        public async Task<IEnumerable<DespesaDTO>> GetByNumeroDocumentoAsync(string numeroDocumento)
        {
            var entities = await _despesaRepository.GetByNumeroDocumentoAsync(Sanitize(numeroDocumento));
            return _mapper.Map<IEnumerable<DespesaDTO>>(entities);
        }

        public async Task<IEnumerable<DespesaDTO>> GetByCodigoAsync(string codigo)
        {
            var entities = await _despesaRepository.GetByCodigoAsync(Sanitize(codigo));
            return _mapper.Map<IEnumerable<DespesaDTO>>(entities);
        }

        public async Task<IEnumerable<DespesaDTO>> GetByUnidadeConsumidoraAsync(int idUnidadeConsumidora)
        {
            var entities = await _despesaRepository.GetByUnidadeConsumidoraAsync(idUnidadeConsumidora);
            return _mapper.Map<IEnumerable<DespesaDTO>>(entities);
        }

        public async Task<IEnumerable<DespesaDTO>> GetByUsuarioAsync(int idUsuario)
        {
            var entities = await _despesaRepository.GetByUsuarioAsync(idUsuario);
            return _mapper.Map<IEnumerable<DespesaDTO>>(entities);
        }

        public async Task<IEnumerable<DespesaDTO>> GetByStatusAsync(Status status)
        {
            var entities = await _despesaRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<DespesaDTO>>(entities);
        }

        public async Task<DespesaDTO> AlterarStatusAsync(int id, Status novoStatus)
        {
            if (!StatusValido(novoStatus))
            {
                throw new DespesaValidationException(["Status invalido. Valores permitidos: 1 (A_PAGAR), 2 (PAGA) ou 3 (ATRASADO)."]);
            }

            var despesa = await _despesaRepository.GetById(id);
            if (despesa is null)
            {
                throw new KeyNotFoundException($"Despesa com id {id} nao encontrada.");
            }

            var dto = _mapper.Map<DespesaDTO>(despesa);
            dto.Status = novoStatus;
            await Update(dto, id);

            return dto;
        }

        public async Task ValidarCadastroAsync(DespesaDTO entityDTO, int? id = null)
        {
            if (entityDTO is null)
            {
                throw new DespesaValidationException(["O corpo da requisicao e obrigatorio."]);
            }

            var errors = new List<string>();

            if (id.HasValue && id.Value <= 0)
            {
                errors.Add("Id da despesa invalido.");
            }

            NormalizarCampos(entityDTO);
            ValidarCamposBasicos(entityDTO, errors);
            await ValidarRelacionamentosAsync(entityDTO, errors);

            if (errors.Count > 0)
            {
                throw new DespesaValidationException(errors);
            }

            ArredondarCampos(entityDTO);
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

        private async Task ValidarRelacionamentosAsync(DespesaDTO despesaDTO, ICollection<string> errors)
        {
            if (despesaDTO.IdUsuario > 0)
            {
                var usuario = await _usuarioRepository.GetById(despesaDTO.IdUsuario);
                if (usuario is null)
                {
                    errors.Add("Usuario informado nao foi encontrado.");
                }
                else if (usuario.Situacao != Situacao.ATIVO)
                {
                    errors.Add("Usuario inativo nao pode cadastrar despesas.");
                }
            }

            if (despesaDTO.IdUnidadeConsumidora <= 0)
            {
                return;
            }

            var unidadeConsumidora = await _unidadeConsumidoraRepository.GetById(despesaDTO.IdUnidadeConsumidora);
            if (unidadeConsumidora is null)
            {
                errors.Add("UnidadeConsumidora informada nao foi encontrada.");
                return;
            }

            TipoDespesa? tipoDespesa = null;
            if (unidadeConsumidora.IdTipoDespesa <= 0)
            {
                errors.Add("UnidadeConsumidora nao possui TipoDespesa valido.");
            }
            else
            {
                tipoDespesa = await _tipoDespesaRepository.GetById(unidadeConsumidora.IdTipoDespesa);
                if (tipoDespesa is null)
                {
                    errors.Add("TipoDespesa vinculado a UnidadeConsumidora nao foi encontrado.");
                }
                else if (tipoDespesa.Situacao != Situacao.ATIVO)
                {
                    errors.Add("TipoDespesa vinculado a UnidadeConsumidora esta inativo.");
                }
            }

            ValidarValoresOpcionais(despesaDTO, tipoDespesa, errors);

            Orcamento? orcamento = null;
            if (unidadeConsumidora.IdOrcamento <= 0)
            {
                errors.Add("UnidadeConsumidora nao possui Orcamento valido.");
            }
            else
            {
                orcamento = await _orcamentoRepository.GetById(unidadeConsumidora.IdOrcamento);
                if (orcamento is null)
                {
                    errors.Add("Orcamento vinculado a UnidadeConsumidora nao foi encontrado.");
                }
            }

            Instituicao? instituicao = null;
            if (unidadeConsumidora.IdInstituicao <= 0)
            {
                errors.Add("UnidadeConsumidora nao possui Instituicao valida.");
            }
            else
            {
                instituicao = await _instituicaoRepository.GetById(unidadeConsumidora.IdInstituicao);
                if (instituicao is null)
                {
                    errors.Add("Instituicao vinculada a UnidadeConsumidora nao foi encontrada.");
                }
                else if (instituicao.Situacao != Situacao.ATIVO)
                {
                    errors.Add("Instituicao vinculada a UnidadeConsumidora esta inativa.");
                }
            }

            if (orcamento is not null && instituicao is not null && orcamento.IdInstituicao != instituicao.Id)
            {
                errors.Add("O orcamento vinculado a UnidadeConsumidora nao pertence a instituicao vinculada.");
            }

            if (unidadeConsumidora.IdFornecedor <= 0)
            {
                errors.Add("UnidadeConsumidora nao possui Fornecedor valido.");
            }
            else
            {
                var fornecedor = await _fornecedorRepository.GetById(unidadeConsumidora.IdFornecedor);
                if (fornecedor is null)
                {
                    errors.Add("Fornecedor vinculado a UnidadeConsumidora nao foi encontrado.");
                }
                else if (fornecedor.Situacao != Situacao.ATIVO)
                {
                    errors.Add("Fornecedor vinculado a UnidadeConsumidora esta inativo.");
                }
            }

            if (orcamento is not null && despesaDTO.ValorPrevisto >= 0)
            {
                await ValidarLimiteOrcamentarioAsync(despesaDTO, orcamento, errors);
            }
        }

        private async Task ValidarLimiteOrcamentarioAsync(
            DespesaDTO despesaDTO,
            Orcamento orcamento,
            ICollection<string> errors)
        {
            var novoValor = Math.Round(despesaDTO.ValorPrevisto, 2, MidpointRounding.AwayFromZero);
            var totalExistente = await _despesaRepository.SumValorPrevistoByOrcamentoAsync(
                orcamento.IdOrcamento,
                despesaDTO.Id > 0 ? despesaDTO.Id : null);

            var totalApos = totalExistente + novoValor;
            if (totalApos > orcamento.ValorOrcamento)
            {
                var excedente = totalApos - orcamento.ValorOrcamento;
                errors.Add(
                    $"A despesa ultrapassa o limite do orcamento em {excedente:F2}. " +
                    $"Orcamento: {orcamento.ValorOrcamento:F2}, Ja comprometido: {totalExistente:F2}, Nova despesa: {novoValor:F2}.");
            }
        }

        private static void NormalizarCampos(DespesaDTO despesaDTO)
        {
            despesaDTO.NumeroDocumento = Sanitize(despesaDTO.NumeroDocumento);
            despesaDTO.Codigo = Sanitize(despesaDTO.Codigo);
        }

        private static void ArredondarCampos(DespesaDTO despesaDTO)
        {
            despesaDTO.ValorPrevisto = Math.Round(despesaDTO.ValorPrevisto, 2, MidpointRounding.AwayFromZero);
            despesaDTO.ValorPago = Math.Round(despesaDTO.ValorPago, 2, MidpointRounding.AwayFromZero);
            despesaDTO.ConsumoPrevisto = Math.Round(despesaDTO.ConsumoPrevisto, 2, MidpointRounding.AwayFromZero);
            despesaDTO.ConsumoReal = Math.Round(despesaDTO.ConsumoReal, 2, MidpointRounding.AwayFromZero);
        }

        private static void ValidarCamposBasicos(DespesaDTO despesaDTO, ICollection<string> errors)
        {
            if (despesaDTO.Id < 0)
            {
                errors.Add("Id da despesa nao pode ser negativo.");
            }

            ValidarObrigatorio(despesaDTO.NumeroDocumento, nameof(despesaDTO.NumeroDocumento), errors);
            ValidarTamanhoMaximo(despesaDTO.NumeroDocumento, 100, "NumeroDocumento", errors);

            ValidarObrigatorio(despesaDTO.Codigo, nameof(despesaDTO.Codigo), errors);
            ValidarTamanhoMaximo(despesaDTO.Codigo, 100, "Codigo", errors);

            if (despesaDTO.DataEmissao == default)
            {
                errors.Add("Campo obrigatorio nao preenchido: DataEmissao.");
            }

            if (despesaDTO.DataVencimento == default)
            {
                errors.Add("Campo obrigatorio nao preenchido: DataVencimento.");
            }

            if (despesaDTO.DataEmissao != default
                && despesaDTO.DataVencimento != default
                && despesaDTO.DataVencimento < despesaDTO.DataEmissao)
            {
                errors.Add("DataVencimento nao pode ser anterior a DataEmissao.");
            }

            ValidarValorNaoNegativo(despesaDTO.ValorPrevisto, nameof(despesaDTO.ValorPrevisto), errors);
            ValidarValorNaoNegativo(despesaDTO.ValorPago, nameof(despesaDTO.ValorPago), errors);
            ValidarValorNaoNegativo(despesaDTO.ConsumoPrevisto, nameof(despesaDTO.ConsumoPrevisto), errors);
            ValidarValorNaoNegativo(despesaDTO.ConsumoReal, nameof(despesaDTO.ConsumoReal), errors);

            if (!StatusValido(despesaDTO.Status))
            {
                errors.Add("Status invalido. Valores permitidos: 1 (A_PAGAR), 2 (PAGA) ou 3 (ATRASADO).");
            }

            ValidarIdPositivo(despesaDTO.IdUsuario, "IdUsuario", errors);
            ValidarIdPositivo(despesaDTO.IdUnidadeConsumidora, "IdUnidadeConsumidora", errors);
        }

        private static void ValidarObrigatorio(string valor, string campo, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                errors.Add($"Campo obrigatorio nao preenchido: {campo}.");
            }
        }

        private static void ValidarTamanhoMaximo(string valor, int maximo, string campo, ICollection<string> errors)
        {
            if (!string.IsNullOrEmpty(valor) && valor.Length > maximo)
            {
                errors.Add($"Campo {campo} deve conter no maximo {maximo} caracteres.");
            }
        }

        private static void ValidarValorNaoNegativo(decimal valor, string campo, ICollection<string> errors)
        {
            if (valor < 0)
            {
                errors.Add($"{campo} nao pode ser menor que zero.");
            }
        }

        private static void ValidarIdPositivo(int valor, string campo, ICollection<string> errors)
        {
            if (valor <= 0)
            {
                errors.Add($"{campo} deve ser maior que zero.");
            }
        }

        private static bool StatusValido(Status status)
        {
            return status is Status.A_PAGAR or Status.PAGA or Status.ATRASADO;
        }

        private static string Sanitize(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }

        internal static void ValidarValoresOpcionais(
            DespesaDTO dto,
            TipoDespesa? tipoDespesa,
            ICollection<string> errors)
        {
            if (dto.ValoresOpcionais is null || dto.ValoresOpcionais.Count == 0)
            {
                return;
            }

            if (tipoDespesa is null)
            {
                errors.Add("Não é possível validar ValoresOpcionais sem TipoDespesa associado.");
                return;
            }

            IReadOnlyList<string> declarados;
            try
            {
                declarados = CamposOpcionaisJsonHelper.ParseCamposDeclarados(tipoDespesa.CamposOpcionais);
            }
            catch (Exception ex) when (ex is ArgumentException or System.Text.Json.JsonException)
            {
                errors.Add($"CamposOpcionais do TipoDespesa estão corrompidos: {ex.Message}");
                return;
            }

            if (declarados.Count == 0)
            {
                errors.Add("O TipoDespesa associado não declara nenhum campo opcional.");
                return;
            }

            var desconhecidas = CamposOpcionaisJsonHelper.EncontrarChavesDesconhecidas(
                dto.ValoresOpcionais.Keys,
                declarados);
            if (desconhecidas.Count > 0)
            {
                errors.Add(
                    $"ValoresOpcionais contém chaves não declaradas em TipoDespesa: " +
                    string.Join(", ", desconhecidas));
            }

            foreach (var key in dto.ValoresOpcionais.Keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    errors.Add("ValoresOpcionais não pode conter chaves vazias.");
                    break;
                }
            }
        }
    }
}
