using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : Controller
    {
        private readonly IAuditoriaService _auditoriaService;
        private readonly Response _response;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
            _response = new Response();
        }

        /// <summary>
        /// Lista todas as auditorias
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var auditorias = await _auditoriaService.GetAll();

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditorias;
                _response.Message = "Lista de auditorias obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditoria por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);
                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Auditoria não encontrada";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditoria;
                _response.Message = "Auditoria obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias por ID do usuário
        /// </summary>
        [HttpGet("GetByUsuarioId/{usuarioId}")]
        public async Task<IActionResult> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByUsuarioId(usuarioId);
                
                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Data = null;
                    _response.Message = "Nenhuma auditoria encontrada para este usuário";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditorias;
                _response.Message = "Auditorias do usuário listadas com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias do usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias por nome da entidade
        /// </summary>
        [HttpGet("GetByEntidade")]
        public async Task<IActionResult> GetByEntidade(string nomeEntidade)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByEntidade(nomeEntidade);
                
                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Data = null;
                    _response.Message = "Nenhuma auditoria encontrada para esta entidade";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditorias;
                _response.Message = "Auditorias da entidade listadas com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias da entidade";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias por tipo de operação
        /// </summary>
        [HttpGet("GetByOperacao")]
        public async Task<IActionResult> GetByOperacao(string operacao)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByOperacao(operacao);
                
                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Data = null;
                    _response.Message = "Nenhuma auditoria encontrada para esta operação";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditorias;
                _response.Message = "Auditorias da operação listadas com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias da operação";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Cadastra uma nova auditoria
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post(AuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                auditoriaDTO.Id = 0;
                await _auditoriaService.Create(auditoriaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditoriaDTO;
                _response.Message = "Auditoria cadastrada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza uma auditoria existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingAuditoriaDTO = await _auditoriaService.GetById(id);
                if (existingAuditoriaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A auditoria informada não existe";
                    return NotFound(_response);
                }

                await _auditoriaService.Update(auditoriaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = auditoriaDTO;
                _response.Message = "Auditoria atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove uma auditoria
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);
                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Auditoria não encontrada";
                    return NotFound(_response);
                }

                await _auditoriaService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Auditoria excluída com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Altera a situação de uma auditoria (ATIVO/INATIVO)
        /// </summary>
        [HttpPatch("{id}/alterar-situacao")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);
                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Auditoria não encontrada";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                auditoria.Situacao = auditoria.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _auditoriaService.Update(auditoria, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    auditoria.Id,
                    Situacao = auditoria.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {auditoria.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação da auditoria";
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
