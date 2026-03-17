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
    /// Controlador responsável pelo gerenciamento de Usuários dentro do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Fornece endpoints de CRUD para entidades de usuário.
    /// - Permite buscas específicas, como consulta por CPF.
    ///
    /// Dependências:
    /// - <see cref="IUsuarioService"/>: Camada de serviço que encapsula regras de negócio.
    /// - <see cref="Response"/>: Objeto padrão utilizado para retorno de respostas.
    /// </remarks>
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controlador de usuários.
        /// </summary>
        /// <param name="usuarioService">Serviço responsável pela lógica de usuários.</param>
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _response = new Response();
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados.
        /// </summary>
        /// <returns>Lista de usuários encapsulada em um objeto de resposta.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var usuarioDTO = await _usuarioService.GetPage(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = usuarioDTO;
            _response.Message = "Usuários listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Busca usuários pelo CPF informado.
        /// </summary>
        /// <param name="cpf">CPF do usuário desejado.</param>
        /// <returns>Lista de usuários que possuem o CPF informado.</returns>
        /// <remarks>
        /// Observações:
        /// - Pode retornar múltiplos usuários, dependendo da modelagem.
        /// - Retorna NotFound caso nenhum usuário seja encontrado.
        /// </remarks>
        [HttpGet("cpf")]
        public async Task<IActionResult> GetUsuarioByCpf([FromQuery] string cpf)
        {
            var usuarioDto = await _usuarioService.GetUsuarioByCpf(cpf);
            if (usuarioDto is null || !usuarioDto.Any())
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum usuário encontrado com esse cpf";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = usuarioDto;
            _response.Message = "Usuários listados com sucesso";
            return Ok(_response);
        }

        /// <summary>
        /// Consulta um usuário pelo ID.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        /// <returns>DTO do usuário, caso encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var usuarioDto = await _usuarioService.GetById(id);
            if (usuarioDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum usuário encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = usuarioDto;
            _response.Message = "Usuário listado com sucesso";
            return Ok(_response);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="usuarioDTO">Dados do usuário a ser criado.</param>
        /// <returns>Objeto de resposta contendo o usuário criado.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                usuarioDTO.Id = 0;
                await _usuarioService.Create(usuarioDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = usuarioDTO;
                _response.Message = "Usuário cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <param name="usuarioDTO">Novos dados do usuário.</param>
        /// <returns>Objeto de resposta com o usuário atualizado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingUsuarioDTO = await _usuarioService.GetById(id);
                if (existingUsuarioDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O usuário informado não existe";
                    return NotFound(_response);
                }

                await _usuarioService.Update(usuarioDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = usuarioDTO;
                _response.Message = "Usuário atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove um usuário pelo ID.
        /// </summary>
        /// <param name="id">Identificador do usuário a ser removido.</param>
        /// <returns>Resultado da operação de remoção.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingUsuarioDTO = await _usuarioService.GetById(id);
                if (existingUsuarioDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O usuário informado não existe";
                    return NotFound(_response);
                }

                await _usuarioService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Usuário removido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar remover o usuário";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Alterna a situação (Ativo/Inativo) de um usuário.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Status atualizado da situação do usuário.</returns>
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetById(id);
                if (usuario == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Usuário não encontrado";
                    return NotFound(_response);
                }

                // Alterna o valor atual do enum
                usuario.Situacao = usuario.Situacao == Situacao.ATIVO
                    ? Situacao.INATIVO
                    : Situacao.ATIVO;

                await _usuarioService.Update(usuario, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    usuario.Id,
                    Situacao = usuario.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {usuario.Situacao} com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do usuário";
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
