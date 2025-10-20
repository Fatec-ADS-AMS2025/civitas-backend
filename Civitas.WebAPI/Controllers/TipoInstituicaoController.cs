using Civitas.WebAPI.Data.Repositories;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoInstituicaoController : ControllerBase
    {
        private readonly ITipoInstituicaoService _tipoInstituicaoService;
        private readonly Response _response;

        public TipoInstituicaoController(ITipoInstituicaoService tipoInstituicaoService)
        {
            _tipoInstituicaoService = tipoInstituicaoService;
            _response = new Response();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var instituicaoDto = await _tipoInstituicaoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Tipos de Instituições listados com sucesso";

            return Ok(_response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoInstituicaoById(int id)
        {
            var tipoInstituicaoDto = await _tipoInstituicaoService.GetById(id);
            if (tipoInstituicaoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum tipo de instituição encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Tipos de Instituições listados com sucesso";
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TipoInstituicaoDTO tipoInstituicaoDTO)
        {
            if (tipoInstituicaoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                tipoInstituicaoDTO.Id = 0;
                await _tipoInstituicaoService.Create(tipoInstituicaoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoInstituicaoDTO;
                _response.Message = "Tipo da instituição cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o tipo da instituição";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TipoInstituicaoDTO tipoInstituicaoDTO)
        {
            if (tipoInstituicaoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingTipoInstituicaoDTO = await _tipoInstituicaoService.GetById(id);
                if (existingTipoInstituicaoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O tipo da instituição informado não existe";
                    return NotFound(_response);
                }

                await _tipoInstituicaoService.Update(tipoInstituicaoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoInstituicaoDTO;
                _response.Message = "O tipo da instituição foi atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do tipo da instituição";
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
                var tipoInstituicao = await _tipoInstituicaoService.GetById(id);
                if (tipoInstituicao == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Tipo de instituição não encontrado.";
                    return NotFound(_response);
                }

                // Se estiver ativo e for desativar, checa se existem Instituições ativas vinculadas
                if (tipoInstituicao.Situacao == Situacao.ATIVO)
                {
                    var possuiAtivas = await _tipoInstituicaoService.ExisteInstituicoesAtivas(id);
                    if (possuiAtivas)
                    {
                        _response.Code = ResponseEnum.INVALID;
                        _response.Data = null;
                        _response.Message = "Não é possível desativar este tipo de instituição, pois existem instituições ativas vinculadas.";
                        return BadRequest(_response);
                    }

                    tipoInstituicao.Situacao = Situacao.INATIVO;
                }
                else
                {
                    // Se estiver inativo, pode ativar
                    tipoInstituicao.Situacao = Situacao.ATIVO;
                }

                // Atualiza o tipo de instituição
                await _tipoInstituicaoService.Update(tipoInstituicao, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    tipoInstituicao.Id,
                    Situacao = tipoInstituicao.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {tipoInstituicao.Situacao} com sucesso.";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do tipo de instituição.";
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
