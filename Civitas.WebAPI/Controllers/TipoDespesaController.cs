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
    public class TipoDespesaController : ControllerBase
    {
        private readonly ITipoDespesaService _tipoDespesaService;
        private readonly Response _response;

        public TipoDespesaController(ITipoDespesaService tipoDespesaService)
        {
            _tipoDespesaService = tipoDespesaService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var instituicaoDto = await _tipoDespesaService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Tipos de despesa listados com sucesso";

            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoInstituicaoById(int id)
        {
            var tipoInstituicaoDto = await _tipoDespesaService.GetById(id);
            if (tipoInstituicaoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum tipo de despesa encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Tipos de despesa listados com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TipoDespesaDTO tipoDespesaDTO)
        {
            if (tipoDespesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                tipoDespesaDTO.Id = 0;
                await _tipoDespesaService.Create(tipoDespesaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoDespesaDTO;
                _response.Message = "Tipo de despesa cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o tipo de despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TipoDespesaDTO tipoDespesaDTO)
        {
            if (tipoDespesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingTipoDespesaDTO = await _tipoDespesaService.GetById(id);
                if (existingTipoDespesaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O tipo de despesa informado não existe";
                    return NotFound(_response);
                }

                await _tipoDespesaService.Update(tipoDespesaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoDespesaDTO;
                _response.Message = "O tipo de despesa foi atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do tipo de despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPatch("{id}/AlterarSituacao")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                // Busca o tipo de instituição pelo id
                var tipoDespesa = await _tipoDespesaService.GetById(id);
                if (tipoDespesa == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Tipo de instituição não encontrado.";
                    return NotFound(_response);
                }

                // Se estiver ativo e for desativar, checa se existem Instituições ativas vinculadas
                if (tipoDespesa.Situacao == Situacao.ATIVO)
                {
                    var possuiAtivas = await _tipoDespesaService.ExisteUnidadesDeMedidaAtivas(id);
                    if (possuiAtivas)
                    {
                        _response.Code = ResponseEnum.INVALID;
                        _response.Data = null;
                        _response.Message = "Não é possível desativar este tipo de despesa, pois existem unidades de medida ativas vinculadas.";
                        return BadRequest(_response);
                    }

                    tipoDespesa.Situacao = Situacao.INATIVO;
                }
                else
                {
                    // Se estiver inativo, pode ativar
                    tipoDespesa.Situacao = Situacao.ATIVO;
                }

                // Atualiza o tipo de instituição
                await _tipoDespesaService.Update(tipoDespesa, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    tipoDespesa.Id,
                    Situacao = tipoDespesa.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {tipoDespesa.Situacao} com sucesso.";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do tipo de despesa.";
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
