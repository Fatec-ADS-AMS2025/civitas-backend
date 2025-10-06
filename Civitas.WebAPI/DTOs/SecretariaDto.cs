using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Civitas.WebAPI.DTOs;

/// <summary>
/// DTO para criação de uma nova secretaria
/// </summary>
public class SecretariaCreateDto
{
    /// <summary>
    /// Descrição da secretaria
    /// </summary>
    /// <example>Secretaria Municipal de Educação</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    /// <summary>
    /// CNPJ da secretaria (formato: XX.XXX.XXX/XXXX-XX)
    /// </summary>
    /// <example>12.345.678/0001-90</example>
    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter entre 14 e 18 caracteres")]
    public string Cnpj { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome da secretaria
    /// </summary>
    /// <example>Secretaria de Educação</example>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(255, ErrorMessage = "O nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Endereço - logradouro
    /// </summary>
    /// <example>Rua das Flores, 123</example>
    [Required(ErrorMessage = "O logradouro é obrigatório")]
    [StringLength(255, ErrorMessage = "O logradouro deve ter no máximo 255 caracteres")]
    public string Logradouro { get; set; } = string.Empty;
    
    /// <summary>
    /// Número do endereço
    /// </summary>
    /// <example>123</example>
    [Required(ErrorMessage = "O número é obrigatório")]
    [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
    public string Numero { get; set; } = string.Empty;
    
    /// <summary>
    /// Bairro
    /// </summary>
    /// <example>Centro</example>
    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(255, ErrorMessage = "O bairro deve ter no máximo 255 caracteres")]
    public string Bairro { get; set; } = string.Empty;
    
    /// <summary>
    /// Nomenclatura especial da secretaria (sigla)
    /// </summary>
    /// <example>SEMED</example>
    [StringLength(255, ErrorMessage = "A nomenclatura especial deve ter no máximo 255 caracteres")]
    public string NomenclaturaSecial { get; set; } = string.Empty;
    
    /// <summary>
    /// Email de contato da secretaria
    /// </summary>
    /// <example>educacao@cidade.gov.br</example>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email deve ter um formato válido")]
    [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefone de contato
    /// </summary>
    /// <example>(11) 3333-4444</example>
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres")]
    public string Telefone { get; set; } = string.Empty;
    
    /// <summary>
    /// Cidade onde a secretaria está localizada
    /// </summary>
    /// <example>São Paulo</example>
    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(255, ErrorMessage = "A cidade deve ter no máximo 255 caracteres")]
    public string Cidade { get; set; } = string.Empty;
    
    /// <summary>
    /// Estado (UF) - 2 caracteres
    /// </summary>
    /// <example>SP</example>
    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter exatamente 2 caracteres")]
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de uma secretaria existente
/// </summary>
public class SecretariaUpdateDto
{
    /// <summary>
    /// Descrição da secretaria
    /// </summary>
    /// <example>Secretaria Municipal de Educação - Atualizada</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    /// <summary>
    /// CNPJ da secretaria (formato: XX.XXX.XXX/XXXX-XX)
    /// </summary>
    /// <example>12.345.678/0001-90</example>
    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter entre 14 e 18 caracteres")]
    public string Cnpj { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome da secretaria
    /// </summary>
    /// <example>Secretaria de Educação Municipal</example>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(255, ErrorMessage = "O nome deve ter no máximo 255 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Endereço - logradouro
    /// </summary>
    /// <example>Rua das Flores, 456</example>
    [Required(ErrorMessage = "O logradouro é obrigatório")]
    [StringLength(255, ErrorMessage = "O logradouro deve ter no máximo 255 caracteres")]
    public string Logradouro { get; set; } = string.Empty;
    
    /// <summary>
    /// Número do endereço
    /// </summary>
    /// <example>456</example>
    [Required(ErrorMessage = "O número é obrigatório")]
    [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
    public string Numero { get; set; } = string.Empty;
    
    /// <summary>
    /// Bairro
    /// </summary>
    /// <example>Centro</example>
    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(255, ErrorMessage = "O bairro deve ter no máximo 255 caracteres")]
    public string Bairro { get; set; } = string.Empty;
    
    /// <summary>
    /// Nomenclatura especial da secretaria (sigla)
    /// </summary>
    /// <example>SEMED</example>
    [StringLength(255, ErrorMessage = "A nomenclatura especial deve ter no máximo 255 caracteres")]
    public string NomenclaturaSecial { get; set; } = string.Empty;
    
    /// <summary>
    /// Email de contato da secretaria
    /// </summary>
    /// <example>educacao@cidade.gov.br</example>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email deve ter um formato válido")]
    [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefone de contato
    /// </summary>
    /// <example>(11) 3333-5555</example>
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres")]
    public string Telefone { get; set; } = string.Empty;
    
    /// <summary>
    /// Cidade onde a secretaria está localizada
    /// </summary>
    /// <example>São Paulo</example>
    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(255, ErrorMessage = "A cidade deve ter no máximo 255 caracteres")]
    public string Cidade { get; set; } = string.Empty;
    
    /// <summary>
    /// Estado (UF) - 2 caracteres
    /// </summary>
    /// <example>SP</example>
    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter exatamente 2 caracteres")]
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta com dados completos da secretaria
/// </summary>
public class SecretariaDto
{
    /// <summary>
    /// ID único da secretaria
    /// </summary>
    /// <example>1</example>
    public int IdSecretaria { get; set; }
    
    /// <summary>
    /// Descrição da secretaria
    /// </summary>
    /// <example>Secretaria Municipal de Educação</example>
    public string Descricao { get; set; } = string.Empty;
    
    /// <summary>
    /// CNPJ da secretaria
    /// </summary>
    /// <example>12.345.678/0001-90</example>
    public string Cnpj { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome da secretaria
    /// </summary>
    /// <example>Secretaria de Educação</example>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>
    /// Endereço - logradouro
    /// </summary>
    /// <example>Rua das Flores, 123</example>
    public string Logradouro { get; set; } = string.Empty;
    
    /// <summary>
    /// Número do endereço
    /// </summary>
    /// <example>123</example>
    public string Numero { get; set; } = string.Empty;
    
    /// <summary>
    /// Bairro
    /// </summary>
    /// <example>Centro</example>
    public string Bairro { get; set; } = string.Empty;
    
    /// <summary>
    /// Nomenclatura especial da secretaria
    /// </summary>
    /// <example>SEMED</example>
    public string NomenclaturaSecial { get; set; } = string.Empty;
    
    /// <summary>
    /// Email de contato da secretaria
    /// </summary>
    /// <example>educacao@cidade.gov.br</example>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefone de contato
    /// </summary>
    /// <example>(11) 3333-4444</example>
    public string Telefone { get; set; } = string.Empty;
    
    /// <summary>
    /// Cidade onde a secretaria está localizada
    /// </summary>
    /// <example>São Paulo</example>
    public string Cidade { get; set; } = string.Empty;
    
    /// <summary>
    /// Estado (UF)
    /// </summary>
    /// <example>SP</example>
    public string Estado { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica se a secretaria está ativa
    /// </summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
    
    /// <summary>
    /// Data de criação do registro
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime DataCriacao { get; set; }
    
    /// <summary>
    /// Data da última alteração do registro
    /// </summary>
    /// <example>2024-01-16T14:20:00Z</example>
    public DateTime? DataAlteracao { get; set; }
}