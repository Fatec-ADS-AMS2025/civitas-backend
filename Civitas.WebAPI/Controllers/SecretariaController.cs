using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de Secretarias no sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Manipular operações de CRUD das Secretarias.
    /// - Encapsular respostas padronizadas utilizando o objeto <see cref="Response"/>.
    /// 
    /// Dependências:
    /// - <see cref="ISecretariaService"/>: Serviço responsável por regras de negócio e acesso a dados.
    /// </remarks>
    [Route("api/[controller]")]
    [Route("api/secretarias")]
    [ApiController]
    public class SecretariaController : Controller
    {
        private readonly ISecretariaService _secretariaService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controller de Secretaria.
        /// </summary>
        /// <param name="secretariaService">Serviço responsável pelas operações de Secretaria.</param>
        public SecretariaController(ISecretariaService secretariaService)
        {
            _secretariaService = secretariaService;
            _response = new Response();
        }

        /// <summary>
        /// Cadastra uma nova Secretaria.
        /// </summary>
        /// <param name="secretariaDTO">Dados da Secretaria a ser criada.</param>
        /// <returns>Resultado da operação de criação.</returns>
        /// <remarks>
        /// Regras:
        /// - O objeto deve vir preenchido.
        /// - O ID sempre é zerado para garantir criação.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(SecretariaDTO secretariaDTO)
        {
            if (secretariaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                secretariaDTO.IdSecretaria = 0;
                await _secretariaService.Create(secretariaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretariaDTO;
                _response.Message = "Secretaria cadastrada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza os dados de uma Secretaria existente.
        /// </summary>
        /// <param name="id">ID da Secretaria a ser atualizada.</param>
        /// <param name="secretariaDTO">Dados atualizados.</param>
        /// <returns>Resultado da operação de atualização.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SecretariaDTO secretariaDTO)
        {
            if (secretariaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingSecretariaDTO = await _secretariaService.GetById(id);
                if (existingSecretariaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "A secretaria informada não existe";
                    return NotFound(_response);
                }

                await _secretariaService.Update(secretariaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretariaDTO;
                _response.Message = "Secretaria atualizada com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Retorna a lista de todas as Secretarias cadastradas.
        /// </summary>
        /// <returns>Lista de Secretarias.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var secretarias = await _secretariaService.GetAll();

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = secretarias;
                _response.Message = "Lista de secretarias obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter as secretarias";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
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
                    _response.Message = "Secretaria não encontrada";
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
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Exclui uma Secretaria existente.
        /// </summary>
        /// <param name="id">ID da Secretaria.</param>
        /// <returns>Resultado da exclusão.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var secretaria = await _secretariaService.GetById(id);
                if (secretaria == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Secretaria não encontrada";
                    return NotFound(_response);
                }

                await _secretariaService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Secretaria excluída com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir a secretaria";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Alterna a situação (Ativo/Inativo) de uma Secretaria.
        /// </summary>
        /// <param name="id">ID da Secretaria.</param>
        /// <returns>Resultado com a nova situação.</returns>
        /// <remarks>
        /// Regra:
        /// - Caso esteja ATIVO → vira INATIVO
        /// - Caso esteja INATIVO → vira ATIVO
        /// </remarks>
        [HttpPatch("{id}/alterar-situacao")]
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
                    _response.Message = "Secretaria não encontrada";
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
                _response.Message = $"Situação alterada para {secretaria.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação da secretaria";
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
