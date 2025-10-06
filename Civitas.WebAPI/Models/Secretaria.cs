using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Models;

public class Secretaria
{
    [Key]
    public int IdSecretaria { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Descricao { get; set; } = string.Empty;
    
    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Logradouro { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string Numero { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Bairro { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string NomenclaturaSecial { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(15)]
    public string Telefone { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Cidade { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2)]
    public string Estado { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataAlteracao { get; set; }
}