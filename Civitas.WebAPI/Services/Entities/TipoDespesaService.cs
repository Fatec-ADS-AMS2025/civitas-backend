using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão das Categorias de Despesa (Configurações de Lançamento).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar os tipos de contas permitidos no sistema (Água, Luz, Telefone).
    /// - Centralizar a configuração de regras de preenchimento (se exige UC, qual unidade de medida usa).
    /// 
    /// Dependências:
    /// - <see cref="ITipoDespesaRepository"/>: Acesso a dados.
    /// - <see cref="IMapper"/>: Mapeamento de objetos.
    /// </remarks>
    public class TipoDespesaService : GenericService<TipoDespesa, TipoDespesaDTO>, ITipoDespesaService
    {
        private static readonly CultureInfo PtBrCulture = new("pt-BR");

        private readonly ITipoDespesaRepository _tipoDespesaRepository;
        private readonly IUnidadeMedidaRepository _unidadeMedidaRepository;
        private readonly ITipoCodigoRepository _tipoCodigoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Tipos de Despesa.
        /// </summary>
        /// <param name="tipoDespesa">Repositório concreto de tipos de despesa.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public TipoDespesaService(
            ITipoDespesaRepository tipoDespesa,
            IUnidadeMedidaRepository unidadeMedidaRepository,
            ITipoCodigoRepository tipoCodigoRepository,
            IMapper mapper)
            : base(tipoDespesa, mapper)
        {
            _tipoDespesaRepository = tipoDespesa;
            _unidadeMedidaRepository = unidadeMedidaRepository;
            _tipoCodigoRepository = tipoCodigoRepository;
            _mapper = mapper;
        }

        public override async Task Create(TipoDespesaDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors);

            if (errors.Count > 0)
            {
                throw new TipoDespesaValidationException(errors);
            }

            var entity = _mapper.Map<TipoDespesa>(entityDTO);
            await _tipoDespesaRepository.Add(entity);

            entityDTO.Id = entity.Id;
        }

        public override async Task Update(TipoDespesaDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existingTipoDespesa = await _tipoDespesaRepository.GetById(id);
            if (existingTipoDespesa is null)
            {
                throw new KeyNotFoundException($"Tipo de despesa com id {id} não encontrado.");
            }

            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors, id, existingTipoDespesa);

            if (errors.Count > 0)
            {
                throw new TipoDespesaValidationException(errors);
            }

            var entity = _mapper.Map<TipoDespesa>(entityDTO);
            entity.Id = id;
            entity.Excluido = existingTipoDespesa.Excluido;
            entity.DataExclusao = existingTipoDespesa.DataExclusao;

            await _tipoDespesaRepository.Update(entity);

            entityDTO.Id = id;
        }

        /// <summary>
        /// Verifica a validade da Unidade de Medida vinculada a um tipo de despesa.
        /// </summary>
        /// <param name="idTipoDespesa">O identificador do Tipo de Despesa a ser verificado.</param>
        /// <returns>
        /// Retorna <c>true</c> se a unidade de medida vinculada estiver ATIVA.
        /// Retorna <c>false</c> se a unidade de medida estiver INATIVA ou inexistente.
        /// </returns>
        /// <remarks>
        /// Regra de Negócio:
        /// Antes de permitir o cadastro de uma nova despesa (ex: Energia), o sistema deve validar se a unidade usada (ex: kWh) ainda é válida.
        /// Isso evita inconsistências em relatórios de consumo caso uma unidade seja depreciada.
        /// </remarks>
        public async Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa)
        {
            return await _tipoDespesaRepository.ExisteUnidadesDeMedidaAtivas(idTipoDespesa);
        }

        public async Task<bool> HasDespesasVinculadas(int idTipoDespesa)
        {
            return await _tipoDespesaRepository.HasDespesasVinculadas(idTipoDespesa);
        }

        private static void ValidateDtoInstance(TipoDespesaDTO? tipoDespesaDTO)
        {
            if (tipoDespesaDTO is null)
            {
                throw new TipoDespesaValidationException(["O corpo da requisição é obrigatório."]);
            }
        }

        private static void Normalize(TipoDespesaDTO tipoDespesaDTO)
        {
            var descricao = tipoDespesaDTO.Descricao?.Trim() ?? string.Empty;
            descricao = Regex.Replace(descricao, "\\s+", " ");

            if (!string.IsNullOrEmpty(descricao))
            {
                descricao = PtBrCulture.TextInfo.ToTitleCase(descricao.ToLower(PtBrCulture));
            }

            tipoDespesaDTO.Descricao = descricao;
        }

        private static List<string> ValidateCommonRules(TipoDespesaDTO tipoDespesaDTO)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(tipoDespesaDTO.Descricao))
            {
                errors.Add("O campo Descrição é obrigatório.");
            }
            else
            {
                if (tipoDespesaDTO.Descricao.Length < 3)
                {
                    errors.Add("O campo Descrição deve ter no mínimo 3 caracteres.");
                }

                if (tipoDespesaDTO.Descricao.Length > 150)
                {
                    errors.Add("O campo Descrição deve ter no máximo 150 caracteres.");
                }
            }

            if (tipoDespesaDTO.IdUnidadeMedida <= 0)
            {
                errors.Add("O campo IdUnidadeMedida é obrigatório.");
            }

            if (tipoDespesaDTO.IdTipoCodigo <= 0)
            {
                errors.Add("O campo IdTipoCodigo é obrigatório.");
            }

            if (tipoDespesaDTO.SolicitaUc is not (SolicitaUc.Sim or SolicitaUc.Não))
            {
                errors.Add("SolicitaUc inválido. Valores permitidos: 1 (Sim) ou 2 (Não).");
            }

            if (tipoDespesaDTO.Situacao is not (Situacao.ATIVO or Situacao.INATIVO))
            {
                errors.Add("Situação inválida. Valores permitidos: 1 (Ativo) ou 2 (Inativo).");
            }

            ValidarCamposOpcionais(tipoDespesaDTO, errors);

            return errors;
        }

        private async Task ValidateBusinessRules(
            TipoDespesaDTO tipoDespesaDTO,
            ICollection<string> errors,
            int? id = null,
            TipoDespesa? existingTipoDespesa = null)
        {
            if (tipoDespesaDTO.IdUnidadeMedida > 0)
            {
                var unidadeMedida = await _unidadeMedidaRepository.GetById(tipoDespesaDTO.IdUnidadeMedida);
                if (unidadeMedida is null)
                {
                    errors.Add("A unidade de medida informada não existe.");
                }
                else if (unidadeMedida.Situacao != Situacao.ATIVO)
                {
                    errors.Add("A unidade de medida informada está inativa e não pode ser utilizada.");
                }
            }

            if (tipoDespesaDTO.IdTipoCodigo > 0)
            {
                var tipoCodigo = await _tipoCodigoRepository.GetById(tipoDespesaDTO.IdTipoCodigo);
                if (tipoCodigo is null)
                {
                    errors.Add("O tipo de código informado não existe.");
                }
            }

            if (!string.IsNullOrWhiteSpace(tipoDespesaDTO.Descricao))
            {
                var descricaoNormalizada = NormalizeForComparison(tipoDespesaDTO.Descricao);
                if (await _tipoDespesaRepository.ExistsByDescricaoNormalized(descricaoNormalizada, id))
                {
                    errors.Add("Já existe um tipo de despesa cadastrado com esta descrição.");
                }
            }

            if (id.HasValue && existingTipoDespesa is not null
                && existingTipoDespesa.Situacao == Situacao.ATIVO
                && tipoDespesaDTO.Situacao == Situacao.INATIVO
                && await _tipoDespesaRepository.HasDespesasVinculadas(id.Value))
            {
                errors.Add("Não é permitido inativar tipo de despesa com despesas vinculadas.");
            }
        }

        private static string NormalizeForComparison(string descricao)
        {
            return Regex.Replace(descricao.Trim(), "\\s+", " ").ToUpperInvariant();
        }

        private static void ValidarCamposOpcionais(TipoDespesaDTO dto, ICollection<string> errors)
        {
            if (dto.CamposOpcionais is null || dto.CamposOpcionais.Count == 0)
            {
                return;
            }

            try
            {
                // Reutiliza a regra de unicidade/limite/comprimento.
                _ = CamposOpcionaisJsonHelper.SerializeCamposDeclarados(dto.CamposOpcionais);
            }
            catch (ArgumentException ex)
            {
                errors.Add($"CamposOpcionais inválidos: {ex.Message}");
            }
        }
    }
}
