using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsÃ¡vel pelo gerenciamento das auditorias do sistema.
    /// </summary>
    [Authorize]
    [Route("api/auditorias")]
    [ApiController]
    public class AuditoriaController : Controller
    {
        private readonly IAuditoriaService _auditoriaService;
        private readonly Response _response;

        /// <summary>
        /// Construtor que inicializa o serviÃ§o de auditoria
        /// e a estrutura padrÃ£o de resposta.
        /// </summary>
        /// <param name="auditoriaService">ServiÃ§o de auditoria.</param>
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
                var auditorias = await _auditoriaService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.ATIVO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Lista de auditorias ativas obtida com sucesso";
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
                    _response.Message = "Auditoria nÃ£o encontrada";
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
        /// Busca auditorias pelo ID do usuÃ¡rio.
        /// </summary>
        /// <param name="usuarioId">Identificador do usuÃ¡rio.</param>
        /// <returns>Lista de auditorias associadas ao usuÃ¡rio.</returns>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByUsuarioId(usuarioId);

                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Message = "Nenhuma auditoria encontrada para este usuÃ¡rio";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditorias do usuÃ¡rio listadas com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias do usuÃ¡rio";
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
        /// Busca auditorias pelo tipo de operaÃ§Ã£o.
        /// </summary>
        /// <param name="operacao">Tipo de operaÃ§Ã£o (CREATE, UPDATE, DELETE...).</param>
        /// <returns>Lista de auditorias da operaÃ§Ã£o informada.</returns>
        [HttpGet("operacao")]
        public async Task<IActionResult> GetByOperacao([FromQuery] string operacao)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByOperacao(operacao);

                if (auditorias == null || !auditorias.Any())
                {
                    _response.Code = ResponseEnum.SUCCESS;
                    _response.Message = "Nenhuma auditoria encontrada para esta operaÃ§Ã£o";
                    _response.Data = null;
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditorias da operaÃ§Ã£o listadas com sucesso";
                _response.Data = auditorias;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as auditorias da operaÃ§Ã£o";
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
        /// <param name="auditoriaDTO">Objeto contendo as informaÃ§Ãµes da auditoria.</param>
        /// <returns>Auditoria registrada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(AuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = "Dados invÃ¡lidos";
                _response.Data = null;
                return BadRequest(_response);
            }

            try
            {
                auditoriaDTO.Id = 0; // Garantia de criaÃ§Ã£o
                await _auditoriaService.Create(auditoriaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Message = "Auditoria cadastrada com sucesso";
                _response.Data = auditoriaDTO;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "NÃ£o foi possÃ­vel cadastrar a auditoria";
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

