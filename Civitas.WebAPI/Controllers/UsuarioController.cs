using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly Response _response;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _response = new Response();
        }

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
                // Zeramos o id antes de cadastrar para que o banco gere automaticamente
                // e evite conflito com ids existentes
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

        
    }
}
