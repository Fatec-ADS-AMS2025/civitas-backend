using Microsoft.AspNetCore.Mvc;
using Civitas.WebAPI.DTOs;
using Civitas.WebAPI.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Civitas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Operações relacionadas ao gerenciamento de secretarias")]
public class SecretariaController : ControllerBase
{
    private readonly ISecretariaService _service;

    public SecretariaController(ISecretariaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Listar todas as secretarias
    /// </summary>
    /// <returns>Lista com todas as secretarias cadastradas</returns>
    /// <response code="200">Lista de secretarias retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar todas as secretarias",
        Description = "Retorna uma lista com todas as secretarias cadastradas no sistema, incluindo ativas e inativas"
    )]
    [SwaggerResponse(200, "Lista de secretarias retornada com sucesso", typeof(IEnumerable<SecretariaDto>))]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<IEnumerable<SecretariaDto>>> GetAll()
    {
        try
        {
            var secretarias = await _service.GetAllAsync();
            return Ok(secretarias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Listar apenas secretarias ativas
    /// </summary>
    /// <returns>Lista com secretarias ativas</returns>
    /// <response code="200">Lista de secretarias ativas retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("active")]
    [SwaggerOperation(
        Summary = "Listar secretarias ativas",
        Description = "Retorna uma lista com apenas as secretarias que estão ativas no sistema"
    )]
    [SwaggerResponse(200, "Lista de secretarias ativas retornada com sucesso", typeof(IEnumerable<SecretariaDto>))]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<IEnumerable<SecretariaDto>>> GetActive()
    {
        try
        {
            var secretarias = await _service.GetActiveAsync();
            return Ok(secretarias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obter secretaria por ID
    /// </summary>
    /// <param name="id">ID da secretaria</param>
    /// <returns>Dados da secretaria</returns>
    /// <response code="200">Secretaria encontrada</response>
    /// <response code="404">Secretaria não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Buscar secretaria por ID",
        Description = "Retorna os dados de uma secretaria específica baseado no ID fornecido"
    )]
    [SwaggerResponse(200, "Secretaria encontrada", typeof(SecretariaDto))]
    [SwaggerResponse(404, "Secretaria não encontrada")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<SecretariaDto>> GetById(int id)
    {
        try
        {
            var secretaria = await _service.GetByIdAsync(id);
            if (secretaria == null)
            {
                return NotFound(new { message = "Secretaria não encontrada" });
            }

            return Ok(secretaria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Cadastrar nova secretaria
    /// </summary>
    /// <param name="createDto">Dados da secretaria a ser criada</param>
    /// <returns>Dados da secretaria criada</returns>
    /// <response code="201">Secretaria criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="409">Conflito - CNPJ ou email já cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Criar nova secretaria",
        Description = "Cadastra uma nova secretaria no sistema com os dados fornecidos. O CNPJ e email devem ser únicos."
    )]
    [SwaggerResponse(201, "Secretaria criada com sucesso", typeof(SecretariaDto))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(409, "Conflito - CNPJ ou email já cadastrado")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<SecretariaDto>> Create([FromBody] SecretariaCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var secretaria = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = secretaria.IdSecretaria }, secretaria);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Alterar secretaria existente
    /// </summary>
    /// <param name="id">ID da secretaria a ser alterada</param>
    /// <param name="updateDto">Novos dados da secretaria</param>
    /// <returns>Dados da secretaria atualizada</returns>
    /// <response code="200">Secretaria atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Secretaria não encontrada</response>
    /// <response code="409">Conflito - CNPJ ou email já cadastrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Atualizar secretaria",
        Description = "Atualiza os dados de uma secretaria existente. O CNPJ e email devem continuar únicos."
    )]
    [SwaggerResponse(200, "Secretaria atualizada com sucesso", typeof(SecretariaDto))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(404, "Secretaria não encontrada")]
    [SwaggerResponse(409, "Conflito - CNPJ ou email já cadastrado")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult<SecretariaDto>> Update(int id, [FromBody] SecretariaUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var secretaria = await _service.UpdateAsync(id, updateDto);
            return Ok(secretaria);
        }
        catch (ArgumentException)
        {
            return NotFound(new { message = "Secretaria não encontrada" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Ativar secretaria
    /// </summary>
    /// <param name="id">ID da secretaria a ser ativada</param>
    /// <returns>Mensagem de confirmação</returns>
    /// <response code="200">Secretaria ativada com sucesso</response>
    /// <response code="404">Secretaria não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:int}/activate")]
    [SwaggerOperation(
        Summary = "Ativar secretaria",
        Description = "Ativa uma secretaria que estava inativa, tornando-a disponível para uso no sistema"
    )]
    [SwaggerResponse(200, "Secretaria ativada com sucesso")]
    [SwaggerResponse(404, "Secretaria não encontrada")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult> Activate(int id)
    {
        try
        {
            var result = await _service.ActivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Secretaria não encontrada" });
            }

            return Ok(new { message = "Secretaria ativada com sucesso" });
        }
        catch (ArgumentException)
        {
            return NotFound(new { message = "Secretaria não encontrada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Inativar secretaria
    /// </summary>
    /// <param name="id">ID da secretaria a ser inativada</param>
    /// <returns>Mensagem de confirmação</returns>
    /// <response code="200">Secretaria inativada com sucesso</response>
    /// <response code="404">Secretaria não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:int}/deactivate")]
    [SwaggerOperation(
        Summary = "Inativar secretaria",
        Description = "Inativa uma secretaria, removendo-a da lista de secretarias ativas mas mantendo o registro histórico"
    )]
    [SwaggerResponse(200, "Secretaria inativada com sucesso")]
    [SwaggerResponse(404, "Secretaria não encontrada")]
    [SwaggerResponse(500, "Erro interno do servidor")]
    public async Task<ActionResult> Deactivate(int id)
    {
        try
        {
            var result = await _service.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Secretaria não encontrada" });
            }

            return Ok(new { message = "Secretaria inativada com sucesso" });
        }
        catch (ArgumentException)
        {
            return NotFound(new { message = "Secretaria não encontrada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }
}