using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento dos Orçamentos.
    /// </summary>
    /// <remarks>
    /// Funções:
    /// - Expor os endpoints CRUD de orçamentos.
    /// - Padronizar as respostas utilizando o objeto <see cref="Response"/>.
    /// 
    /// Dependências:
    /// - <see cref="IOrcamentoService"/>: Camada de serviço contendo as regras de negócio.
    /// </remarks>
    [Route("api/orcamentos")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;
        private readonly Response _response;

        /// <summary>
        /// Construtor responsável por inicializar o controller de Orçamentos.
        /// </summary>
        /// <param name="orcamentoService">Serviço de regras de negócio de Orçamentos.</param>
        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
            _response = new Response();
        }

        /// <summary>
        /// Obtém todos os orçamentos cadastrados no sistema.
        /// </summary>
        /// <returns>Lista de orçamentos e mensagem de sucesso.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orcamentoDto = await _orcamentoService.GetAll();

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "Orçamentos listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Obtém um orçamento específico pelo seu identificador.
        /// </summary>
        /// <param name="idOrcamento">Identificador único do orçamento.</param>
        /// <returns>Orçamento correspondente ou mensagem de erro.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrcamentoById(int id)
        {
            var orcamentoDto = await _orcamentoService.GetById(id);
            if (orcamentoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum orçamento encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = orcamentoDto;
            _response.Message = "Orçamento encontrado com sucesso";
            return Ok(_response);
        }

        /// <summary>
        /// Cadastra um novo orçamento no sistema.
        /// </summary>
        /// <param name="orcamentoDTO">Dados do orçamento a ser registrado.</param>
        /// <returns>Resultado da operação, com mensagem de sucesso ou erro.</returns>
        /// <remarks>
        /// Validações executadas:
        /// - Ano do orçamento deve ser maior que zero.
        /// - Valor total deve ser maior que zero.
        /// - A instituição vinculada deve ser válida.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(OrcamentoDTO orcamentoDTO)
        {
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orçamento inválido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orçamento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Instituição inválida";
                    return BadRequest(_response);
                }

                orcamentoDTO.IdOrcamento = 0;
                await _orcamentoService.Create(orcamentoDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "Orçamento cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o orçamento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza os dados de um orçamento existente.
        /// </summary>
        /// <param name="idOrcamento">Identificador do orçamento.</param>
        /// <param name="orcamentoDTO">Dados atualizados do orçamento.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, OrcamentoDTO orcamentoDTO)
        {
            if (orcamentoDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingOrcamentoDTO = await _orcamentoService.GetById(id);
                if (existingOrcamentoDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O orçamento informado não existe";
                    return NotFound(_response);
                }

                if (orcamentoDTO.AnoOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Ano do orçamento inválido";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.ValorOrcamento <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Valor do orçamento deve ser maior que zero";
                    return BadRequest(_response);
                }

                if (orcamentoDTO.IdInstituicao <= 0)
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Data = null;
                    _response.Message = "Instituição inválida";
                    return BadRequest(_response);
                }

                await _orcamentoService.Update(orcamentoDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = orcamentoDTO;
                _response.Message = "Orçamento atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do orçamento";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "No stack trace available"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Remove um orçamento existente do sistema.
        /// </summary>
        /// <param name="idOrcamento">Identificador do orçamento a ser excluído.</param>
        /// <returns>Mensagem indicando o sucesso ou falha da operação.</returns>
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
                    _response.Message = "Orçamento não encontrado";
                    return NotFound(_response);
                }

                if (await _orcamentoService.ExisteDespesaVinculada(id))
                {
                    _response.Code = ResponseEnum.INVALID;
                    _response.Message = "Não é possível excluir o orçamento, pois há despesas vinculadas a ele.";
                    return BadRequest(_response);
                }

                await _orcamentoService.Remove(id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Orçamento excluído com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao excluir o orçamento";
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
