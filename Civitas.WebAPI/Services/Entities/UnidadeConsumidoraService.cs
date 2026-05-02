using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Servico especializado na gestao de Unidades Consumidoras.
    /// </summary>
    public class UnidadeConsumidoraService
        : GenericService<UnidadeConsumidora, UnidadeConsumidoraDTO>, IUnidadeConsumidoraService
    {
        private readonly IUnidadeConsumidoraRepository _repository;
        private readonly IMapper _mapper;

        public UnidadeConsumidoraService(IUnidadeConsumidoraRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override async Task Create(UnidadeConsumidoraDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);
            ValidateFields(entityDTO);
            await ValidateRelationships(entityDTO);
            await ValidateUniqueness(entityDTO);

            entityDTO.Id = 0;
            entityDTO.Excluido = false;
            entityDTO.DataExclusao = null;

            var entity = _mapper.Map<UnidadeConsumidora>(entityDTO);
            await _repository.Add(entity);

            entityDTO.Id = entity.Id;
        }

        public override async Task Update(UnidadeConsumidoraDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existing = await _repository.GetById(id);
            if (existing is null)
            {
                throw new KeyNotFoundException($"Unidade consumidora com id {id} nao encontrada.");
            }

            if (existing.Excluido)
            {
                throw new UnidadeConsumidoraValidationException(
                    ["Nao e possivel atualizar uma unidade consumidora excluida. Restaure-a antes de editar."]);
            }

            Normalize(entityDTO);
            ValidateFields(entityDTO);
            await ValidateRelationships(entityDTO);
            await ValidateUniqueness(entityDTO, id);

            var entity = _mapper.Map<UnidadeConsumidora>(entityDTO);
            entity.Id = id;
            entity.Excluido = existing.Excluido;
            entity.DataExclusao = existing.DataExclusao;

            await _repository.Update(entity);

            entityDTO.Id = id;
            entityDTO.Excluido = existing.Excluido;
            entityDTO.DataExclusao = existing.DataExclusao;
        }

        public async Task<PaginatedResult<UnidadeConsumidoraDTO>> GetPageNaoExcluidos(PaginationQuery paginationQuery)
        {
            var page = await _repository.GetPageNaoExcluidos(paginationQuery);
            return MapPage(page);
        }

        public async Task<PaginatedResult<UnidadeConsumidoraDTO>> GetPageExcluidos(PaginationQuery paginationQuery)
        {
            var page = await _repository.GetPageExcluidos(paginationQuery);
            return MapPage(page);
        }

        public async Task<UnidadeConsumidoraDTO?> GetByIdNaoExcluidoAsync(int id)
        {
            var entity = await _repository.GetByIdNaoExcluidoAsync(id);
            return entity is null ? null : _mapper.Map<UnidadeConsumidoraDTO>(entity);
        }

        public async Task<UnidadeConsumidoraDTO?> GetByIdentificadorNaoExcluidoAsync(string identificador)
        {
            var entity = await _repository.GetByIdentificadorNaoExcluidoAsync(identificador?.Trim() ?? string.Empty);
            return entity is null ? null : _mapper.Map<UnidadeConsumidoraDTO>(entity);
        }

        public async Task<IEnumerable<UnidadeConsumidoraDTO>> GetByInstituicaoNaoExcluidosAsync(int idInstituicao)
        {
            var entities = await _repository.GetByInstituicaoNaoExcluidosAsync(idInstituicao);
            return _mapper.Map<IEnumerable<UnidadeConsumidoraDTO>>(entities);
        }

        public async Task<IEnumerable<UnidadeConsumidoraDTO>> GetBySecretariaNaoExcluidosAsync(int idSecretaria)
        {
            var entities = await _repository.GetBySecretariaNaoExcluidosAsync(idSecretaria);
            return _mapper.Map<IEnumerable<UnidadeConsumidoraDTO>>(entities);
        }

        public async Task<UnidadeConsumidoraDTO> ToggleStatusExclusaoAsync(int id)
        {
            var existing = await _repository.GetById(id);
            if (existing is null)
            {
                throw new KeyNotFoundException($"Unidade consumidora com id {id} nao encontrada.");
            }

            if (existing.Excluido)
            {
                existing.Excluido = false;
                existing.DataExclusao = null;
            }
            else
            {
                existing.Excluido = true;
                existing.DataExclusao = DateTime.UtcNow;
            }

            await _repository.Update(existing);

            return _mapper.Map<UnidadeConsumidoraDTO>(existing);
        }

        private static void ValidateDtoInstance(UnidadeConsumidoraDTO? dto)
        {
            if (dto is null)
            {
                throw new UnidadeConsumidoraValidationException(["O corpo da requisicao e obrigatorio."]);
            }
        }

        private static void Normalize(UnidadeConsumidoraDTO dto)
        {
            dto.Identificador = dto.Identificador?.Trim() ?? string.Empty;
        }

        private static void ValidateFields(UnidadeConsumidoraDTO dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Identificador))
            {
                errors.Add("O campo Identificador e obrigatorio.");
            }
            else if (dto.Identificador.Length > 100)
            {
                errors.Add("O Identificador deve ter no maximo 100 caracteres.");
            }

            ValidatePositiveId(dto.IdInstituicao, "Instituicao", errors);
            ValidatePositiveId(dto.IdTipoDespesa, "Tipo de despesa", errors);
            ValidatePositiveId(dto.IdSecretaria, "Secretaria", errors);
            ValidatePositiveId(dto.IdOrcamento, "Orcamento", errors);
            ValidatePositiveId(dto.IdFornecedor, "Fornecedor", errors);

            if (errors.Count > 0)
            {
                throw new UnidadeConsumidoraValidationException(errors);
            }
        }

        private async Task ValidateRelationships(UnidadeConsumidoraDTO dto)
        {
            var errors = new List<string>();

            if (dto.IdInstituicao > 0 && !await _repository.InstituicaoExistsAsync(dto.IdInstituicao))
            {
                errors.Add("A instituicao informada nao foi encontrada.");
            }

            if (dto.IdTipoDespesa > 0 && !await _repository.TipoDespesaExistsAsync(dto.IdTipoDespesa))
            {
                errors.Add("O tipo de despesa informado nao foi encontrado.");
            }

            if (dto.IdSecretaria > 0 && !await _repository.SecretariaExistsAsync(dto.IdSecretaria))
            {
                errors.Add("A secretaria informada nao foi encontrada.");
            }

            if (dto.IdOrcamento > 0 && !await _repository.OrcamentoExistsAsync(dto.IdOrcamento))
            {
                errors.Add("O orcamento informado nao foi encontrado.");
            }

            if (dto.IdFornecedor > 0 && !await _repository.FornecedorExistsAsync(dto.IdFornecedor))
            {
                errors.Add("O fornecedor informado nao foi encontrado.");
            }

            if (errors.Count > 0)
            {
                throw new UnidadeConsumidoraValidationException(errors);
            }
        }

        private async Task ValidateUniqueness(UnidadeConsumidoraDTO dto, int? ignoredId = null)
        {
            if (await _repository.ExistsByIdentificadorAsync(dto.Identificador, ignoredId))
            {
                throw new UnidadeConsumidoraConflictException(
                    "identificador",
                    "Ja existe uma unidade consumidora com este identificador.");
            }
        }

        private static void ValidatePositiveId(int value, string fieldName, ICollection<string> errors)
        {
            if (value <= 0)
            {
                errors.Add($"O campo {fieldName} e obrigatorio.");
            }
        }

        private PaginatedResult<UnidadeConsumidoraDTO> MapPage(PaginatedResult<UnidadeConsumidora> page)
        {
            return new PaginatedResult<UnidadeConsumidoraDTO>
            {
                Items = _mapper.Map<List<UnidadeConsumidoraDTO>>(page.Items),
                TotalRecords = page.TotalRecords,
                TotalPages = page.TotalPages,
                CurrentPage = page.CurrentPage,
                PageSize = page.PageSize
            };
        }
    }
}
