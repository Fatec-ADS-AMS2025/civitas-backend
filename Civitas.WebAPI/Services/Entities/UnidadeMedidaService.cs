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
    /// <summary>Centraliza o cadastro, a normalização e as regras de negócio das unidades de medida.</summary>
    public class UnidadeMedidaService : GenericService<UnidadeMedida, UnidadeMedidaDTO>, IUnidadeMedidaService
    {
        private static readonly CultureInfo PtBrCulture = new("pt-BR");
        private readonly IUnidadeMedidaRepository _unidadeMedida;
        private readonly IMapper _mapper;

        public UnidadeMedidaService(IUnidadeMedidaRepository unidadeMedida, IMapper mapper)
            : base(unidadeMedida, mapper)
        {
            _unidadeMedida = unidadeMedida;
            _mapper = mapper;
        }

        public override async Task Create(UnidadeMedidaDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors);
            ThrowIfInvalid(errors);

            var entity = _mapper.Map<UnidadeMedida>(entityDTO);
            await _unidadeMedida.Add(entity);
            entityDTO.Id = entity.Id;
        }

        public override async Task Update(UnidadeMedidaDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existingUnidadeMedida = await _unidadeMedida.GetById(id);
            if (existingUnidadeMedida is null)
            {
                throw new KeyNotFoundException($"Unidade de medida com id {id} não encontrada.");
            }

            Normalize(entityDTO);

            var errors = ValidateCommonRules(entityDTO);
            await ValidateBusinessRules(entityDTO, errors, id, existingUnidadeMedida);
            ThrowIfInvalid(errors);

            var entity = _mapper.Map<UnidadeMedida>(entityDTO);
            entity.Id = id;
            entity.Excluido = existingUnidadeMedida.Excluido;
            entity.DataExclusao = existingUnidadeMedida.DataExclusao;
            await _unidadeMedida.Update(entity);
            entityDTO.Id = id;
        }

        private static void ValidateDtoInstance(UnidadeMedidaDTO? dto)
        {
            if (dto is null)
            {
                throw new UnidadeMedidaValidationException(["O corpo da requisição é obrigatório."]);
            }
        }

        private static void Normalize(UnidadeMedidaDTO dto)
        {
            var descricao = NormalizeSpaces(dto.Descricao);
            dto.Descricao = string.IsNullOrEmpty(descricao)
                ? descricao
                : PtBrCulture.TextInfo.ToTitleCase(descricao.ToLower(PtBrCulture));
            dto.Abreviatura = NormalizeSpaces(dto.Abreviatura).ToUpperInvariant();
        }

        private static List<string> ValidateCommonRules(UnidadeMedidaDTO dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                errors.Add("O campo Descrição é obrigatório.");
            }
            else
            {
                if (dto.Descricao.Length < 3) errors.Add("O campo Descrição deve ter no mínimo 3 caracteres.");
                if (dto.Descricao.Length > 150) errors.Add("O campo Descrição deve ter no máximo 150 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(dto.Abreviatura))
            {
                errors.Add("O campo Abreviatura é obrigatório.");
            }
            else if (dto.Abreviatura.Length > 45)
            {
                errors.Add("O campo Abreviatura deve ter no máximo 45 caracteres.");
            }

            if (!Enum.IsDefined(dto.Situacao) || dto.Situacao is not (Situacao.ATIVO or Situacao.INATIVO))
            {
                errors.Add("Situação inválida. Valores permitidos: 1 (Ativo) ou 2 (Inativo).");
            }

            return errors;
        }

        private async Task ValidateBusinessRules(
            UnidadeMedidaDTO dto,
            ICollection<string> errors,
            int? id = null,
            UnidadeMedida? existing = null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Descricao) &&
                await _unidadeMedida.ExistsByDescricaoNormalized(NormalizeForComparison(dto.Descricao), id))
            {
                errors.Add("Já existe uma unidade de medida cadastrada com esta descrição.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Abreviatura) &&
                await _unidadeMedida.ExistsByAbreviaturaNormalized(NormalizeForComparison(dto.Abreviatura), id))
            {
                errors.Add("Já existe uma unidade de medida cadastrada com esta abreviatura.");
            }

            if (id.HasValue && existing?.Situacao == Situacao.ATIVO && dto.Situacao == Situacao.INATIVO &&
                await _unidadeMedida.HasTiposDespesaAtivosVinculados(id.Value))
            {
                errors.Add("Não é permitido inativar unidade de medida com tipos de despesa ativos vinculados.");
            }
        }

        private static void ThrowIfInvalid(ICollection<string> errors)
        {
            if (errors.Count > 0) throw new UnidadeMedidaValidationException(errors);
        }

        private static string NormalizeSpaces(string? value) => Regex.Replace(value?.Trim() ?? string.Empty, "\\s+", " ");
        private static string NormalizeForComparison(string value) => NormalizeSpaces(value).ToUpperInvariant();
    }
}
