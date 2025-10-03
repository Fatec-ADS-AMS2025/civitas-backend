# ?? Generic - Classes Base Reutilizáveis

Este diretório contém classes genéricas que servem como base para implementação de CRUDs no sistema Civitas.

## ?? Estrutura

```
Generic/
??? Models/
?   ??? BaseEntity.cs          # Entidade base com campos comuns
??? Repositories/
?   ??? IGenericRepository.cs  # Interface genérica de repositório
?   ??? GenericRepository.cs   # Implementação genérica de repositório
??? Services/
?   ??? IGenericService.cs     # Interface genérica de serviço
?   ??? GenericService.cs      # Implementação genérica de serviço
??? Controllers/
    ??? GenericController.cs   # Controller base com endpoints CRUD
```

## ?? Objetivo

Reduzir código repetitivo e padronizar implementações de CRUD em todo o sistema.

## ?? Como Usar

### 1. Model (Entidade)

Herde de `BaseEntity` para ter campos comuns automaticamente:

```csharp
public class MinhaEntidade : BaseEntity
{
    // BaseEntity já inclui: Id, Ativo, DataCriacao, DataAlteracao
    
    public string Nome { get; set; }
    public string Descricao { get; set; }
}
```

### 2. Repository

#### Interface
```csharp
public interface IMinhaEntidadeRepository : IGenericRepository<MinhaEntidade>
{
    // Adicione métodos específicos aqui, se necessário
    Task<MinhaEntidade?> GetByCnpjAsync(string cnpj);
}
```

#### Implementação
```csharp
public class MinhaEntidadeRepository : GenericRepository<MinhaEntidade>, IMinhaEntidadeRepository
{
    public MinhaEntidadeRepository(CivitasDbContext context) : base(context)
    {
    }
    
    // Implemente métodos específicos
    public async Task<MinhaEntidade?> GetByCnpjAsync(string cnpj)
    {
        return await GetFirstOrDefaultAsync(e => e.Cnpj == cnpj);
    }
}
```

### 3. Service

#### Interface
```csharp
public interface IMinhaEntidadeService : IGenericService<MinhaEntidadeDto, MinhaEntidadeCreateDto, MinhaEntidadeUpdateDto>
{
    // Adicione métodos específicos aqui
}
```

#### Implementação
```csharp
public class MinhaEntidadeService : GenericService<MinhaEntidade, MinhaEntidadeDto, MinhaEntidadeCreateDto, MinhaEntidadeUpdateDto>, IMinhaEntidadeService
{
    private readonly IMinhaEntidadeRepository _repository;

    public MinhaEntidadeService(IMinhaEntidadeRepository repository, IMapper mapper) 
        : base(repository, mapper)
    {
        _repository = repository;
    }
    
    // Override para validações customizadas
    protected override async Task ValidateCreateAsync(MinhaEntidadeCreateDto createDto)
    {
        if (await _repository.ExistsAsync(e => e.Cnpj == createDto.Cnpj))
        {
            throw new InvalidOperationException("CNPJ já cadastrado");
        }
    }
    
    protected override async Task ValidateUpdateAsync(int id, MinhaEntidadeUpdateDto updateDto)
    {
        if (await _repository.ExistsAsync(e => e.Cnpj == updateDto.Cnpj && e.Id != id))
        {
            throw new InvalidOperationException("CNPJ já cadastrado");
        }
    }
}
```

### 4. Controller

```csharp
[Route("api/[controller]")]
[SwaggerTag("Operações relacionadas a MinhaEntidade")]
public class MinhaEntidadeController : GenericController<MinhaEntidadeDto, MinhaEntidadeCreateDto, MinhaEntidadeUpdateDto>
{
    public MinhaEntidadeController(IMinhaEntidadeService service) : base(service)
    {
    }
    
    // Override para fornecer o ID do item
    protected override int GetItemId(MinhaEntidadeDto item)
    {
        return item.Id;
    }
    
    // Adicione endpoints específicos se necessário
}
```

## ? Benefícios

- **Menos código repetitivo**: Implementação base já fornece operações CRUD
- **Padronização**: Todos os CRUDs seguem o mesmo padrão
- **Manutenibilidade**: Alterações na lógica genérica refletem em todos os CRUDs
- **Produtividade**: Criar novos CRUDs fica muito mais rápido
- **Validações centralizadas**: Lógica comum em um único lugar

## ?? Funcionalidades Incluídas

### Repository
- `GetAllAsync()` - Buscar todos
- `GetWhereAsync(predicate)` - Buscar com filtro
- `GetByIdAsync(id)` - Buscar por ID
- `GetFirstOrDefaultAsync(predicate)` - Buscar primeiro
- `ExistsAsync(predicate)` - Verificar existência
- `AddAsync(entity)` - Adicionar
- `UpdateAsync(entity)` - Atualizar
- `DeleteAsync(id)` - Remover

### Service
- `GetAllAsync()` - Listar todos
- `GetActiveAsync()` - Listar ativos
- `GetByIdAsync(id)` - Buscar por ID
- `CreateAsync(createDto)` - Criar
- `UpdateAsync(id, updateDto)` - Atualizar
- `ActivateAsync(id)` - Ativar (soft delete)
- `DeactivateAsync(id)` - Desativar (soft delete)
- `DeleteAsync(id)` - Remover permanentemente

### Controller
- `GET /api/{controller}` - Listar todos
- `GET /api/{controller}/active` - Listar ativos
- `GET /api/{controller}/{id}` - Buscar por ID
- `POST /api/{controller}` - Criar
- `PUT /api/{controller}/{id}` - Atualizar
- `PATCH /api/{controller}/{id}/activate` - Ativar
- `PATCH /api/{controller}/{id}/deactivate` - Desativar

## ?? Exemplo Completo: Secretaria

Veja a implementação de `Secretaria` como exemplo de uso das classes genéricas:
- `Models/Secretaria.cs`
- `Repositories/SecretariaRepository.cs`
- `Services/SecretariaService.cs`
- `Controllers/SecretariaController.cs`

## ?? Próximos Passos

Para criar um novo CRUD:

1. Crie a entidade herdando de `BaseEntity`
2. Crie o repositório herdando de `GenericRepository`
3. Crie o serviço herdando de `GenericService`
4. Crie o controller herdando de `GenericController`
5. Configure no `Program.cs` a injeção de dependência
6. Adicione o AutoMapper profile

Pronto! Seu CRUD estará funcional com todas as operações básicas! ??
