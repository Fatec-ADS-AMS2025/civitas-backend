using Civitas.WebAPI.DTOs;
using Civitas.WebAPI.Generic.Services;

namespace Civitas.WebAPI.Services.Interfaces;

/// <summary>
/// Interface de serviço específica para Secretaria
/// Herda operações CRUD básicas do serviço genérico
/// </summary>
public interface ISecretariaService : IGenericService<SecretariaDto, SecretariaCreateDto, SecretariaUpdateDto>
{
    // Adicione métodos específicos de Secretaria aqui, se necessário
}