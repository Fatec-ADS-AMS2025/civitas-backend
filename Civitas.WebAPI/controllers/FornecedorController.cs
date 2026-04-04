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
    // Define a rota base da controller: api/Fornecedor
    [Authorize]
    [Route("api/fornecedores")]
    [ApiController]
    public class FornecedorController : Controller
    {
        // Serviï¿½o responsï¿½vel pela lï¿½gica de negï¿½cios dos fornecedores
        private readonly IFornecedorService _fornecedorService;

        // Objeto padrï¿½o para respostas da API
        private readonly Response _response;

        // Construtor com injeï¿½ï¿½o do service
        public FornecedorController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
            _response = new Response();
        }

        // ========================================
        //   POST /api/Fornecedor
        //   Cadastra um novo fornecedor
        // ========================================
        [HttpPost]
        public async Task<IActionResult> Post(FornecedorDTO fornecedorDTO)
        {
            // Verifica se o DTO recebido ï¿½ nulo
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invï¿½lidos";
                _response.Message = "Dados invï¿½lidos";

                return BadRequest(_response);
            }

            try
            {
                await _fornecedorService.ValidarCadastroAsync(fornecedorDTO);

                // Garante que o ID serï¿½ gerado pelo banco
                fornecedorDTO.IdFornecedor = 0;

                // Chama o serviï¿½o para criar o registro
                await _fornecedorService.Create(fornecedorDTO);

                // Retorna sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor cadastrado com sucesso";

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
                // Resposta de erro com detalhes tï¿½cnicos
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Nï¿½o foi possï¿½vel cadastrar o fornecedor";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   PUT /api/Fornecedor/{id}
        //   Atualiza um fornecedor existente
        // ========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FornecedorDTO fornecedorDTO)
        {
            // Valida o DTO
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invï¿½lidos";
                _response.Message = "Dados invï¿½lidos";

                return BadRequest(_response);
            }

            try
            {
                // Verifica se o fornecedor existe
                var existingFornecedorDTO = await _fornecedorService.GetById(id);

                if (existingFornecedorDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O fornecedor informado nï¿½o existe";
                    return NotFound(_response);
                }

                await _fornecedorService.ValidarCadastroAsync(fornecedorDTO, id);

                // Atualiza o fornecedor
                await _fornecedorService.Update(fornecedorDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor atualizado com sucesso";

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
                // Erro no processo de atualizaï¿½ï¿½o
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do fornecedor";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   GET /api/Fornecedor
        //   Obtï¿½m todos os fornecedores
        // ========================================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                // Chama o service para listar apenas os ativos
                var fornecedores = await _fornecedorService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.ATIVO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedores;
                _response.Message = "Lista de fornecedores ativos obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Retorna erro
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter os fornecedores";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   GET /api/fornecedores/inativos
        //   Obtém apenas fornecedores inativos
        // ========================================
        [HttpGet("inativos")]
        public async Task<IActionResult> GetInactive([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                var fornecedores = await _fornecedorService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.INATIVO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedores;
                _response.Message = "Lista de fornecedores inativos obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter os fornecedores inativos";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   GET /api/Fornecedor/{id}
        //   Obtï¿½m um fornecedor especï¿½fico
        // ========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // Busca o fornecedor pelo ID
                var fornecedor = await _fornecedorService.GetById(id);

                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor nï¿½o encontrado";
                    return NotFound(_response);
                }

                // Sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedor;
                _response.Message = "Fornecedor obtido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Erro ao obter fornecedor
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter o fornecedor";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ==========================================================
        //   PATCH /api/Fornecedor/{id}/alterar-situacao
        //   Altera apenas a situaï¿½ï¿½o ATIVO/INATIVO do fornecedor
        // ==========================================================
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                // Obtï¿½m o fornecedor
                var fornecedor = await _fornecedorService.GetById(id);

                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor nï¿½o encontrado";
                    return NotFound(_response);
                }

                // Alterna o enum:
                // ATIVO ? INATIVO
                // INATIVO ? ATIVO
                fornecedor.Situacao = fornecedor.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                // Salva atualizaï¿½ï¿½o
                await _fornecedorService.Update(fornecedor, id);

                // Monta resposta de sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    fornecedor.IdFornecedor,
                    Situacao = fornecedor.Situacao.ToString()
                };
                _response.Message = $"Situaï¿½ï¿½o alterada para {fornecedor.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Erro ao processar alteraï¿½ï¿½o
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situaï¿½ï¿½o do fornecedor";
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

