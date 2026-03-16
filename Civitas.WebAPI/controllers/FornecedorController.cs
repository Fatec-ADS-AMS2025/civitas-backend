using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    // Define a rota base da controller: api/Fornecedor
    [Route("api/fornecedores")]
    [ApiController]
    public class FornecedorController : Controller
    {
        // Servi�o respons�vel pela l�gica de neg�cios dos fornecedores
        private readonly IFornecedorService _fornecedorService;

        // Objeto padr�o para respostas da API
        private readonly Response _response;

        // Construtor com inje��o do service
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
            // Verifica se o DTO recebido � nulo
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inv�lidos";
                _response.Message = "Dados inv�lidos";

                return BadRequest(_response);
            }

            try
            {
                // Garante que o ID ser� gerado pelo banco
                fornecedorDTO.IdFornecedor = 0;

                // Chama o servi�o para criar o registro
                await _fornecedorService.Create(fornecedorDTO);

                // Retorna sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Resposta de erro com detalhes t�cnicos
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "N�o foi poss�vel cadastrar o fornecedor";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
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
                _response.Message = "Dados inv�lidos";
                _response.Message = "Dados inv�lidos";

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
                    _response.Message = "O fornecedor informado n�o existe";
                    return NotFound(_response);
                }

                // Atualiza o fornecedor
                await _fornecedorService.Update(fornecedorDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Erro no processo de atualiza��o
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do fornecedor";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   GET /api/Fornecedor
        //   Obt�m todos os fornecedores
        // ========================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Chama o service para listar todos
                var fornecedores = await _fornecedorService.GetAll();

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedores;
                _response.Message = "Lista de fornecedores obtida com sucesso";

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
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ========================================
        //   GET /api/Fornecedor/{id}
        //   Obt�m um fornecedor espec�fico
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
                    _response.Message = "Fornecedor n�o encontrado";
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
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ==========================================================
        //   PATCH /api/Fornecedor/{id}/alterar-situacao
        //   Altera apenas a situa��o ATIVO/INATIVO do fornecedor
        // ==========================================================
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                // Obt�m o fornecedor
                var fornecedor = await _fornecedorService.GetById(id);

                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor n�o encontrado";
                    return NotFound(_response);
                }

                // Alterna o enum:
                // ATIVO ? INATIVO
                // INATIVO ? ATIVO
                fornecedor.Situacao = fornecedor.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                // Salva atualiza��o
                await _fornecedorService.Update(fornecedor, id);

                // Monta resposta de sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    fornecedor.IdFornecedor,
                    Situacao = fornecedor.Situacao.ToString()
                };
                _response.Message = $"Situa��o alterada para {fornecedor.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Erro ao processar altera��o
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situa��o do fornecedor";
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
