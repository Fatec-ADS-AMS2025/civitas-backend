using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DespesaController : ControllerBase
    {
        private readonly IDespesaService _despesaService;
        private readonly Response _response;

        public DespesaController(IDespesaService despesaService)
        {
            _despesaService = despesaService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioDTO = await _despesaService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = usuarioDTO;
            _response.Message = "Despesas listadas com sucesso";

            return Ok(_response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var despesaDto = await _despesaService.GetById(id);
            if (despesaDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhuma despesa encontrada";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesaDto;
            _response.Message = "Despesas listadas com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(DespesaDTO despesaDTO)
        {
            if (despesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                despesaDTO.Id = 0;
                await _despesaService.Create(despesaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = despesaDTO;
                _response.Message = "Despesa cadastrada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DespesaDTO despesaDTO)
        {
            if (despesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingDespesaDTO = await _despesaService.GetById(id);
                if (existingDespesaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A despesa informada não existe";
                    return NotFound(_response);
                }

                await _despesaService.Update(despesaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = despesaDTO;
                _response.Message = "Despesa atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }


        [HttpPatch("{id}/alterar-situacao")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var despesa = await _despesaService.GetById(id);
                if (despesa == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Despesa não encontrada";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                despesa.Situacao = despesa.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _despesaService.Update(despesa, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    despesa.Id,
                    Situacao = despesa.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {despesa.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do fornecedor";
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
