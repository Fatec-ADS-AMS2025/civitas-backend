using AutoMapper;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    // Define a rota base da controller como: api/Fluxo
    [Route("api/fluxos")]
    [ApiController]
    public class FluxoController : Controller
    {
        // Serviï¿½o responsï¿½vel pela regra de negï¿½cio dos Fluxos
        private readonly IFluxoService _fluxoService;

        // Objeto padrï¿½o de resposta da API
        private readonly Response _response;

        // Construtor com injeï¿½ï¿½o de dependï¿½ncia do serviï¿½o de fluxo
        public FluxoController(IFluxoService fluxoService)
        {
            _fluxoService = fluxoService;

            // Inicializa o objeto de resposta
            _response = new Response();
        }

        // =========================
        //   ENDPOINT: POST /api/Fluxo
        //   Cria um novo fluxo
        // =========================
        [HttpPost]
        public async Task<IActionResult> Post(FluxoDTO fluxoDTO)
        {
            // Verifica se os dados recebidos sï¿½o vï¿½lidos
            if (fluxoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invï¿½lidos";

                return BadRequest(_response);
            }

            try
            {
                // Garante que o ID serï¿½ gerado pelo banco
                fluxoDTO.IdFluxo = 0;

                // Chama o service para criar o fluxo
                await _fluxoService.Create(fluxoDTO);

                // Prepara a resposta
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxoDTO;
                _response.Message = "Fluxo cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Resposta de erro com detalhes
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Nï¿½o foi possï¿½vel cadastrar o fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // =========================
        //   ENDPOINT: PUT /api/Fluxo/{id}
        //   Atualiza um fluxo existente
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FluxoDTO fluxoDTO)
        {
            // Verifica se os dados sï¿½o vï¿½lidos
            if (fluxoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invï¿½lidos";

                return BadRequest(_response);
            }

            try
            {
                // Busca o fluxo pelo ID
                var existingFluxoDTO = await _fluxoService.GetById(id);

                // Verifica se o fluxo existe
                if (existingFluxoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O fluxo informado nï¿½o existe";
                    return NotFound(_response);
                }

                // Atualiza o fluxo via service
                await _fluxoService.Update(fluxoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxoDTO;
                _response.Message = "Fluxo atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Retorno em caso de erro
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // =========================
        //   ENDPOINT: GET /api/Fluxo
        //   Retorna todos os fluxos
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            try
            {
                // ObtÃ©m todos os fluxos
                var fluxos = await _fluxoService.GetPage(paginationQuery);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxos;
                _response.Message = "Lista de fluxos obtida com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Retorno de erro
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter a lista de fluxos";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // =========================
        //   ENDPOINT: GET /api/Fluxo/{id}
        //   Retorna um fluxo especï¿½fico
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // Busca o fluxo pelo ID
                var fluxo = await _fluxoService.GetById(id);

                // Caso nï¿½o exista
                if (fluxo == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fluxo nï¿½o encontrado";
                    return NotFound(_response);
                }

                // Retorno sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = fluxo;
                _response.Message = "Fluxo obtido com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Retorno em caso de erro
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao obter o fluxo";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        // ==============================================
        //   ENDPOINT: PATCH /api/Fluxo/{id}/alterar-status
        //   Altera apenas o status de um fluxo existente
        // ==============================================
        [HttpPatch("status/{id}")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] Status novoStatus)
        {
            try
            {
                // Obtï¿½m o fluxo existente
                var fluxo = await _fluxoService.GetById(id);

                // Verifica se existe
                if (fluxo == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Fluxo nï¿½o encontrado";
                    return NotFound(_response);
                }

                // Atualiza somente o status
                fluxo.Status = novoStatus;

                // Chama o service para salvar a alteraï¿½ï¿½o
                await _fluxoService.Update(fluxo, id);

                // Retorno de sucesso
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    fluxo.IdFluxo,
                    StatusAtual = fluxo.Status.ToString()
                };
                _response.Message = $"Status alterado para '{fluxo.Status}' com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Caso ocorra erro
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar o status do fluxo";
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

