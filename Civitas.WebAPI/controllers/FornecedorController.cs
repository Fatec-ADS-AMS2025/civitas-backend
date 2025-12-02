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
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedorController : Controller
    {
        // Serviço responsável pela lógica de negócios dos fornecedores
        private readonly IFornecedorService _fornecedorService;

        // Objeto padrão para respostas da API
        private readonly Response _response;

        // Construtor com injeção do service
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
            // Verifica se o DTO recebido é nulo
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";
                return BadRequest(_response);
            }

            try
            {
                // Garante que o ID será gerado pelo banco
                fornecedorDTO.IdFornecedor = 0;

                // Chama o serviço para criar o registro
                await _fornecedorService.Create(fornecedorDTO);

                // Retorna sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Resposta de erro com detalhes técnicos
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o fornecedor";
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
                _response.Message = "Dados inválidos";
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
                    _response.Message = "O fornecedor informado não existe";
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
                // Erro no processo de atualização
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
        //   Obtém todos os fornecedores
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
        //   Obtém um fornecedor específico
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
                    _response.Message = "Fornecedor não encontrado";
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
        //   Altera apenas a situação ATIVO/INATIVO do fornecedor
        // ==========================================================
        [HttpPatch("{id}/alterar-situacao")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                // Obtém o fornecedor
                var fornecedor = await _fornecedorService.GetById(id);

                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor não encontrado";
                    return NotFound(_response);
                }

                // Alterna o enum:
                // ATIVO ? INATIVO
                // INATIVO ? ATIVO
                fornecedor.Situacao = fornecedor.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                // Salva atualização
                await _fornecedorService.Update(fornecedor, id);

                // Monta resposta de sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    fornecedor.IdFornecedor,
                    Situacao = fornecedor.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {fornecedor.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Erro ao processar alteração
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
