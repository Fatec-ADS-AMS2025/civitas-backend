using AutoMapper;
using Civitas.WebAPI.DTOs;
using Civitas.WebAPI.Models;
using Civitas.WebAPI.Repositories.Interfaces;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services;

/// <summary>
/// Serviço específico para Secretaria
/// Implementa lógica de negócio e validações específicas
/// </summary>
public class SecretariaService : ISecretariaService
{
    private readonly ISecretariaRepository _repository;
    private readonly IMapper _mapper;

    public SecretariaService(ISecretariaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SecretariaDto>> GetAllAsync()
    {
        var secretarias = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SecretariaDto>>(secretarias);
    }

    public async Task<IEnumerable<SecretariaDto>> GetActiveAsync()
    {
        var secretarias = await _repository.GetWhereAsync(s => s.Ativo);
        return _mapper.Map<IEnumerable<SecretariaDto>>(secretarias);
    }

    public async Task<SecretariaDto?> GetByIdAsync(int id)
    {
        var secretaria = await _repository.GetByIdAsync(id);
        return secretaria != null ? _mapper.Map<SecretariaDto>(secretaria) : null;
    }

    public async Task<SecretariaDto> CreateAsync(SecretariaCreateDto createDto)
    {
        // Validações específicas de Secretaria
        await ValidateCreateAsync(createDto);

        var secretaria = _mapper.Map<Secretaria>(createDto);
        secretaria.DataCriacao = DateTime.UtcNow;
        secretaria.Ativo = true;
        
        var createdSecretaria = await _repository.AddAsync(secretaria);
        return _mapper.Map<SecretariaDto>(createdSecretaria);
    }

    public async Task<SecretariaDto> UpdateAsync(int id, SecretariaUpdateDto updateDto)
    {
        var existingSecretaria = await _repository.GetByIdAsync(id);
        if (existingSecretaria == null)
        {
            throw new ArgumentException("Secretaria não encontrada.", nameof(id));
        }

        // Validações específicas de Secretaria
        await ValidateUpdateAsync(id, updateDto);

        _mapper.Map(updateDto, existingSecretaria);
        existingSecretaria.DataAlteracao = DateTime.UtcNow;
        
        var updatedSecretaria = await _repository.UpdateAsync(existingSecretaria);
        return _mapper.Map<SecretariaDto>(updatedSecretaria);
    }

    public async Task<bool> ActivateAsync(int id)
    {
        var secretaria = await _repository.GetByIdAsync(id);
        if (secretaria == null)
            return false;

        secretaria.Ativo = true;
        secretaria.DataAlteracao = DateTime.UtcNow;
        await _repository.UpdateAsync(secretaria);
        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var secretaria = await _repository.GetByIdAsync(id);
        if (secretaria == null)
            return false;

        secretaria.Ativo = false;
        secretaria.DataAlteracao = DateTime.UtcNow;
        await _repository.UpdateAsync(secretaria);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Validações específicas na criação de Secretaria
    /// </summary>
    private async Task ValidateCreateAsync(SecretariaCreateDto createDto)
    {
        // Validar CNPJ único
        if (await _repository.CnpjExistsAsync(createDto.Cnpj))
        {
            throw new InvalidOperationException("Já existe uma secretaria com este CNPJ.");
        }

        // Validar Email único
        if (await _repository.EmailExistsAsync(createDto.Email))
        {
            throw new InvalidOperationException("Já existe uma secretaria com este email.");
        }
    }

    /// <summary>
    /// Validações específicas na atualização de Secretaria
    /// </summary>
    private async Task ValidateUpdateAsync(int id, SecretariaUpdateDto updateDto)
    {
        // Validar CNPJ único (excluindo o registro atual)
        if (await _repository.CnpjExistsAsync(updateDto.Cnpj, id))
        {
            throw new InvalidOperationException("Já existe uma secretaria com este CNPJ.");
        }

        // Validar Email único (excluindo o registro atual)
        if (await _repository.EmailExistsAsync(updateDto.Email, id))
        {
            throw new InvalidOperationException("Já existe uma secretaria com este email.");
        }
    }
}