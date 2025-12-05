using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/unidade-medida")]
    [ApiController]
    public class UnidadeMedidaController : ControllerBase
    {
        private readonly IUnidadeMedidaService _unidadeMedidaService;
        private readonly Response _response;

        public UnidadeMedidaController(IUnidadeMedidaService unidadeMedidaService)
        {
            _unidadeMedidaService = unidadeMedidaService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var instituicaoDto = await _unidadeMedidaService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Unidades de medida listadas com sucesso";

            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstituicaoById(int id)
        {
            var tipoInstituicaoDto = await _unidadeMedidaService.GetById(id);
            if (tipoInstituicaoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhuma unidade de medida encontrada";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Unidades de medida listadas com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UnidadeMedidaDTO unidadeMedidaDTO)
        {
            if (unidadeMedidaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                unidadeMedidaDTO.Id = 0;
                await _unidadeMedidaService.Create(unidadeMedidaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = unidadeMedidaDTO;
                _response.Message = "Unidade de medida cadastrada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a unidade de medida";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UnidadeMedidaDTO unidadeMedidaDTO)
        {
            if (unidadeMedidaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingInstituicaoDTO = await _unidadeMedidaService.GetById(id);
                if (existingInstituicaoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A unidade de medida informada não existe";
                    return NotFound(_response);
                }

                await _unidadeMedidaService.Update(unidadeMedidaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = unidadeMedidaDTO;
                _response.Message = "Unidade de medida atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da unidade de medida";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var unidadeMedida = await _unidadeMedidaService.GetById(id);
                if (unidadeMedida == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Unidade de medida não encontrada";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                unidadeMedida.Situacao = unidadeMedida.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _unidadeMedidaService.Update(unidadeMedida, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    unidadeMedida.Id,
                    Situacao = unidadeMedida.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {unidadeMedida.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação da unidade de medida";
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
