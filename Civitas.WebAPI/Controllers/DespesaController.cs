using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de despesas cadastradas no sistema.
    /// </summary>
    [Authorize]
    [Route("api/despesas")]
    [ApiController]
    public class DespesaController : ControllerBase
    {
        private readonly IDespesaService _despesaService;
        private readonly Response _response;

        public DespesaController(IDespesaService despesaService)
        {
            _despesaService = despesaService;
            _response = new Response();
        }

        /// <summary>
        /// Retorna todas as despesas cadastradas.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await _despesaService.AtualizarDespesasAtrasadasAsync();

            var despesas = await _despesaService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas listadas com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna apenas as despesas pagas.
        /// </summary>
        [HttpGet("pagas")]
        public async Task<IActionResult> GetPagas()
        {
            var despesas = await _despesaService.GetByStatusAsync(Status.PAGA);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas pagas listadas com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna apenas as despesas atrasadas.
        /// </summary>
        [HttpGet("atrasadas")]
        public async Task<IActionResult> GetAtrasadas()
        {
            var despesas = await _despesaService.GetByStatusAsync(Status.ATRASADO);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas atrasadas listadas com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna despesas pelo número do documento.
        /// </summary>
        [HttpGet("numero-documento/{numeroDocumento}")]
        public async Task<IActionResult> GetByNumeroDocumento(string numeroDocumento)
        {
            var despesas = await _despesaService.GetByNumeroDocumentoAsync(numeroDocumento);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas listadas por número do documento com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna despesas pelo código.
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var despesas = await _despesaService.GetByCodigoAsync(codigo);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas listadas por código com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna despesas vinculadas a uma unidade consumidora.
        /// </summary>
        [HttpGet("unidade-consumidora/{idUnidadeConsumidora}")]
        public async Task<IActionResult> GetByUnidadeConsumidora(int idUnidadeConsumidora)
        {
            var despesas = await _despesaService.GetByUnidadeConsumidoraAsync(idUnidadeConsumidora);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas da unidade consumidora listadas com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna despesas cadastradas por um usuário.
        /// </summary>
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario)
        {
            var despesas = await _despesaService.GetByUsuarioAsync(idUsuario);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas do usuário listadas com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna despesas filtradas pelo status.
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(Status status)
        {
            if (!StatusValido(status))
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Status inválido";
                return BadRequest(_response);
            }

            var despesas = await _despesaService.GetByStatusAsync(status);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesas;
            _response.Message = "Despesas listadas por status com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Retorna uma despesa específica pelo seu ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var despesa = await _despesaService.GetById(id);
            if (despesa is null)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "Despesa não encontrada";
                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = despesa;
            _response.Message = "Despesa encontrada com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Cria uma nova despesa no sistema.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] DespesaDTO despesaDTO)
        {
            if (despesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                await _despesaService.ValidarCadastroAsync(despesaDTO);

                despesaDTO.Id = 0;

                await _despesaService.Create(despesaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = despesaDTO;
                _response.Message = "Despesa cadastrada com sucesso";

                return Ok(_response);
            }
            catch (DespesaValidationException ex)
            {
                _response.Code = ex.Errors.Any(e => e.Contains("documento com o mesmo conteúdo"))
                    ? ResponseEnum.CONFLICT
                    : ResponseEnum.INVALID;

                _response.Message = ex.Message;
                _response.Data = ex.Errors;

                return _response.Code == ResponseEnum.CONFLICT
                    ? Conflict(_response)
                    : BadRequest(_response);
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
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar a despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza uma despesa existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] DespesaDTO despesaDTO)
        {
            try
            {
                await _despesaService.Update(despesaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    despesaDTO.Id,
                    despesaDTO.NumeroDocumento,
                    despesaDTO.NomeDocumento,
                    despesaDTO.Codigo,
                    despesaDTO.DataEmissao,
                    despesaDTO.DataVencimento,
                    despesaDTO.ValorPrevisto,
                    despesaDTO.ValorPago,
                    despesaDTO.ConsumoPrevisto,
                    despesaDTO.ConsumoReal,
                    despesaDTO.Status,
                    despesaDTO.IdUsuario,
                    despesaDTO.IdUnidadeConsumidora,
                    despesaDTO.HashDocumento
                };
                _response.Message = "Despesa atualizada com sucesso";

                return Ok(_response);
            }
            catch (DespesaValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = ex.Message;
                _response.Data = ex.Errors;
                
                // Verificar se é erro de documento duplicado
                if (ex.Errors.Any(e => e.Contains("documento com o mesmo conteúdo")))
                {
                    _response.Code = ResponseEnum.CONFLICT;
                    return Conflict(_response);
                }

                return BadRequest(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Message = ex.Message;
                _response.Data = null;
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados da despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Altera o status financeiro da despesa.
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] Status novoStatus)
        {
            try
            {
                var despesa = await _despesaService.AlterarStatusAsync(id, novoStatus);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    despesa.Id,
                    StatusAtual = despesa.Status.ToString()
                };
                _response.Message = $"Status alterado para '{despesa.Status}' com sucesso";

                return Ok(_response);
            }
            catch (DespesaValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Message = ex.Message;
                _response.Data = ex.Errors;
                return BadRequest(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Message = ex.Message;
                _response.Data = null;
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar o status da despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        private static bool StatusValido(Status status)
        {
            return status is Status.A_PAGAR or Status.PAGA or Status.ATRASADO;
        }
    }
}
