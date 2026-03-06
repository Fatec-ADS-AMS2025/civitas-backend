using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão das Instituições.
    /// </summary>
    /// <remarks>
    /// Este controller fornece endpoints para:
    /// - Listagem de instituições
    /// - Consulta por ID
    /// - Consulta por nome
    /// - Criação
    /// - Atualização
    /// - Alteração de situação (Ativo/Inativo)
    ///
    /// Regras de Negócio:
    /// - A situação pode ser alternada via PATCH.
    /// - O ID não deve ser enviado no POST.
    ///
    /// Autenticação/Autorização:
    /// - *Não especificado*, presumidamente livre ou via políticas globais.
    /// </remarks>
    [Route("api/[controller]")]
    [Route("api/instituicoes")]
    [ApiController]
    public class InstituicaoController : ControllerBase
    {
        private readonly IInstituicaoService _instituicaoService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controller de Instituições.
        /// </summary>
        /// <param name="instituicaoService">Serviço de Instituições.</param>
        public InstituicaoController(IInstituicaoService instituicaoService)
        {
            _instituicaoService = instituicaoService;
            _response = new Response();
        }

        // ============================================================================================
        // GET: api/Instituicao
        // ============================================================================================

        /// <summary>
        /// Retorna todas as instituições cadastradas.
        /// </summary>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET  
        ///
        /// <b>Exemplo de Request:</b>
        /// GET /api/Instituicao
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "data": [...],
        ///   "message": "Instituições listadas com sucesso"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 500: Erro interno ao buscar instituições.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var instituicaoDto = await _instituicaoService.GetPage(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Instituições listadas com sucesso";

            return Ok(_response);
        }

        // ============================================================================================
        // GET: api/Instituicao/{id}
        // ============================================================================================

        /// <summary>
        /// Busca uma instituição pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador da instituição.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET  
        ///
        /// <b>Exemplo de Request:</b>
        /// GET /api/Instituicao/5
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "data": {...},
        ///   "message": "Instituições listadas com sucesso"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 404: Instituição não encontrada
        /// </remarks>
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

        // ============================================================================================
        // GET: api/Instituicao/GetInstituicaoByName?name=X
        // ============================================================================================

        /// <summary>
        /// Busca instituições pelo nome.
        /// </summary>
        /// <param name="name">Nome da instituição.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET  
        ///
        /// <b>Exemplo de Request:</b>
        /// GET /api/Instituicao/GetInstituicaoByName?name=Senac
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "data": [...],
        ///   "message": "Instituição listados com sucesso"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 404: Nenhuma instituição encontrada com esse nome
        /// </remarks>
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

        // ============================================================================================
        // POST: api/Instituicao
        // ============================================================================================

        /// <summary>
        /// Cria uma nova instituição.
        /// </summary>
        /// <param name="instituicaoDTO">Dados da instituição a ser criada.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> POST  
        ///
        /// <b>Exemplo de Request:</b>
        /// POST /api/Instituicao
        /// {
        ///   "nome": "Faculdade X",
        ///   "cnpj": "00.000.000/0001-00",
        ///   "situacao": "ATIVO"
        /// }
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "data": {...},
        ///   "message": "Instituição cadastrado com sucesso"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 400: Dados inválidos
        /// - 500: Erro ao tentar cadastrar
        ///
        /// Observação:
        /// - O ID é sobrescrito para 0 antes da criação.
        /// </remarks>
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

        // ============================================================================================
        // PUT: api/Instituicao/{id}
        // ============================================================================================

        /// <summary>
        /// Atualiza uma instituição existente.
        /// </summary>
        /// <param name="id">Identificador da instituição.</param>
        /// <param name="instituicaoDTO">Dados atualizados.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> PUT  
        ///
        /// <b>Exemplo de Request:</b>
        /// PUT /api/Instituicao/10
        /// {
        ///   "nome": "Nova Faculdade",
        ///   "situacao": "ATIVO"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 400: Dados inválidos
        /// - 404: Instituição não encontrada
        /// - 500: Erro interno ao atualizar
        /// </remarks>
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

        // ============================================================================================
        // PATCH: api/Instituicao/{id}/AlterarSituacao
        // ============================================================================================

        /// <summary>
        /// Alterna a situação da instituição entre ATIVO e INATIVO.
        /// </summary>
        /// <param name="id">Identificador da instituição.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> PATCH  
        ///
        /// <b>Exemplo de Request:</b>
        /// PATCH /api/Instituicao/10/AlterarSituacao
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "data": { "id": 10, "situacao": "INATIVO" },
        ///   "message": "Situação alterada com sucesso"
        /// }
        ///
        /// <b>Possíveis Erros:</b>
        /// - 404: Instituição não encontrada
        /// - 500: Erro ao alterar situação
        /// </remarks>
        [HttpPatch("{id}/AlterarSituacao")]
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

                // Alterna ATIVO/INATIVO
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
