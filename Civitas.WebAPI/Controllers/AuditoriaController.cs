using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela consulta das auditorias do sistema.
    /// </summary>
    /// <remarks>
    /// Somente leitura. Os registros são gerados pelo backend a partir das operações efetivamente
    /// persistidas e não podem ser criados, alterados ou excluídos pela API.
    /// O acesso exige usuário autenticado com perfil ADMINISTRADOR, mesmo com o modo DEV ativo
    /// (mesma política do Hub de auditoria).
    /// </remarks>
    [Authorize(Policy = AuditoriaAuthorization.PolicyName)]
    [Route("api/auditorias")]
    [ApiController]
    public class AuditoriaController : ControllerBase
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
        /// Lista as auditorias de forma paginada.
        /// </summary>
        /// <param name="paginationQuery">Página, tamanho e ordenação.</param>
        /// <returns>Página de auditorias.</returns>
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
            catch (Exception)
            {
                return ErroInterno("Ocorreu um erro ao obter as auditorias");
            }
        }

        /// <summary>
        /// Busca auditoria pelo identificador.
        /// </summary>
        /// <param name="id">ID da auditoria (maior que zero).</param>
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
            catch (AuditoriaValidationException ex)
            {
                return Invalido(ex);
            }
            catch (Exception)
            {
                return ErroInterno("Ocorreu um erro ao obter a auditoria");
            }
        }

        /// <summary>
        /// Busca auditorias pelo ID do usuário autor.
        /// </summary>
        /// <param name="usuarioId">Identificador do usuário (maior que zero).</param>
        /// <returns>Lista de auditorias associadas ao usuário.</returns>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuarioId(int usuarioId)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByUsuarioId(usuarioId);
                return ListaOuNaoEncontrado(auditorias,
                    "Auditorias do usuário listadas com sucesso",
                    "Nenhuma auditoria encontrada para este usuário");
            }
            catch (AuditoriaValidationException ex)
            {
                return Invalido(ex);
            }
            catch (Exception)
            {
                return ErroInterno("Ocorreu um erro ao obter as auditorias do usuário");
            }
        }

        /// <summary>
        /// Busca auditorias pelo nome da entidade.
        /// </summary>
        /// <param name="nomeEntidade">Nome da entidade auditada (ex: Despesa, Fornecedor, Autenticacao).</param>
        /// <returns>Lista de auditorias da entidade.</returns>
        [HttpGet("entidade")]
        public async Task<IActionResult> GetByEntidade([FromQuery] string? nomeEntidade)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByEntidade(nomeEntidade);
                return ListaOuNaoEncontrado(auditorias,
                    "Auditorias da entidade listadas com sucesso",
                    "Nenhuma auditoria encontrada para esta entidade");
            }
            catch (AuditoriaValidationException ex)
            {
                return Invalido(ex);
            }
            catch (Exception)
            {
                return ErroInterno("Ocorreu um erro ao obter as auditorias da entidade");
            }
        }

        /// <summary>
        /// Busca auditorias pelo tipo de operação.
        /// </summary>
        /// <param name="operacao">Tipo de operação: INSERT, UPDATE, DELETE ou LOGIN.</param>
        /// <returns>Lista de auditorias da operação informada.</returns>
        [HttpGet("operacao")]
        public async Task<IActionResult> GetByOperacao([FromQuery] string? operacao)
        {
            try
            {
                var auditorias = await _auditoriaService.GetByOperacao(operacao);
                return ListaOuNaoEncontrado(auditorias,
                    "Auditorias da operação listadas com sucesso",
                    "Nenhuma auditoria encontrada para esta operação");
            }
            catch (AuditoriaValidationException ex)
            {
                return Invalido(ex);
            }
            catch (Exception)
            {
                return ErroInterno("Ocorreu um erro ao obter as auditorias da operação");
            }
        }

        private IActionResult ListaOuNaoEncontrado<T>(IEnumerable<T> itens, string mensagemSucesso, string mensagemVazia)
        {
            var lista = itens.ToList();
            if (lista.Count == 0)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Message = mensagemVazia;
                _response.Data = null;
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Message = mensagemSucesso;
            _response.Data = lista;
            return Ok(_response);
        }

        private IActionResult Invalido(AuditoriaValidationException ex)
        {
            _response.Code = ResponseEnum.INVALID;
            _response.Message = ex.Message;
            _response.Data = ex.Errors;
            return BadRequest(_response);
        }

        /// <summary>Erro interno sem stack trace nem detalhes técnicos.</summary>
        private IActionResult ErroInterno(string mensagem)
        {
            _response.Code = ResponseEnum.ERROR;
            _response.Message = mensagem;
            _response.Data = null;
            return StatusCode(StatusCodes.Status500InternalServerError, _response);
        }
    }
}
