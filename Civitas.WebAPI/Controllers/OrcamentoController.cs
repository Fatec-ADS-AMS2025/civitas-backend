using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsÃ¡vel pelo gerenciamento dos OrÃ§amentos.
    /// </summary>
    /// <remarks>
    /// FunÃ§Ãµes:
    /// - Expor os endpoints CRUD de orÃ§amentos.
    /// - Padronizar as respostas utilizando o objeto <see cref="Response"/>.
    /// 
    /// DependÃªncias:
    /// - <see cref="IOrcamentoService"/>: Camada de serviÃ§o contendo as regras de negÃ³cio.
    /// </remarks>
    [Authorize]
    [Route("api/orcamentos")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly Response _response;

        /// <summary>
        /// Construtor responsÃ¡vel por inicializar o controller de OrÃ§amentos.
        /// </summary>
        /// <param name="orcamentoService">ServiÃ§o de regras de negÃ³cio de OrÃ§amentos.</param>
        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
            _response = new Response();
        }

        /// <summary>
        /// ObtÃ©m todos os orÃ§amentos cadastrados no sistema.
        /// </summary>
        /// <returns>Lista de orÃ§amentos e mensagem de sucesso.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var orcamentoDto = await _orcamentoService.GetPage(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "OrÃ§amentos listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// ObtÃ©m um orÃ§amento especÃ­fico pelo seu identificador.
        /// </summary>
        /// <param name="idOrcamento">Identificador Ãºnico do orÃ§amento.</param>
        /// <returns>OrÃ§amento correspondente ou mensagem de erro.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrcamentoById(int id)
        {
            var orcamentoDto = await _orcamentoService.GetById(id);
            if (orcamentoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum orÃ§amento encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "OrÃ§amento encontrado com sucesso";
            return Ok(_response);
        }

        /// <summary>
        /// Cadastra um novo orÃ§amento no sistema.
        /// </summary>
        /// <param name="orcamentoDTO">Dados do orÃ§amento a ser registrado.</param>
        /// <returns>Resultado da operaÃ§Ã£o, com mensagem de sucesso ou erro.</returns>
        /// <remarks>
        /// ValidaÃ§Ãµes executadas:
        /// - Ano do orÃ§amento deve ser maior que zero.
        /// - Valor total deve ser maior que zero.
        /// - A instituiÃ§Ã£o vinculada deve ser vÃ¡lida.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(OrcamentoDTO orcamentoDTO)
        {
            try
            {
                await _orcamentoService.Create(orcamentoDTO);
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "OrÃ§amento cadastrado com sucesso";

                return Ok(_response);
            }
            catch (OrcamentoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "NÃ£o foi possÃ­vel cadastrar o orÃ§amento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza os dados de um orÃ§amento existente.
        /// </summary>
        /// <param name="idOrcamento">Identificador do orÃ§amento.</param>
        /// <param name="orcamentoDTO">Dados atualizados do orÃ§amento.</param>
        /// <returns>Resultado da operaÃ§Ã£o.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, OrcamentoDTO orcamentoDTO)
        {
            try
            {
                await _orcamentoService.Update(orcamentoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "OrÃ§amento atualizado com sucesso";

                return Ok(_response);
            }
            catch (KeyNotFoundException)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "O orÃ§amento informado nÃ£o existe";
                return NotFound(_response);
            }
            catch (OrcamentoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do orÃ§amento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove um orÃ§amento caso nÃ£o possua despesas vinculadas.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orcamentoService.RemoverAsync(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "OrÃ§amento removido com sucesso";
                return Ok(_response);
            }
            catch (KeyNotFoundException)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = "O orÃ§amento informado nÃ£o existe";
                return NotFound(_response);
            }
            catch (OrcamentoValidationException ex)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = ex.Errors;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar remover o orÃ§amento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
        [HttpGet("excluidos")]
        public async Task<IActionResult> GetExcluidos([FromQuery] PaginationQuery paginationQuery)
        {
            var result = await _orcamentoService.GetPageExcluidos(paginationQuery);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = result;
            _response.Message = "Orcamentos excluidos listados com sucesso";

            return Ok(_response);
        }
        [HttpPatch("{id}/status-exclusao")]
        public async Task<IActionResult> ToggleStatusExclusao(int id)
        {
            try
            {
                var dto = await _orcamentoService.ToggleStatusExclusaoAsync(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = dto;
                _response.Message = "Orcamentos com status de exclusao alterado com sucesso";
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.Code = ResponseEnum.NOT_FOUND;
                _response.Data = null;
                _response.Message = ex.Message;
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar o status de exclusao";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponivel"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}


