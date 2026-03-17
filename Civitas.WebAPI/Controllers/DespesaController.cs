using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de despesas cadastradas no sistema.
    /// </summary>
    /// <remarks>
    /// Este controller expõe endpoints REST para operações de CRUD e alteração de situação.
    /// 
    /// Funcionalidades disponíveis:
    /// - Listar todas as despesas
    /// - Consultar despesa por ID
    /// - Criar nova despesa
    /// - Atualizar uma despesa existente
    /// - Alterar situação (Ativo/Inativo)
    ///
    /// Observações aos desenvolvedores:
    /// - Todos os retornos seguem o padrão de objeto <see cref="Response"/>.
    /// - Erros internos são retornados como Status 500 contendo mensagem + StackTrace.
    /// - Situação usa o enum <see cref="Situacao"/>.
    /// </remarks>
    [Route("api/despesas")]
    [ApiController]
    public class DespesaController : ControllerBase
    {
        private readonly IDespesaService _despesaService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controller com suas dependências.
        /// </summary>
        /// <param name="despesaService">Serviço de regras de negócio e persistência de despesas.</param>
        public DespesaController(IDespesaService despesaService)
        {
            _despesaService = despesaService;
            _response = new Response();
        }

        // ======================================================================================================
        // GET /api/despesa
        // ======================================================================================================

        /// <summary>
        /// Retorna todas as despesas cadastradas.
        /// </summary>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET  
        ///
        /// <b>Exemplo de Request:</b>  
        /// GET /api/despesa
        ///
        /// <b>Exemplo de Response:</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "message": "Despesas listadas com sucesso",
        ///   "data": [ ... ]
        /// }
        /// 
        /// Possíveis Erros:
        /// - 500: Erro interno inesperado.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var usuarioDTO = await _despesaService.GetPage(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = usuarioDTO;
            _response.Message = "Despesas listadas com sucesso";

            return Ok(_response);
        }

        // ======================================================================================================
        // GET /api/despesa/{id}
        // ======================================================================================================

        /// <summary>
        /// Retorna uma despesa específica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da despesa.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET
        ///
        /// <b>Exemplo de Request:</b>  
        /// GET /api/despesa/10
        ///
        /// <b>Exemplo de Response (200):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "message": "Despesas listadas com sucesso",
        ///   "data": { ... }
        /// }
        ///
        /// <b>Exemplo de Response (404):</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "message": "Nenhuma despesa encontrada",
        ///   "data": null
        /// }
        ///
        /// Possíveis Erros:
        /// - 404: Não encontrado.
        /// - 500: Erro interno.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var despesaDto = await _despesaService.GetById(id);
            if (despesaDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhuma despesa encontrada";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesaDto;
            _response.Message = "Despesas listadas com sucesso";
            return Ok(_response);
        }

        // ======================================================================================================
        // POST /api/despesa
        // ======================================================================================================

        /// <summary>
        /// Cria uma nova despesa no sistema.
        /// </summary>
        /// <param name="despesaDTO">Objeto contendo os dados da despesa.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> POST
        ///
        /// <b>Exemplo de Request:</b>
        /// {
        ///   "descricao": "Conta de luz",
        ///   "valor": 150.75,
        ///   "situacao": "ATIVO"
        /// }
        ///
        /// <b>Exemplo de Response:</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "message": "Despesa cadastrada com sucesso",
        ///   "data": { ... }
        /// }
        ///
        /// Possíveis Erros:
        /// - 400: Dados inválidos
        /// - 500: Erro ao salvar no banco
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(DespesaDTO despesaDTO)
        {
            if (despesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                despesaDTO.Id = 0;
                await _despesaService.Create(despesaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = despesaDTO;
                _response.Message = "Despesa cadastrada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ======================================================================================================
        // PUT /api/despesa/{id}
        // ======================================================================================================

        /// <summary>
        /// Atualiza uma despesa existente.
        /// </summary>
        /// <param name="id">ID da despesa.</param>
        /// <param name="despesaDTO">Dados atualizados.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> PUT
        ///
        /// <b>Exemplo de Request:</b>
        /// {
        ///   "descricao": "Conta de luz - mês atual",
        ///   "valor": 180.00,
        ///   "situacao": "ATIVO"
        /// }
        ///
        /// Possíveis Erros:
        /// - 400: Dados inválidos
        /// - 404: Despesa não encontrada
        /// - 500: Erro interno
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DespesaDTO despesaDTO)
        {
            if (despesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingDespesaDTO = await _despesaService.GetById(id);
                if (existingDespesaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A despesa informada não existe";
                    return NotFound(_response);
                }

                await _despesaService.Update(despesaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = despesaDTO;
                _response.Message = "Despesa atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ======================================================================================================
        // PATCH /api/despesa/{id}/alterar-situacao
        // ======================================================================================================

        /// <summary>
        /// Alterna a situação da despesa entre ATIVO e INATIVO.
        /// </summary>
        /// <param name="id">ID da despesa.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> PATCH  
        ///
        /// <b>Exemplo de Request:</b>
        /// PATCH /api/despesa/20/alterar-situacao
        ///
        /// <b>Exemplo de Response:</b>
        /// {
        ///   "code": "SUCCESS",
        ///   "message": "Situação alterada para INATIVO com sucesso",
        ///   "data": {
        ///       "id": 20,
        ///       "situacao": "INATIVO"
        ///   }
        /// }
        ///
        /// Possíveis Erros:
        /// - 404: Despesa não encontrada  
        /// - 500: Erro interno  
        ///
        /// Observação: Nenhum corpo de request é necessário.
        /// </remarks>
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var despesa = await _despesaService.GetById(id);
                if (despesa == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Despesa não encontrada";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                despesa.Situacao = despesa.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _despesaService.Update(despesa, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    despesa.Id,
                    Situacao = despesa.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {despesa.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do fornecedor";
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
    
