using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar Documentos dentro do sistema.
    /// </summary>
    /// <remarks>
    /// Funcionalidades disponíveis:
    /// - Listar todos os Documentos
    /// - Buscar Documento por ID
    /// - Criar novo Documento
    /// - Atualizar Documento
    /// - Excluir Documento
    ///
    /// Observações:
    /// - Todos os retornos seguem o padrão do objeto Response.
    /// - Erros internos retornam Status 500 com detalhes.
    /// </remarks>
    [Route("api/documentos")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly Response _response;

        /// <summary>
        /// Construtor com injeção de dependência.
        /// </summary>
        public DocumentoController(IDocumentoService documentoService)
        {
            _documentoService = documentoService;
            _response = new Response();
        }

        // ======================================================================================================
        // GET /api/documento
        // ======================================================================================================

        /// <summary>
        /// Retorna a lista de todos os Documentos cadastrados no sistema.
        /// </summary>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET  
        ///
        /// Possíveis respostas:
        /// - 200: Documentos listados com sucesso
        /// - 500: Erro interno
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var documentoDto = await _documentoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = documentoDto;
            _response.Message = "Documentos listados com sucesso";

            return Ok(_response);
        }

        // ======================================================================================================
        // GET /api/documento/{id}
        // ======================================================================================================

        /// <summary>
        /// Retorna os dados de um Documento específico pelo ID.
        /// </summary>
        /// <param name="id">ID do Documento.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> GET
        ///
        /// Possíveis respostas:
        /// - 200: Documento encontrado
        /// - 404: Documento não encontrado
        /// - 500: Erro interno
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentoById(int id)
        {
            var fornecedorDto = await _documentoService.GetById(id);
            if (fornecedorDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum Documento encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = fornecedorDto;
            _response.Message = "Documentos listados com sucesso";
            return Ok(_response);
        }

        // ======================================================================================================
        // POST /api/documento
        // ======================================================================================================

        /// <summary>
        /// Cadastra um novo Documento.
        /// </summary>
        /// <param name="DocumentoDTO">Dados do Documento.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> POST  
        ///
        /// Possíveis respostas:
        /// - 200: Documento criado com sucesso
        /// - 400: Dados inválidos
        /// - 500: Erro interno ao cadastrar
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(DocumentoDTO DocumentoDTO)
        {
            if (DocumentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                // ID deve ser zerado antes da criação
                DocumentoDTO.IdDocumento = 0;
                await _documentoService.Create(DocumentoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = DocumentoDTO;
                _response.Message = "Documento cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o Documento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ======================================================================================================
        // PUT /api/documento/{id}
        // ======================================================================================================

        /// <summary>
        /// Atualiza os dados de um Documento existente.
        /// </summary>
        /// <param name="id">ID do Documento.</param>
        /// <param name="documentoDTO">Dados atualizados.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> PUT
        ///
        /// Possíveis respostas:
        /// - 200: Documento atualizado
        /// - 400: Dados inválidos
        /// - 404: Documento não existe
        /// - 500: Erro interno ao atualizar
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DocumentoDTO documentoDTO)
        {
            if (documentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingDocumentoDTO = await _documentoService.GetById(id);
                if (existingDocumentoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O Documento informado não existe";
                    return NotFound(_response);
                }

                await _documentoService.Update(documentoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = documentoDTO;
                _response.Message = "Documneto atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do documento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ======================================================================================================
        // DELETE /api/documento/{id}
        // ======================================================================================================

        /// <summary>
        /// Remove um Documento do sistema.
        /// </summary>
        /// <param name="id">ID do Documento.</param>
        /// <remarks>
        /// <b>Verbo HTTP:</b> DELETE  
        ///
        /// Possíveis respostas:
        /// - 200: Documento excluído
        /// - 404: Documento não encontrado
        /// - 500: Erro interno ao excluir
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var fornecedor = await _documentoService.GetById(id);
                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Documento não encontrado";
                    return NotFound(_response);
                }

                await _documentoService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Documento excluído com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir o documento";
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
