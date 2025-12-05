using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
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
        public async Task<IActionResult> GetAll()
        {
            var instituicaoDto = await _instituicaoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituições listadas com sucesso";

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
                _response.Message = "Nenhuma instituição encontrada";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Instituições listadas com sucesso";
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
                _response.Message = "Nenhuma instituição encontrada com esse nome";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituição listados com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(InstituicaoDTO instituicaoDTO)
        {
            if (instituicaoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                instituicaoDTO.Id = 0;
                await _instituicaoService.Create(instituicaoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = instituicaoDTO;
                _response.Message = "Instituição cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a instituição";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
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
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingInstituicaoDTO = await _instituicaoService.GetById(id);
                if (existingInstituicaoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A instituição informada não existe";
                    return NotFound(_response);
                }

                await _instituicaoService.Update(instituicaoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = instituicaoDTO;
                _response.Message = "Instituição atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da instituição";
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
                var instituicao = await _instituicaoService.GetById(id);
                if (instituicao == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Instituição não encontrada";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
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
                _response.Message = $"Situação alterada para {instituicao.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação da instituição";
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
