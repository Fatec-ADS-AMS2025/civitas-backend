using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
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
    [Route("api/orcamentos")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;
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
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invÃ¡lidos";

                return BadRequest(_response);
            }

            try
            {
                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orÃ§amento invÃ¡lido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orÃ§amento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "InstituiÃ§Ã£o invÃ¡lida";
                    return BadRequest(_response);
                }

                orcamentoDTO.IdOrcamento = 0;
                await _orcamentoService.Create(orcamentoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "OrÃ§amento cadastrado com sucesso";

                return Ok(_response);
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
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados invÃ¡lidos";

                return BadRequest(_response);
            }

            try
            {
                var existingOrcamentoDTO = await _orcamentoService.GetById(id);
                if (existingOrcamentoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O orÃ§amento informado nÃ£o existe";
                    return NotFound(_response);
                }

                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orÃ§amento invÃ¡lido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orÃ§amento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "InstituiÃ§Ã£o invÃ¡lida";
                    return BadRequest(_response);
                }

                await _orcamentoService.Update(orcamentoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "OrÃ§amento atualizado com sucesso";

                return Ok(_response);
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
        /// Remove um orÃ§amento existente do sistema.
        /// </summary>
        /// <param name="idOrcamento">Identificador do orÃ§amento a ser excluÃ­do.</param>
        /// <returns>Mensagem indicando o sucesso ou falha da operaÃ§Ã£o.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var orcamento = await _orcamentoService.GetById(id);
                if (orcamento == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "OrÃ§amento nÃ£o encontrado";
                    return NotFound(_response);
                }

                if (await _orcamentoService.ExisteDespesaVinculada(id))
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Message = "NÃ£o Ã© possÃ­vel excluir o orÃ§amento, pois hÃ¡ despesas vinculadas a ele.";
                    return BadRequest(_response);
                }

                await _orcamentoService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "OrÃ§amento excluÃ­do com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir o orÃ§amento";
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

