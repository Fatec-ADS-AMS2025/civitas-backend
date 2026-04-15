using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsavel pela gestao das Instituicoes.
    /// </summary>
    [Authorize]
    [Route("api/instituicoes")]
    [ApiController]
    public class InstituicaoController : ControllerBase
    {
        private readonly IInstituicaoService _instituicaoService;
        private readonly Response _response;

        public InstituicaoController(IInstituicaoService instituicaoService)
        {
            _instituicaoService = instituicaoService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var instituicaoDto = await _instituicaoService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.ATIVO);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituicoes ativas listadas com sucesso";

            return Ok(_response);
        }

        [HttpGet("inativos")]
        public async Task<IActionResult> GetInactive([FromQuery] PaginationQuery paginationQuery)
        {
            var instituicaoDto = await _instituicaoService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.INATIVO);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituicoes inativas listadas com sucesso";

            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstituicaoById(int id)
        {
            var tipoInstituicaoDto = await _instituicaoService.GetById(id);
            if (tipoInstituicaoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhuma instituicao encontrada";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Instituicoes listadas com sucesso";
            return Ok(_response);
        }

        [HttpGet("nome")]
        public async Task<IActionResult> GetInstituicaoByName([FromQuery] string name)
        {
            var instituicaoDto = await _instituicaoService.GetInstituicaoByName(name);
            if (instituicaoDto is null || !instituicaoDto.Any())
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhuma instituicao encontrada com esse nome";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituicao listada com sucesso";
            return Ok(_response);
        }

        [HttpGet("{id}/gastos")]
        public async Task<IActionResult> GetGastosByInstituicao(int id)
        {
            var gastosInstituicao = await _instituicaoService.GetGastosByInstituicaoIdAsync(id);
            if (gastosInstituicao is null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Instituicao nao encontrada";
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = gastosInstituicao;
            _response.Message = "Gastos da instituicao obtidos com sucesso";
            return Ok(_response);
        }

        [HttpGet("{id}/orcamento-disponivel")]
        public async Task<IActionResult> GetOrcamentoDisponivelByInstituicao(int id)
        {
            var orcamentoInstituicao = await _instituicaoService.GetOrcamentoDisponivelByInstituicaoIdAsync(id);
            if (orcamentoInstituicao is null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Instituicao nao encontrada";
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoInstituicao;
            _response.Message = "Orcamento disponivel da instituicao obtido com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(InstituicaoDTO instituicaoDTO)
        {
            if (instituicaoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invalidos";

                return BadRequest(_response);
            }

            try
            {
                instituicaoDTO.Id = 0;
                await _instituicaoService.Create(instituicaoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = instituicaoDTO;
                _response.Message = "Instituicao cadastrada com sucesso";

                return Ok(_response);
            }
            catch (InstituicaoConflictException ex)
            {
                _response.Code = ResponseEnum.CONFLICT;
                _response.Data = ex.Field is null ? null : new[] { ex.Field };
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (InstituicaoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para a instituicao sao invalidos";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Nao foi possivel cadastrar a instituicao";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, InstituicaoDTO instituicaoDTO)
        {
            if (instituicaoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invalidos";

                return BadRequest(_response);
            }

            try
            {
                await _instituicaoService.Update(instituicaoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = instituicaoDTO;
                _response.Message = "Instituicao atualizada com sucesso";

                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (InstituicaoConflictException ex)
            {
                _response.Code = ResponseEnum.CONFLICT;
                _response.Data = ex.Field is null ? null : new[] { ex.Field };
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (InstituicaoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para a instituicao sao invalidos";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da instituicao";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var instituicao = await _instituicaoService.GetById(id);
                if (instituicao == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Instituicao nao encontrada";
                    return NotFound(_response);
                }

                instituicao.Situacao = instituicao.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _instituicaoService.Update(instituicao, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    instituicao.Id,
                    Situacao = instituicao.Situacao.ToString()
                };
                _response.Message = $"Situacao alterada para {instituicao.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (InstituicaoConflictException ex)
            {
                _response.Code = ResponseEnum.CONFLICT;
                _response.Data = ex.Field is null ? null : new[] { ex.Field };
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (InstituicaoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = "Os dados informados para a instituicao sao invalidos";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situacao da instituicao";
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
