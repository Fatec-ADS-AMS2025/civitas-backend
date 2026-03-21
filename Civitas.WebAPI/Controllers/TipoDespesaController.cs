using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos Tipos de Despesa do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Disponibilizar operações de CRUD para tipos de despesa.
    /// - Controlar ativação e desativação considerando regras de dependência.
    ///
    /// Dependências:
    /// - <see cref="ITipoDespesaService"/>: Camada de serviços que contém regras de negócio.
    /// </remarks>
    [Route("api/tipo-despesa")]
    [ApiController]
    public class TipoDespesaController : ControllerBase
    {
        private readonly ITipoDespesaService _tipoDespesaService;
        private readonly Response _response;

        /// <summary>
        /// Inicializa o controlador de Tipos de Despesa.
        /// </summary>
        /// <param name="tipoDespesaService">Serviço responsável pelo gerenciamento de tipos de despesa.</param>
        public TipoDespesaController(ITipoDespesaService tipoDespesaService)
        {
            _tipoDespesaService = tipoDespesaService;
            _response = new Response();
        }

        /// <summary>
        /// Lista todos os tipos de despesa cadastrados.
        /// </summary>
        /// <returns>Lista de tipos de despesa.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var instituicaoDto = await _tipoDespesaService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.ATIVO);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = instituicaoDto;
            _response.Message = "Tipos de despesa ativos listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Lista todos os tipos de despesa inativos.
        /// </summary>
        /// <returns>Lista paginada de tipos de despesa inativos.</returns>
        [HttpGet("inativos")]
        public async Task<IActionResult> GetInactive([FromQuery] PaginationQuery paginationQuery)
        {
            var tipoDespesaDto = await _tipoDespesaService.GetPageByEnumValue(paginationQuery, "Situacao", Situacao.INATIVO);

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoDespesaDto;
            _response.Message = "Tipos de despesa inativos listados com sucesso";

            return Ok(_response);
        }

        /// <summary>
        /// Obtém um tipo de despesa específico pelo ID.
        /// </summary>
        /// <param name="id">Identificador único do tipo de despesa.</param>
        /// <returns>Dados do tipo de despesa correspondente.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoInstituicaoById(int id)
        {
            var tipoInstituicaoDto = await _tipoDespesaService.GetById(id);
            if (tipoInstituicaoDto is null)
            {
                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = null;
                _response.Message = "Nenhum tipo de despesa encontrado";

                return NotFound(_response);
            }

            _response.Code = ResponseEnum.SUCCESS;
            _response.Data = tipoInstituicaoDto;
            _response.Message = "Tipos de despesa listados com sucesso";
            return Ok(_response);
        }

        /// <summary>
        /// Cadastra um novo tipo de despesa.
        /// </summary>
        /// <param name="tipoDespesaDTO">Objeto contendo os dados do tipo de despesa.</param>
        /// <returns>Retorna o tipo de despesa criado.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TipoDespesaDTO tipoDespesaDTO)
        {
            if (tipoDespesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                tipoDespesaDTO.Id = 0;
                await _tipoDespesaService.Create(tipoDespesaDTO);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoDespesaDTO;
                _response.Message = "Tipo de despesa cadastrado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Não foi possível cadastrar o tipo de despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Atualiza os dados de um tipo de despesa existente.
        /// </summary>
        /// <param name="id">ID do tipo de despesa a ser alterado.</param>
        /// <param name="tipoDespesaDTO">Objeto com os novos dados.</param>
        /// <returns>Retorna o tipo de despesa atualizado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TipoDespesaDTO tipoDespesaDTO)
        {
            if (tipoDespesaDTO is null)
            {
                _response.Code = ResponseEnum.INVALID;
                _response.Data = null;
                _response.Message = "Dados inválidos";

                return BadRequest(_response);
            }

            try
            {
                var existingTipoDespesaDTO = await _tipoDespesaService.GetById(id);
                if (existingTipoDespesaDTO is null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "O tipo de despesa informado não existe";
                    return NotFound(_response);
                }

                await _tipoDespesaService.Update(tipoDespesaDTO, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = tipoDespesaDTO;
                _response.Message = "O tipo de despesa foi atualizado com sucesso";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao tentar atualizar os dados do tipo de despesa";
                _response.Data = new
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace ?? "Sem stack trace disponível"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        /// <summary>
        /// Alterna o status (Ativo/Inativo) de um tipo de despesa.
        /// </summary>
        /// <param name="id">ID do tipo de despesa.</param>
        /// <returns>Status atualizado.</returns>
        /// <remarks>
        /// Regras:
        /// - Não permite desativar caso existam unidades de medida ativas vinculadas.
        /// - Ativação é sempre permitida.
        /// </remarks>
        [HttpPatch("situacao/{id}")]
        public async Task<IActionResult> AlterarSituacao(int id)
        {
            try
            {
                var tipoDespesa = await _tipoDespesaService.GetById(id);
                if (tipoDespesa == null)
                {
                    _response.Code = ResponseEnum.NOT_FOUND;
                    _response.Data = null;
                    _response.Message = "Tipo de instituição não encontrado.";
                    return NotFound(_response);
                }

                if (tipoDespesa.Situacao == Situacao.ATIVO)
                {
                    var possuiAtivas = await _tipoDespesaService.ExisteUnidadesDeMedidaAtivas(id);
                    if (possuiAtivas)
                    {
                        _response.Code = ResponseEnum.INVALID;
                        _response.Data = null;
                        _response.Message = "Não é possível desativar este tipo de despesa, pois existem unidades de medida ativas vinculadas.";
                        return BadRequest(_response);
                    }

                    tipoDespesa.Situacao = Situacao.INATIVO;
                }
                else
                {
                    tipoDespesa.Situacao = Situacao.ATIVO;
                }

                await _tipoDespesaService.Update(tipoDespesa, id);

                _response.Code = ResponseEnum.SUCCESS;
                _response.Data = new
                {
                    tipoDespesa.Id,
                    Situacao = tipoDespesa.Situacao.ToString()
                };
                _response.Message = $"Situação alterada para {tipoDespesa.Situacao} com sucesso.";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Code = ResponseEnum.ERROR;
                _response.Message = "Ocorreu um erro ao alterar a situação do tipo de despesa.";
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

