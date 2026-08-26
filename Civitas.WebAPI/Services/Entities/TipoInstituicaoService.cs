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
    /// Serviço responsável pela gestão das Categorias de Instituição.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar a taxonomia/classificação das instituições (ex: Escola, Hospital, ONG).
    /// - Garantir a integridade dos dados impedindo a exclusão de categorias em uso.
    /// 
    /// Dependências:
    /// - <see cref="ITipoInstituicaoRepository"/>: Persistência de dados.
    /// - <see cref="IMapper"/>: Transformação de objetos.
    /// </remarks>
    public class TipoInstituicaoService : GenericService<TipoInstituicao, TipoInstituicaoDTO>, ITipoInstituicaoService
    {
        private static readonly CultureInfo PtBrCulture = new("pt-BR");

        private readonly ITipoInstituicaoRepository _tipoInstituicaoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Tipos de Instituição.
        /// </summary>
        /// <param name="tipoInstituicao">Repositório concreto de tipos de instituição.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public TipoInstituicaoService(ITipoInstituicaoRepository tipoInstituicao, IMapper mapper)
            : base(tipoInstituicao, mapper)
        {
            _tipoInstituicaoRepository = tipoInstituicao;
            _mapper = mapper;
        }

        public override async Task Create(TipoInstituicaoDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors);

            if (errors.Count > 0)
            {
                throw new TipoInstituicaoValidationException(errors);
            }

            var entity = _mapper.Map<TipoInstituicao>(entityDTO);
            await _tipoInstituicaoRepository.Add(entity);

            entityDTO.Id = entity.Id;
        }

        public override async Task Update(TipoInstituicaoDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existingTipoInstituicao = await _tipoInstituicaoRepository.GetById(id);
            if (existingTipoInstituicao is null)
            {
                throw new KeyNotFoundException($"Tipo de instituição com id {id} não encontrado.");
            }

            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors, id, existingTipoInstituicao);

            if (errors.Count > 0)
            {
                throw new TipoInstituicaoValidationException(errors);
            }

            var entity = _mapper.Map<TipoInstituicao>(entityDTO);
            entity.Id = id;
            entity.Excluido = existingTipoInstituicao.Excluido;
            entity.DataExclusao = existingTipoInstituicao.DataExclusao;

            await _tipoInstituicaoRepository.Update(entity);

            entityDTO.Id = id;
        }

        /// <summary>
        /// Verifica se existem instituições ativas vinculadas a uma determinada categoria.
        /// </summary>
        /// <param name="idTipoInstituicao">O identificador da categoria (Tipo) a ser verificada.</param>
        /// <returns>
        /// Retorna <c>true</c> se houver instituições vinculadas. 
        /// Retorna <c>false</c> se a categoria estiver livre para ser removida ou desativada.
        /// </returns>
        /// <remarks>
        /// Regra de Negócio (Safe Delete):
        /// Este método atua como uma trava de segurança. O sistema não deve permitir a exclusão de um Tipo de Instituição
        /// enquanto houver registros dependentes dele, para evitar "órfãos" no banco de dados.
        /// </remarks>
        public async Task<bool> ExisteInstituicoesAtivas(int idTipoInstituicao)
        {
            return await _tipoInstituicaoRepository.ExisteInstituicoesAtivasAsync(idTipoInstituicao);
        }

        private static void ValidateDtoInstance(TipoInstituicaoDTO? tipoInstituicaoDTO)
        {
            if (tipoInstituicaoDTO is null)
            {
                throw new TipoInstituicaoValidationException(["O corpo da requisição é obrigatório."]);
            }
        }

        private static void Normalize(TipoInstituicaoDTO tipoInstituicaoDTO)
        {
            var descricao = tipoInstituicaoDTO.Descricao?.Trim() ?? string.Empty;
            descricao = Regex.Replace(descricao, "\\s+", " ");

            if (!string.IsNullOrEmpty(descricao))
            {
                descricao = PtBrCulture.TextInfo.ToTitleCase(descricao.ToLower(PtBrCulture));
            }

            tipoInstituicaoDTO.Descricao = descricao;
        }

        private static List<string> ValidateCommonRules(TipoInstituicaoDTO tipoInstituicaoDTO)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(tipoInstituicaoDTO.Descricao))
            {
                errors.Add("O campo Descrição é obrigatório.");
            }
            else
            {
                if (tipoInstituicaoDTO.Descricao.Length < 3)
                {
                    errors.Add("O campo Descrição deve ter no mínimo 3 caracteres.");
                }

                if (tipoInstituicaoDTO.Descricao.Length > 150)
                {
                    errors.Add("O campo Descrição deve ter no máximo 150 caracteres.");
                }
            }

            if (tipoInstituicaoDTO.Situacao is not (Situacao.ATIVO or Situacao.INATIVO))
            {
                errors.Add("Situação inválida. Valores permitidos: 1 (Ativo) ou 2 (Inativo).");
            }

            return errors;
        }

        private async Task ValidateBusinessRules(
            TipoInstituicaoDTO tipoInstituicaoDTO,
            ICollection<string> errors,
            int? id = null,
            TipoInstituicao? existingTipoInstituicao = null)
        {
            if (!string.IsNullOrWhiteSpace(tipoInstituicaoDTO.Descricao))
            {
                var descricaoNormalizada = NormalizeForComparison(tipoInstituicaoDTO.Descricao);
                if (await _tipoInstituicaoRepository.ExistsByDescricaoNormalized(descricaoNormalizada, id))
                {
                    errors.Add("Já existe um tipo de instituição cadastrado com esta descrição.");
                }
            }

            if (id.HasValue && existingTipoInstituicao is not null
                && existingTipoInstituicao.Situacao == Situacao.ATIVO
                && tipoInstituicaoDTO.Situacao == Situacao.INATIVO
                && await _tipoInstituicaoRepository.ExisteInstituicoesAtivasAsync(id.Value))
            {
                errors.Add("Não é permitido inativar tipo de instituição com instituições ativas vinculadas.");
            }
        }

        private static string NormalizeForComparison(string descricao)
        {
            return Regex.Replace(descricao.Trim(), "\\s+", " ").ToUpperInvariant();
        }
    }
}
