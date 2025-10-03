using Civitas.WebAPI.Models;
using Civitas.WebAPI.Generic.Repositories;

namespace Civitas.WebAPI.Repositories.Interfaces;

/// <summary>
/// Interface de repositório específica para Secretaria
/// Herda operações CRUD básicas do repositório genérico
/// </summary>
public interface ISecretariaRepository : IGenericRepository<Secretaria>
{
    /// <summary>
    /// Busca secretaria por CNPJ
    /// </summary>
    Task<Secretaria?> GetByCnpjAsync(string cnpj);

    /// <summary>
    /// Busca secretaria por email
    /// </summary>
    Task<Secretaria?> GetByEmailAsync(string email);

    /// <summary>
    /// Verifica se CNPJ já existe (excluindo um ID específico)
    /// </summary>
    Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null);

    /// <summary>
    /// Verifica se email já existe (excluindo um ID específico)
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}