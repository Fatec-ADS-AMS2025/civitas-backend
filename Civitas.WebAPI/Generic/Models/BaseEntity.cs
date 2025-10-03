using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Generic.Models;

/// <summary>
/// Classe base para todas as entidades do sistema
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Indica se o registro está ativo no sistema
    /// </summary>
    public bool Ativo { get; set; } = true;
    
    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Data da última alteração do registro
    /// </summary>
    public DateTime? DataAlteracao { get; set; }
}
