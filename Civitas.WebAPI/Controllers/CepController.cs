using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pela consulta genérica de endereços por CEP.
    /// </summary>
    /// <remarks>
    /// Este endpoint pode ser reutilizado por qualquer formulário do sistema que precise
    /// preencher logradouro, bairro, cidade e estado a partir de um CEP válido.
    /// </remarks>
    [Authorize]
    [Route("api/cep")]
    [ApiController]
    public class CepController : ControllerBase
    {
        private readonly ICepService _cepService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controller de consulta de CEP.
        /// </summary>
        /// <param name="cepService">Serviço responsável pela integração com a ViaCEP.</param>
        public CepController(ICepService cepService)
        {
            _cepService = cepService;
            _response = new Response();
        }

        /// <summary>
        /// Consulta um endereço a partir do CEP informado.
        /// </summary>
        /// <param name="cep">CEP com ou sem máscara.</param>
        /// <returns>Envelope padrão com os dados do endereço consultado.</returns>
        /// <remarks>
        /// Retorna:
        /// - 200 quando o endereço é encontrado
        /// - 400 quando o CEP é inválido
        /// - 404 quando a ViaCEP informa que o CEP não existe
        /// - 500 quando ocorre falha na integração externa
        /// </remarks>
        [HttpGet("{cep}")]
        public async Task<IActionResult> GetByCep(string cep)
        {
            try
            {
                var endereco = await _cepService.BuscarEnderecoPorCepAsync(cep);

                if (endereco is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "CEP não encontrado";
                    return NotFound(_response);
                }

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = endereco;
                _response.Message = "Endereço encontrado com sucesso";
                return Ok(_response);
            }
            catch (ArgumentException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            catch (CepServiceException)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Data = null;
                _response.Message = "Erro ao consultar CEP";
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            catch (Exception)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Data = null;
                _response.Message = "Erro ao consultar CEP";
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
