using Civitas.WebAPI.DTOs;

namespace Civitas.WebAPI.Services.Interfaces;

public interface ISecretariaService
{
    Task<IEnumerable<SecretariaDto>> GetAllAsync();
    Task<IEnumerable<SecretariaDto>> GetActiveAsync();
    Task<SecretariaDto?> GetByIdAsync(int id);
    Task<SecretariaDto> CreateAsync(SecretariaCreateDto createDto);
    Task<SecretariaDto> UpdateAsync(int id, SecretariaUpdateDto updateDto);
    Task<bool> ActivateAsync(int id);
    Task<bool> DeactivateAsync(int id);
}