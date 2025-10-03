using Microsoft.AspNetCore.Mvc;
using Civitas.WebAPI.Generic.Services;

namespace Civitas.WebAPI.Generic.Controllers;

/// <summary>
/// Controller base genérico com operações CRUD padrão
/// </summary>
/// <typeparam name="TDto">Tipo do DTO de resposta</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
[ApiController]
public abstract class GenericController<TDto, TCreateDto, TUpdateDto> : ControllerBase
{
    protected readonly IGenericService<TDto, TCreateDto, TUpdateDto> _service;

    protected GenericController(IGenericService<TDto, TCreateDto, TUpdateDto> service)
    {
        _service = service;
    }

    /// <summary>
    /// Listar todos os registros
    /// </summary>
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        try
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Listar apenas registros ativos
    /// </summary>
    [HttpGet("active")]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetActive()
    {
        try
        {
            var items = await _service.GetActiveAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obter registro por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public virtual async Task<ActionResult<TDto>> GetById(int id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Registro não encontrado" });
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Cadastrar novo registro
    /// </summary>
    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var item = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = GetItemId(item) }, item);
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
    /// Alterar registro existente
    /// </summary>
    [HttpPut("{id:int}")]
    public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var item = await _service.UpdateAsync(id, updateDto);
            return Ok(item);
        }
        catch (ArgumentException)
        {
            return NotFound(new { message = "Registro não encontrado" });
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
    /// Ativar registro
    /// </summary>
    [HttpPatch("{id:int}/activate")]
    public virtual async Task<ActionResult> Activate(int id)
    {
        try
        {
            var result = await _service.ActivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Registro não encontrado" });
            }

            return Ok(new { message = "Registro ativado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Inativar registro
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    public virtual async Task<ActionResult> Deactivate(int id)
    {
        try
        {
            var result = await _service.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Registro não encontrado" });
            }

            return Ok(new { message = "Registro inativado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Método abstrato para obter o ID do item (deve ser implementado nas classes derivadas)
    /// </summary>
    protected abstract int GetItemId(TDto item);
}
