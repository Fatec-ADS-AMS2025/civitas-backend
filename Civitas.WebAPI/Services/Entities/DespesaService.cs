using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using System.Globalization;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Servico responsavel pelas regras de negocio e operacoes de gerenciamento de Despesas.
    /// </summary>
    public class DespesaService : GenericService<Despesa, DespesaDTO>, IDespesaService
    {
        private static readonly CultureInfo PtBrCulture = new("pt-BR");
        private static readonly string[] SupportedDateFormats = ["yyyy-MM-dd", "dd/MM/yyyy", "yyyy/MM/dd", "dd-MM-yyyy"];

        private readonly IDespesaRepository _despesaRepository;
        private readonly ITipoDespesaRepository _tipoDespesaRepository;
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        /// <summary>
        /// Inicializa uma nova instancia do servico de Despesas com as dependencias necessarias.
        /// </summary>
        public DespesaService(
            IDespesaRepository despesaRepository,
            ITipoDespesaRepository tipoDespesaRepository,
            IOrcamentoRepository orcamentoRepository,
            IInstituicaoRepository instituicaoRepository,
            IFornecedorRepository fornecedorRepository,
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
            : base(despesaRepository, mapper)
        {
            _despesaRepository = despesaRepository;
            _tipoDespesaRepository = tipoDespesaRepository;
            _orcamentoRepository = orcamentoRepository;
            _instituicaoRepository = instituicaoRepository;
            _fornecedorRepository = fornecedorRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task ValidarCadastroAsync(DespesaDTO entityDTO, int? id = null)
        {
            if (entityDTO is null)
            {
                throw new ArgumentException("Dados da despesa sao obrigatorios.");
            }

            if (id.HasValue && id.Value <= 0)
            {
                throw new ArgumentException("Id da despesa invalido.");
            }

            if (id.HasValue)
            {
                var existingEntity = await _despesaRepository.GetById(id.Value);
                if (existingEntity is null)
                {
                    throw new KeyNotFoundException($"Entidade com id {id.Value} nao encontrada.");
                }
            }

            entityDTO.Id = id ?? 0;

            NormalizarCampos(entityDTO);
            ValidarCamposBasicos(entityDTO);
            await ValidarRelacionamentosAsync(entityDTO);
            await ValidarUnicidadeAsync(entityDTO, id);
        }

        private async Task ValidarRelacionamentosAsync(DespesaDTO despesaDTO)
        {
            var tipoDespesa = await _tipoDespesaRepository.GetById(despesaDTO.IdTipoDespesa);
            if (tipoDespesa is null)
            {
                throw new ArgumentException("TipoDespesa informado nao foi encontrado.");
            }

            if (tipoDespesa.SolicitaUc == SolicitaUc.Sim && string.IsNullOrWhiteSpace(despesaDTO.UC))
            {
                throw new ArgumentException("UC e obrigatoria para o tipo de despesa informado.");
            }

            var orcamento = await _orcamentoRepository.GetById(despesaDTO.IdOrcamento);
            if (orcamento is null)
            {
                throw new ArgumentException("Orcamento informado nao foi encontrado.");
            }

            var instituicao = await _instituicaoRepository.GetById(despesaDTO.IdInstituicao);
            if (instituicao is null)
            {
                throw new ArgumentException("Instituicao informada nao foi encontrada.");
            }

            if (orcamento.IdInstituicao != despesaDTO.IdInstituicao)
            {
                throw new ArgumentException("O orcamento informado nao pertence a instituicao selecionada.");
            }

            if (orcamento.IdTipoDespesa > 0 && orcamento.IdTipoDespesa != despesaDTO.IdTipoDespesa)
            {
                throw new ArgumentException("O orcamento informado nao esta vinculado ao tipo de despesa selecionado.");
            }

            var fornecedor = await _fornecedorRepository.GetById(despesaDTO.IdFornecedor);
            if (fornecedor is null)
            {
                throw new ArgumentException("Fornecedor informado nao foi encontrado.");
            }

            if (fornecedor.Situacao != Situacao.ATIVO)
            {
                throw new ArgumentException("Fornecedor inativo nao pode ser utilizado em novas despesas.");
            }

            var usuario = await _usuarioRepository.GetById(despesaDTO.IdUsuario);
            if (usuario is null)
            {
                throw new ArgumentException("Usuario informado nao foi encontrado.");
            }
        }

        private async Task ValidarUnicidadeAsync(DespesaDTO despesaDTO, int? ignoreId)
        {
            var documentoDuplicado = await _despesaRepository.ExistsByNumeroDocumentoAndFornecedorAsync(
                despesaDTO.NumeroDocumento,
                despesaDTO.IdFornecedor,
                ignoreId);

            if (documentoDuplicado)
            {
                throw new ArgumentException("Ja existe uma despesa cadastrada para este fornecedor com o mesmo numero de documento.");
            }
        }

        private static void NormalizarCampos(DespesaDTO despesaDTO)
        {
            despesaDTO.NumeroDocumento = Sanitize(despesaDTO.NumeroDocumento);
            despesaDTO.UC = Sanitize(despesaDTO.UC);
            despesaDTO.DataEmissao = Sanitize(despesaDTO.DataEmissao);
        }

        private static void ValidarCamposBasicos(DespesaDTO despesaDTO)
        {
            if (despesaDTO.Id < 0)
            {
                throw new ArgumentException("Id da despesa nao pode ser negativo.");
            }

            ValidarObrigatorio(despesaDTO.NumeroDocumento, nameof(despesaDTO.NumeroDocumento));
            ValidarTamanhoMaximo(despesaDTO.NumeroDocumento, 100, "NumeroDocumento");
            ValidarSomenteNumeros(despesaDTO.NumeroDocumento, "NumeroDocumento");
            ValidarTamanhoMaximo(despesaDTO.UC, 100, "UC");
            ValidarObrigatorio(despesaDTO.DataEmissao, nameof(despesaDTO.DataEmissao));

            var dataEmissao = ParseDataEmissao(despesaDTO.DataEmissao);
            if (dataEmissao > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("DataEmicao nao pode ser futura.");
            }

            if (despesaDTO.DataVencimento == default)
            {
                throw new ArgumentException("Campo obrigatorio nao preenchido: DataVencimento.");
            }

            if (despesaDTO.DataVencimento < dataEmissao)
            {
                throw new ArgumentException("DataVencimento nao pode ser anterior a DataEmicao.");
            }

            if (double.IsNaN(despesaDTO.ConsumoPrevisto) || double.IsInfinity(despesaDTO.ConsumoPrevisto))
            {
                throw new ArgumentException("ConsumoPrevisto invalido.");
            }

            if (despesaDTO.ConsumoPrevisto < 0)
            {
                throw new ArgumentException("ConsumoPrevisto nao pode ser negativo.");
            }

            var valorStatus = (int)despesaDTO.Status;
            if (valorStatus is not ((int)Status.A_PAGAR) and not ((int)Status.PAGA) and not ((int)Status.ATRASADO))
            {
                throw new ArgumentException("Status invalido. Valores permitidos: 1 (A_PAGAR), 2 (PAGA) ou 3 (ATRASADO).");
            }

            ValidarIdPositivo(despesaDTO.IdTipoDespesa, "IdTipoDespesa");
            ValidarIdPositivo(despesaDTO.IdOrcamento, "IdOrcamento");
            ValidarIdPositivo(despesaDTO.IdInstituicao, "IdInstituicao");
            ValidarIdPositivo(despesaDTO.IdFornecedor, "IdFornecedor");
            ValidarIdPositivo(despesaDTO.IdUsuario, "IdUsuario");

            despesaDTO.DataEmissao = dataEmissao.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        private static void ValidarObrigatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException($"Campo obrigatorio nao preenchido: {campo}.");
            }
        }

        private static void ValidarTamanhoMaximo(string valor, int maximo, string campo)
        {
            if (valor.Length > maximo)
            {
                throw new ArgumentException($"Campo {campo} deve conter no maximo {maximo} caracteres.");
            }
        }

        private static void ValidarIdPositivo(int valor, string campo)
        {
            if (valor <= 0)
            {
                throw new ArgumentException($"{campo} deve ser maior que zero.");
            }
        }

        private static void ValidarSomenteNumeros(string valor, string campo)
        {
            if (!valor.All(char.IsDigit))
            {
                throw new ArgumentException($"{campo} deve conter apenas numeros.");
            }
        }

        private static DateOnly ParseDataEmissao(string dataEmissao)
        {
            if (DateOnly.TryParseExact(
                    dataEmissao,
                    SupportedDateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var data))
            {
                return data;
            }

            if (DateOnly.TryParse(dataEmissao, PtBrCulture, DateTimeStyles.None, out data))
            {
                return data;
            }

            throw new ArgumentException("DataEmicao invalida.");
        }

        private static string Sanitize(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }
    }
}
