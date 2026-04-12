using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsÃ¡vel pela gestÃ£o de Secretarias no sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Manipular operaÃ§Ãµes de CRUD das Secretarias.
    /// - Encapsular respostas padronizadas utilizando o objeto <see cref="Response"/>.
    /// 
    /// DependÃªncias:
    /// - <see cref="ISecretariaService"/>: ServiÃ§o responsÃ¡vel por regras de negÃ³cio e acesso a dados.
    /// </remarks>
    [Authorize]
    [Route("api/secretarias")]
    [ApiController]
    public class SecretariaController : Controller
    {
        private readonly ISecretariaService _secretariaService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controller de Secretaria.
        /// </summary>
        /// <param name="secretariaService">ServiÃ§o responsÃ¡vel pelas operaÃ§Ãµes de Secretaria.</param>
        public SecretariaController(ISecretariaService secretariaService)
        {
            _secretariaService = secretariaService;
            _response = new Response();
        }

        /// <summary>
        /// Cadastra uma nova Secretaria.
        /// </summary>
        /// <param name="secretariaDTO">Dados da Secretaria a ser criada.</param>
        /// <returns>Resultado da operaÃ§Ã£o de criaÃ§Ã£o.</returns>
        /// <remarks>
        /// Regras:
        /// - O objeto deve vir preenchido.
        /// - O ID sempre Ã© zerado para garantir criaÃ§Ã£o.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(SecretariaDTO secretariaDTO)
        {
            if (secretariaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invÃ¡lidos";

                return BadRequest(_response);
            }

            try
            {
                await _secretariaService.ValidarCadastroAsync(secretariaDTO);
                secretariaDTO.IdSecretaria = 0;
                await _secretariaService.Create(secretariaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretariaDTO;
                _response.Message = "Secretaria cadastrada com sucesso";

                return Ok(_response);
            }
            catch (ArgumentException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = ex.Message;
                _response.Data = null;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "NÃ£o foi possÃ­vel cadastrar a secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza os dados de uma Secretaria existente.
        /// </summary>
        /// <param name="id">ID da Secretaria a ser atualizada.</param>
        /// <param name="secretariaDTO">Dados atualizados.</param>
        /// <returns>Resultado da operaÃ§Ã£o de atualizaÃ§Ã£o.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SecretariaDTO secretariaDTO)
        {
            if (secretariaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invÃ¡lidos";

                return BadRequest(_response);
            }

            try
            {
                var existingSecretariaDTO = await _secretariaService.GetById(id);
                if (existingSecretariaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A secretaria informada nÃ£o existe";
                    return NotFound(_response);
                }

                await _secretariaService.ValidarCadastroAsync(secretariaDTO, id);
                await _secretariaService.Update(secretariaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretariaDTO;
                _response.Message = "Secretaria atualizada com sucesso";

                return Ok(_response);
            }
            catch (ArgumentException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = ex.Message;
                _response.Data = null;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Retorna a lista de todas as Secretarias cadastradas.
        /// </summary>
        /// <returns>Lista de Secretarias.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                var secretarias = await _secretariaService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.ATIVO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretarias;
                _response.Message = "Lista de secretarias ativas obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as secretarias";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Retorna a lista de Secretarias inativas.
        /// </summary>
        /// <returns>Lista de Secretarias inativas.</returns>
        [HttpGet("inativos")]
        public async Task<IActionResult> GetInactive([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                var secretarias = await _secretariaService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.INATIVO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretarias;
                _response.Message = "Lista de secretarias inativas obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as secretarias inativas";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Obtém uma Secretaria específica pelo ID.
        /// </summary>
        /// <param name="id">ID da Secretaria.</param>
        /// <returns>Dados da Secretaria solicitada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var secretaria = await _secretariaService.GetById(id);
                if (secretaria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Secretaria nÃ£o encontrada";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretaria;
                _response.Message = "Secretaria obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter a secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id}/gastos")]
        public async Task<IActionResult> GetGastosBySecretaria(int id)
        {
            try
            {
                var gastosSecretaria = await _secretariaService.GetGastosBySecretariaIdAsync(id);
                if (gastosSecretaria is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Secretaria não encontrada";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = gastosSecretaria;
                _response.Message = "Gastos da secretaria obtidos com sucesso";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter os gastos da secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{id}/orcamento-disponivel")]
        public async Task<IActionResult> GetOrcamentoDisponivelBySecretaria(int id)
        {
            try
            {
                var orcamentoSecretaria = await _secretariaService.GetOrcamentoDisponivelBySecretariaIdAsync(id);
                if (orcamentoSecretaria is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Secretaria não encontrada";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoSecretaria;
                _response.Message = "Orçamento disponível da secretaria obtido com sucesso";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter o orçamento disponível da secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Alterna a situaÃ§Ã£o (Ativo/Inativo) de uma Secretaria.
        /// </summary>
        /// <param name="id">ID da Secretaria.</param>
        /// <returns>Resultado com a nova situaÃ§Ã£o.</returns>
        /// <remarks>
        /// Regra:
        /// - Caso esteja ATIVO â†’ vira INATIVO
        /// - Caso esteja INATIVO â†’ vira ATIVO
        /// </remarks>
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var secretaria = await _secretariaService.GetById(id);
                if (secretaria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Secretaria nÃ£o encontrada";
                    return NotFound(_response);
                }

                secretaria.Situacao = secretaria.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _secretariaService.Update(secretaria, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    secretaria.IdSecretaria,
                    Situacao = secretaria.Situacao.ToString()
                };
                _response.Message = $"SituaÃ§Ã£o alterada para {secretaria.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situaÃ§Ã£o da secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}

