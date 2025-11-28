using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/fluxos")]
    [ApiController]
    public class FluxoController : Controller
    {
        private readonly IFluxoService _fluxoService;
        private readonly Response _response;

        public FluxoController(IFluxoService fluxoService)
        {
            _fluxoService = fluxoService;
            _response = new Response();
        }

        [HttpPost]
        public async Task<IActionResult> Post(FluxoDTO fluxoDTO)
        {
            if (fluxoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inv�lidos";

                return BadRequest(_response);
            }

            try
            {
                fluxoDTO.IdFluxo = 0;
                await _fluxoService.Create(fluxoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxoDTO;
                _response.Message = "Fluxo cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "N�o foi poss�vel cadastrar o fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FluxoDTO fluxoDTO)
        {
            if (fluxoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inv�lidos";

                return BadRequest(_response);
            }

            try
            {
                var existingFluxoDTO = await _fluxoService.GetById(id);
                if (existingFluxoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O fluxo informado n�o existe";
                    return NotFound(_response);
                }

                await _fluxoService.Update(fluxoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxoDTO;
                _response.Message = "Fluxo atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var fluxos = await _fluxoService.GetAll();

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxos;
                _response.Message = "Lista de fluxos obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter a lista de fluxos";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var fluxo = await _fluxoService.GetById(id);
                if (fluxo == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fluxo n�o encontrado";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxo;
                _response.Message = "Fluxo obtido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter o fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] Status novoStatus)
        {
            try
            {
                var fluxo = await _fluxoService.GetById(id);
                if (fluxo == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fluxo n�o encontrado";
                    return NotFound(_response);
                }

                // Atualiza o status conforme o valor recebido
                fluxo.Status = novoStatus;
                await _fluxoService.Update(fluxo, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    fluxo.IdFluxo,
                    StatusAtual = fluxo.Status.ToString()
                };
                _response.Message = $"Status alterado para '{fluxo.Status}' com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar o status do fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

    }
}
