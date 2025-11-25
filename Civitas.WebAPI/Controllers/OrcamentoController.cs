using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;
        private readonly Response _response;

        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orcamentoDto = await _orcamentoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "Orçamentos listados com sucesso";

            return Ok(_response);
        }

        [HttpGet("{idOrcamento}")]
        public async Task<IActionResult> GetOrcamentoById(int idOrcamento)
        {
            var orcamentoDto = await _orcamentoService.GetById(idOrcamento);
            if (orcamentoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum orçamento encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "Orçamento encontrado com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OrcamentoDTO orcamentoDTO)
        {
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                // Validações básicas
                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orçamento inválido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orçamento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Instituição inválida";
                    return BadRequest(_response);
                }

                orcamentoDTO.IdOrcamento = 0;
                await _orcamentoService.Create(orcamentoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "Orçamento cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o orçamento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{idOrcamento}")]
        public async Task<IActionResult> Put(int idOrcamento, OrcamentoDTO orcamentoDTO)
        {
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingOrcamentoDTO = await _orcamentoService.GetById(idOrcamento);
                if (existingOrcamentoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O orçamento informado não existe";
                    return NotFound(_response);
                }

                // Validações básicas
                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orçamento inválido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orçamento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Instituição inválida";
                    return BadRequest(_response);
                }

                await _orcamentoService.Update(orcamentoDTO, idOrcamento);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "Orçamento atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do orçamento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("{idOrcamento}")]
        public async Task<IActionResult> Delete(int idOrcamento)
        {
            try
            {
                var orcamento = await _orcamentoService.GetById(idOrcamento);
                if (orcamento == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Orçamento não encontrado";
                    return NotFound(_response);
                }

                if (await _orcamentoService.ExisteDespesaVinculada(idOrcamento))
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Message = "Não é possível excluir o orçamento, pois há despesas vinculadas a ele.";
                    return BadRequest(_response);
                }


                await _orcamentoService.Remove(idOrcamento);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Orçamento excluído com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir o orçamento";
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
