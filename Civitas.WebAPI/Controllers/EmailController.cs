using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Endpoints de envio e consulta do histórico de e-mails.
    /// </summary>
    [Authorize]
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly Response _response;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
            _response = new Response();
        }

        /// <summary>
        /// Lista o histórico de e-mails enviados, com filtros opcionais.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EmailFiltroDto filtro)
        {
            try
            {
                var emails = await _emailService.ListarAsync(filtro);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = emails;
                _response.Message = "E-mails listados com sucesso";
                return Ok(_response);
            }
            catch (EmailValidationException ex)
            {
                return Invalid(ex);
            }
            catch (Exception)
            {
                return Unexpected("Ocorreu um erro ao listar os e-mails enviados.");
            }
        }

        /// <summary>
        /// Lista e-mails pelo destinatário informado.
        /// </summary>
        [HttpGet("destinatario/{email}")]
        public async Task<IActionResult> GetByDestinatario(string email)
        {
            try
            {
                var emails = await _emailService.ListarPorDestinatarioAsync(email);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = emails;
                _response.Message = "E-mails do destinatário listados com sucesso";
                return Ok(_response);
            }
            catch (EmailValidationException ex)
            {
                return Invalid(ex);
            }
            catch (Exception)
            {
                return Unexpected("Ocorreu um erro ao consultar e-mails por destinatário.");
            }
        }

        /// <summary>
        /// Lista e-mails pelo status (nome ou número do enum).
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var emails = await _emailService.ListarPorStatusAsync(status);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = emails;
                _response.Message = "E-mails filtrados por status com sucesso";
                return Ok(_response);
            }
            catch (EmailValidationException ex)
            {
                return Invalid(ex);
            }
            catch (Exception)
            {
                return Unexpected("Ocorreu um erro ao consultar e-mails por status.");
            }
        }

        /// <summary>
        /// Busca um e-mail enviado pelo identificador.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var email = await _emailService.ObterPorIdAsync(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = email;
                _response.Message = "E-mail encontrado com sucesso";
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (Exception)
            {
                return Unexpected("Ocorreu um erro ao consultar o e-mail enviado.");
            }
        }

        /// <summary>
        /// Valida, envia e registra um e-mail.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post(EmailEnvioDto emailEnvioDto)
        {
            try
            {
                var email = await _emailService.EnviarAsync(emailEnvioDto);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = email;
                _response.Message = "E-mail enviado com sucesso";
                return Ok(_response);
            }
            catch (EmailValidationException ex)
            {
                return Invalid(ex);
            }
            catch (EmailSendException ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Data = ex.Email;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status502BadGateway, _response);
            }
            catch (Exception)
            {
                return Unexpected("Ocorreu um erro ao enviar o e-mail.");
            }
        }

        private BadRequestObjectResult Invalid(EmailValidationException ex)
        {
            _response.Code = ResponseEnum.INVALID;
            _response.Data = ex.Errors;
            _response.Message = ex.Message;
            return BadRequest(_response);
        }

        private ObjectResult Unexpected(string message)
        {
            _response.Code = ResponseEnum.ERROR;
            _response.Data = null;
            _response.Message = message;
            return StatusCode(StatusCodes.Status500InternalServerError, _response);
        }
    }
}
