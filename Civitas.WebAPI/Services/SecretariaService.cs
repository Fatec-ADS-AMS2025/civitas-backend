using AutoMapper;
using Civitas.WebAPI.DTOs;
using Civitas.WebAPI.Models;
using Civitas.WebAPI.Repositories.Interfaces;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services;

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
        var secretarias = await _repository.GetActiveAsync();
        return _mapper.Map<IEnumerable<SecretariaDto>>(secretarias);
    }

    public async Task<SecretariaDto?> GetByIdAsync(int id)
    {
        var secretaria = await _repository.GetByIdAsync(id);
        return secretaria != null ? _mapper.Map<SecretariaDto>(secretaria) : null;
    }

    public async Task<SecretariaDto> CreateAsync(SecretariaCreateDto createDto)
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

        var secretaria = _mapper.Map<Secretaria>(createDto);
        var createdSecretaria = await _repository.CreateAsync(secretaria);
        return _mapper.Map<SecretariaDto>(createdSecretaria);
    }

    public async Task<SecretariaDto> UpdateAsync(int id, SecretariaUpdateDto updateDto)
    {
        var existingSecretaria = await _repository.GetByIdAsync(id);
        if (existingSecretaria == null)
        {
            throw new ArgumentException("Secretaria não encontrada.", nameof(id));
        }

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

        _mapper.Map(updateDto, existingSecretaria);
        var updatedSecretaria = await _repository.UpdateAsync(existingSecretaria);
        return _mapper.Map<SecretariaDto>(updatedSecretaria);
    }

    public async Task<bool> ActivateAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            throw new ArgumentException("Secretaria não encontrada.", nameof(id));
        }

        return await _repository.ActivateAsync(id);
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            throw new ArgumentException("Secretaria não encontrada.", nameof(id));
        }

        return await _repository.DeactivateAsync(id);
    }
}