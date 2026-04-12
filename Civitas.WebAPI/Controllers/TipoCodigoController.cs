using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento dos Tipos de Código.
    /// </summary>
    [Authorize]
    [Route("api/tipo-codigo")]
    [ApiController]
    public class TipoCodigoController : ControllerBase
    {
        private readonly ITipoCodigoService _tipoCodigoService;
        private readonly Response _response;

        public TipoCodigoController(ITipoCodigoService tipoCodigoService)
        {
            _tipoCodigoService = tipoCodigoService;
            _response = new Response();
        }

        /// <summary>
        /// Lista todos os tipos de código.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tipoCodigoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = result;
            _response.Message = "Tipos de código listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Busca um tipo de código pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _tipoCodigoService.GetById(id);

            if (result == null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Tipo de código não encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = result;
            _response.Message = "Tipo de código encontrado com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Cadastra um novo tipo de código.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post(TipoCodigoDTO dto)
        {
            if (dto == null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                await _tipoCodigoService.Create(dto);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = dto;
                _response.Message = "Tipo de código cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Erro ao cadastrar tipo de código";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza um tipo de código existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TipoCodigoDTO dto)
        {
            if (dto == null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existing = await _tipoCodigoService.GetById(id);
                if (existing == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Tipo de código não encontrado";

                    return NotFound(_response);
                }

                await _tipoCodigoService.Update(dto, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = dto;
                _response.Message = "Tipo de código atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Erro ao atualizar tipo de código";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove um tipo de código.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = await _tipoCodigoService.GetById(id);
                if (existing == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Tipo de código não encontrado";

                    return NotFound(_response);
                }

                await _tipoCodigoService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Tipo de código removido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Erro ao remover tipo de código";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}