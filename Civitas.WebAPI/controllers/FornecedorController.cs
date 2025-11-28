using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/fornecedores")]
    [ApiController]
    public class FornecedorController : Controller
    {
        private readonly IFornecedorService _fornecedorService;
        private readonly Response _response;

        public FornecedorController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
            _response = new Response();
        }

        [HttpPost]
        public async Task<IActionResult> Post(FornecedorDTO fornecedorDTO)
        {
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inv�lidos";

                return BadRequest(_response);
            }

            try
            {
                fornecedorDTO.IdFornecedor = 0;
                await _fornecedorService.Create(fornecedorDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FornecedorDTO fornecedorDTO)
        {
            if (fornecedorDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inv�lidos";

                return BadRequest(_response);
            }

            try
            {
                var existingFornecedorDTO = await _fornecedorService.GetById(id);
                if (existingFornecedorDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O fornecedor informado n�o existe";
                    return NotFound(_response);
                }

                await _fornecedorService.Update(fornecedorDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedorDTO;
                _response.Message = "Fornecedor atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var fornecedores = await _fornecedorService.GetAll();

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedores;
                _response.Message = "Lista de fornecedores obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var fornecedor = await _fornecedorService.GetById(id);
                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor n�o encontrado";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fornecedor;
                _response.Message = "Fornecedor obtido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
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

        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var fornecedor = await _fornecedorService.GetById(id);
                if (fornecedor == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fornecedor n�o encontrado";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                fornecedor.Situacao = fornecedor.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _fornecedorService.Update(fornecedor, id);

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
