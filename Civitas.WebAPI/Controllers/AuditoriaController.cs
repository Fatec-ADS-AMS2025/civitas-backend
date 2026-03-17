using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento das auditorias do sistema.
    /// </summary>
    [Route("api/auditorias")]
    [ApiController]
    public class AuditoriaController : Controller
    {
        private readonly IAuditoriaService _auditoriaService;
        private readonly Response _response;

        /// <summary>
        /// Construtor que inicializa o serviço de auditoria
        /// e a estrutura padrão de resposta.
        /// </summary>
        /// <param name="auditoriaService">Serviço de auditoria.</param>
        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
            _response = new Response();
        }

        /// <summary>
        /// Lista todas as auditorias.
        /// </summary>
        /// <returns>Lista completa das auditorias.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                var auditorias = await _auditoriaService.GetPage(paginationQuery);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Lista de auditorias obtida com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditoria pelo identificador.
        /// </summary>
        /// <param name="id">ID da auditoria.</param>
        /// <returns>Auditoria correspondente ao ID informado.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);

                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Message = "Auditoria não encontrada";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditoria obtida com sucesso";
                _response.Data = auditoria;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias pelo ID do usuário.
        /// </summary>
        /// <param name="usuarioId">Identificador do usuário.</param>
        /// <returns>Lista de auditorias associadas ao usuário.</returns>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByUsuarioId(usuarioId);

                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Message = "Nenhuma auditoria encontrada para este usuário";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditorias do usuário listadas com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias do usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias pelo nome da entidade.
        /// </summary>
        /// <param name="nomeEntidade">Nome da entidade auditada.</param>
        /// <returns>Lista de auditorias da entidade.</returns>
        [HttpGet("entidade")]
        public async Task<IActionResult> GetByEntidade([FromQuery] string nomeEntidade)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByEntidade(nomeEntidade);

                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Message = "Nenhuma auditoria encontrada para esta entidade";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditorias da entidade listadas com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias da entidade";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Busca auditorias pelo tipo de operação.
        /// </summary>
        /// <param name="operacao">Tipo de operação (CREATE, UPDATE, DELETE...).</param>
        /// <returns>Lista de auditorias da operação informada.</returns>
        [HttpGet("operacao")]
        public async Task<IActionResult> GetByOperacao([FromQuery] string operacao)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByOperacao(operacao);

                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Message = "Nenhuma auditoria encontrada para esta operação";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditorias da operação listadas com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias da operação";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Cadastra uma nova auditoria.
        /// </summary>
        /// <param name="auditoriaDTO">Objeto contendo as informações da auditoria.</param>
        /// <returns>Auditoria registrada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = "Dados inválidos";
                _response.Data = null;
                return BadRequest(_response);
            }

            try
            {
                auditoriaDTO.Id = 0; // Garantia de criação
                await _auditoriaService.Create(auditoriaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditoria cadastrada com sucesso";
                _response.Data = auditoriaDTO;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza uma auditoria existente.
        /// </summary>
        /// <param name="id">ID da auditoria a ser atualizada.</param>
        /// <param name="auditoriaDTO">Novos dados da auditoria.</param>
        /// <returns>Auditoria atualizada.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = "Dados inválidos";
                _response.Data = null;
                return BadRequest(_response);
            }

            try
            {
                var existingAuditoria = await _auditoriaService.GetById(id);

                if (existingAuditoria is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Message = "A auditoria informada não existe";
                    _response.Data = null;
                    return NotFound(_response);
                }

                await _auditoriaService.Update(auditoriaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditoria atualizada com sucesso";
                _response.Data = auditoriaDTO;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove uma auditoria pelo ID.
        /// </summary>
        /// <param name="id">Identificador da auditoria a remover.</param>
        /// <returns>Resultado da exclusão.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);

                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Message = "Auditoria não encontrada";
                    _response.Data = null;
                    return NotFound(_response);
                }

                await _auditoriaService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditoria excluída com sucesso";
                _response.Data = null;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir a auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Altera a situação da auditoria (ATIVO / INATIVO).
        /// </summary>
        /// <param name="id">Identificador da auditoria.</param>
        /// <returns>Status atualizado da auditoria.</returns>
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var auditoria = await _auditoriaService.GetById(id);

                if (auditoria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Message = "Auditoria não encontrada";
                    _response.Data = null;
                    return NotFound(_response);
                }

                auditoria.Situacao = auditoria.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _auditoriaService.Update(auditoria, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = $"Situação alterada para {auditoria.Situacao} com sucesso";
                _response.Data = new
                {
                    auditoria.Id,
                    Situacao = auditoria.Situacao.ToString()
                };

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação da auditoria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
