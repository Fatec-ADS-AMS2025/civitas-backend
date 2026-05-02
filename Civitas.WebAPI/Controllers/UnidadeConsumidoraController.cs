using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsavel pela gestao das Unidades Consumidoras.
    /// </summary>
    [Authorize]
    [Route("api/unidades-consumidoras")]
    [ApiController]
    public class UnidadeConsumidoraController : ControllerBase
    {
        private readonly IUnidadeConsumidoraService _service;
        private readonly Response _response;

        public UnidadeConsumidoraController(IUnidadeConsumidoraService service)
        {
            _service = service;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var page = await _service.GetPageNaoExcluidos(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = page;
            _response.Message = "Unidades consumidoras listadas com sucesso";
            return Ok(_response);
        }

        [HttpGet("excluidos")]
        public async Task<IActionResult> GetExcluidos([FromQuery] PaginationQuery paginationQuery)
        {
            var page = await _service.GetPageExcluidos(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = page;
            _response.Message = "Unidades consumidoras excluidas listadas com sucesso";
            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdNaoExcluidoAsync(id);
            if (dto is null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Unidade consumidora nao encontrada";
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = dto;
            _response.Message = "Unidade consumidora obtida com sucesso";
            return Ok(_response);
        }

        [HttpGet("identificador/{identificador}")]
        public async Task<IActionResult> GetByIdentificador(string identificador)
        {
            var dto = await _service.GetByIdentificadorNaoExcluidoAsync(identificador);
            if (dto is null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Unidade consumidora nao encontrada";
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = dto;
            _response.Message = "Unidade consumidora obtida com sucesso";
            return Ok(_response);
        }

        [HttpGet("instituicao/{idInstituicao}")]
        public async Task<IActionResult> GetByInstituicao(int idInstituicao)
        {
            var items = await _service.GetByInstituicaoNaoExcluidosAsync(idInstituicao);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = items;
            _response.Message = "Unidades consumidoras da instituicao listadas com sucesso";
            return Ok(_response);
        }

        [HttpGet("secretaria/{idSecretaria}")]
        public async Task<IActionResult> GetBySecretaria(int idSecretaria)
        {
            var items = await _service.GetBySecretariaNaoExcluidosAsync(idSecretaria);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = items;
            _response.Message = "Unidades consumidoras da secretaria listadas com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UnidadeConsumidoraDTO unidadeConsumidoraDTO)
        {
            if (unidadeConsumidoraDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invalidos";
                return BadRequest(_response);
            }

            try
            {
                await _service.Create(unidadeConsumidoraDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = unidadeConsumidoraDTO;
                _response.Message = "Unidade consumidora cadastrada com sucesso";
                return Ok(_response);
            }
            catch (UnidadeConsumidoraConflictException ex)
            {
                _response.Code = ResponseEnum.CONFLICT;
                _response.Data = ex.Field is null ? null : new[] { ex.Field };
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (UnidadeConsumidoraValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para a unidade consumidora sao invalidos";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Nao foi possivel cadastrar a unidade consumidora";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UnidadeConsumidoraDTO unidadeConsumidoraDTO)
        {
            if (unidadeConsumidoraDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invalidos";
                return BadRequest(_response);
            }

            try
            {
                await _service.Update(unidadeConsumidoraDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = unidadeConsumidoraDTO;
                _response.Message = "Unidade consumidora atualizada com sucesso";
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (UnidadeConsumidoraConflictException ex)
            {
                _response.Code = ResponseEnum.CONFLICT;
                _response.Data = ex.Field is null ? null : new[] { ex.Field };
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (UnidadeConsumidoraValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para a unidade consumidora sao invalidos";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar a unidade consumidora";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPatch("{id}/status-exclusao")]
        public async Task<IActionResult> ToggleStatusExclusao(int id)
        {
            try
            {
                var dto = await _service.ToggleStatusExclusaoAsync(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    dto.Id,
                    dto.Excluido,
                    dto.DataExclusao
                };
                _response.Message = dto.Excluido
                    ? "Unidade consumidora excluida (soft delete) com sucesso"
                    : "Unidade consumidora restaurada com sucesso";
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar o status de exclusao da unidade consumidora";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
