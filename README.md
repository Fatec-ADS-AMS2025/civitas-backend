# Civitas Backend

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-purple?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Sistema de Gestao Publica Municipal**

*API RESTful para gerenciamento de despesas, orcamentos, instituicoes e fornecedores da administracao publica*

[Recursos](#recursos) •
[Tecnologias](#tecnologias) •
[Instalacao](#instalacao) •
[Uso](#uso) •
[API Endpoints](#api-endpoints) •
[Estrutura](#estrutura-do-projeto) •
[Contribuicao](#contribuicao)

</div>

---

## Sobre o Projeto

O **Civitas** e um sistema ERP (Enterprise Resource Planning) desenvolvido para auxiliar na gestao publica municipal. O backend foi construido utilizando ASP.NET Core 9.0, seguindo principios de arquitetura em camadas e boas praticas de desenvolvimento.

O sistema permite o gerenciamento completo de:
- **Usuarios** - Controle de acesso com diferentes tipos de permissoes
- **Secretarias** - Gestao das secretarias municipais
- **Instituicoes** - Cadastro e controle de instituicoes publicas
- **Orcamentos** - Planejamento e acompanhamento orcamentario
- **Despesas** - Registro e controle de despesas publicas
- **Fornecedores** - Cadastro de fornecedores e prestadores de servico
- **Documentos** - Gestao documental do sistema
- **Fluxos** - Controle de fluxos e processos
- **Auditorias** - Rastreabilidade de operacoes

---

## Recursos

- API RESTful com documentacao Swagger
- Arquitetura em camadas (Controllers, Services, Repositories)
- Entity Framework Core com PostgreSQL
- AutoMapper para mapeamento de DTOs
- Injecao de dependencia nativa do .NET
- Suporte a CORS para integracao com frontend
- Migrations para versionamento do banco de dados
- Padrao Repository para acesso a dados
- Responses padronizadas para consistencia da API

---

## Tecnologias

### Backend
| Tecnologia | Versao | Descricao |
|------------|--------|-----------|
| **.NET** | 9.0 | Framework principal |
| **ASP.NET Core** | 9.0 | Framework web |
| **Entity Framework Core** | 9.0.9 | ORM para acesso a dados |
| **PostgreSQL** | - | Banco de dados relacional |
| **Npgsql** | 9.0.4 | Provider PostgreSQL para EF Core |
| **AutoMapper** | 15.0.1 | Mapeamento objeto-objeto |
| **Swashbuckle** | 9.0.6 | Documentacao Swagger/OpenAPI |

---

## Instalacao

### Pre-requisitos

Certifique-se de ter instalado em sua maquina:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior
- [PostgreSQL](https://www.postgresql.org/download/) 13 ou superior
- [Git](https://git-scm.com/)

### Clonando o Repositorio

```bash
git clone https://github.com/Fatec-ADS-AMS2025/civitas-backend.git
cd civitas-backend
```

### Configurando o Banco de Dados

1. Crie um banco de dados PostgreSQL chamado `civitas`:

```sql
CREATE DATABASE civitas;
```

2. Configure a connection string no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Username=postgres;Port=5432;Password=SUA_SENHA;Database=civitas"
  }
}
```

### Instalando Dependencias e Executando Migrations

```bash
cd Civitas.WebAPI

dotnet restore
dotnet ef database update
dotnet run
```

---

## Uso

Apos iniciar a aplicacao, acesse:

- **Swagger UI**: `http://localhost:5000` ou `https://localhost:5001`
- **API Base URL**: `http://localhost:5000/api`

A documentacao interativa do Swagger permite testar os endpoints diretamente no navegador.

---

## API Endpoints

### Usuarios
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/usuarios` | Lista usuarios ativos |
| `GET` | `/api/usuarios/inativos` | Lista usuarios inativos |
| `GET` | `/api/usuarios/cpf?cpf={cpf}` | Busca usuario por CPF |
| `GET` | `/api/usuarios/{id}` | Busca usuario por ID |
| `POST` | `/api/usuarios` | Cria novo usuario |
| `PUT` | `/api/usuarios/{id}` | Atualiza usuario |
| `PATCH` | `/api/usuarios/situacao/{id}` | Alterna situacao do usuario |

### Secretarias
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/secretarias` | Lista secretarias ativas |
| `GET` | `/api/secretarias/inativos` | Lista secretarias inativas |
| `GET` | `/api/secretarias/{id}` | Busca secretaria por ID |
| `POST` | `/api/secretarias` | Cria nova secretaria |
| `PUT` | `/api/secretarias/{id}` | Atualiza secretaria |
| `PATCH` | `/api/secretarias/situacao/{id}` | Alterna situacao da secretaria |

### Fornecedores
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/fornecedores` | Lista fornecedores ativos |
| `GET` | `/api/fornecedores/inativos` | Lista fornecedores inativos |
| `GET` | `/api/fornecedores/{id}` | Busca fornecedor por ID |
| `POST` | `/api/fornecedores` | Cria novo fornecedor |
| `PUT` | `/api/fornecedores/{id}` | Atualiza fornecedor |
| `PATCH` | `/api/fornecedores/situacao/{id}` | Alterna situacao do fornecedor |

### Instituicoes
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/instituicoes` | Lista instituicoes ativas |
| `GET` | `/api/instituicoes/inativos` | Lista instituicoes inativas |
| `GET` | `/api/instituicoes/{id}` | Busca instituicao por ID |
| `GET` | `/api/instituicoes/nome?name={name}` | Busca instituicoes por nome |
| `POST` | `/api/instituicoes` | Cria nova instituicao |
| `PUT` | `/api/instituicoes/{id}` | Atualiza instituicao |
| `PATCH` | `/api/instituicoes/situacao/{id}` | Alterna situacao da instituicao |

### Orcamentos
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/orcamentos` | Lista orcamentos |
| `GET` | `/api/orcamentos/{id}` | Busca orcamento por ID |
| `POST` | `/api/orcamentos` | Cria novo orcamento |
| `PUT` | `/api/orcamentos/{id}` | Atualiza orcamento |

### Despesas
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/despesas` | Lista despesas ativas |
| `GET` | `/api/despesas/inativos` | Lista despesas inativas |
| `GET` | `/api/despesas/{id}` | Busca despesa por ID |
| `POST` | `/api/despesas` | Cria nova despesa |
| `PUT` | `/api/despesas/{id}` | Atualiza despesa |
| `PATCH` | `/api/despesas/situacao/{id}` | Alterna situacao da despesa |

### Documentos
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/documentos` | Lista documentos |
| `GET` | `/api/documentos/{id}` | Busca documento por ID |
| `POST` | `/api/documentos` | Cria novo documento |
| `PUT` | `/api/documentos/{id}` | Atualiza documento |

### Fluxos
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/fluxos` | Lista fluxos |
| `GET` | `/api/fluxos/{id}` | Busca fluxo por ID |
| `POST` | `/api/fluxos` | Cria novo fluxo |
| `PUT` | `/api/fluxos/{id}` | Atualiza fluxo |
| `PATCH` | `/api/fluxos/status/{id}` | Atualiza o status do fluxo |

### Auditorias
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/auditorias` | Lista auditorias ativas |
| `GET` | `/api/auditorias/inativos` | Lista auditorias inativas |
| `GET` | `/api/auditorias/{id}` | Busca auditoria por ID |
| `GET` | `/api/auditorias/usuario/{usuarioId}` | Lista auditorias por usuario |
| `GET` | `/api/auditorias/entidade?nomeEntidade={nomeEntidade}` | Busca auditorias por entidade |
| `GET` | `/api/auditorias/operacao?operacao={operacao}` | Busca auditorias por operacao |
| `POST` | `/api/auditorias` | Cria nova auditoria |
| `PATCH` | `/api/auditorias/situacao/{id}` | Alterna situacao da auditoria |

### Tipos de Instituicao
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/tipo-instituicao` | Lista tipos de instituicao ativos |
| `GET` | `/api/tipo-instituicao/inativos` | Lista tipos de instituicao inativos |
| `GET` | `/api/tipo-instituicao/{id}` | Busca tipo de instituicao por ID |
| `POST` | `/api/tipo-instituicao` | Cria novo tipo de instituicao |
| `PUT` | `/api/tipo-instituicao/{id}` | Atualiza tipo de instituicao |
| `PATCH` | `/api/tipo-instituicao/situacao/{id}` | Alterna situacao do tipo de instituicao |

### Tipos de Despesa
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/tipo-despesa` | Lista tipos de despesa ativos |
| `GET` | `/api/tipo-despesa/inativos` | Lista tipos de despesa inativos |
| `GET` | `/api/tipo-despesa/{id}` | Busca tipo de despesa por ID |
| `POST` | `/api/tipo-despesa` | Cria novo tipo de despesa |
| `PUT` | `/api/tipo-despesa/{id}` | Atualiza tipo de despesa |
| `PATCH` | `/api/tipo-despesa/situacao/{id}` | Alterna situacao do tipo de despesa |

### Unidades de Medida
| Metodo | Endpoint | Descricao |
|--------|----------|-----------|
| `GET` | `/api/unidade-medida` | Lista unidades de medida ativas |
| `GET` | `/api/unidade-medida/inativos` | Lista unidades de medida inativas |
| `GET` | `/api/unidade-medida/{id}` | Busca unidade de medida por ID |
| `POST` | `/api/unidade-medida` | Cria nova unidade de medida |
| `PUT` | `/api/unidade-medida/{id}` | Atualiza unidade de medida |
| `PATCH` | `/api/unidade-medida/situacao/{id}` | Alterna situacao da unidade de medida |

---

## Estrutura do Projeto

```text
civitas-backend/
|-- Civitas.WebAPI/
|   |-- Controllers/
|   |   |-- AuditoriaController.cs
|   |   |-- DespesaController.cs
|   |   |-- DocumentoController.cs
|   |   |-- FluxoController.cs
|   |   |-- FornecedorController.cs
|   |   |-- InstituicaoController.cs
|   |   |-- OrcamentoController.cs
|   |   |-- SecretariaController.cs
|   |   |-- TipoDespesaController.cs
|   |   |-- TipoInstituicaoController.cs
|   |   |-- UnidadeMedidaController.cs
|   |   `-- UsuarioController.cs
|   |-- Data/
|   |   |-- AppDbContext.cs
|   |   |-- Builders/
|   |   |-- Interfaces/
|   |   `-- Repositories/
|   |-- Migrations/
|   |-- Objects/
|   |   |-- Contracts/
|   |   |-- Dtos/
|   |   |-- Enums/
|   |   `-- Models/
|   |-- Services/
|   |   |-- Entities/
|   |   `-- Interfaces/
|   |-- sql/
|   |-- appsettings.json
|   |-- Program.cs
|   `-- Civitas.WebAPI.csproj
|-- documentation/
|   |-- ClassDiagram/
|   `-- template/
`-- README.md
```

---

## Modelo de Dados

As entidades principais do sistema incluem Secretaria, Instituicao, Fornecedor, Orcamento, Documento e Despesa, com relacionamento entre cadastro institucional, planejamento orcamentario e execucao de despesas.

---

## Configuracao de Desenvolvimento

### Variaveis de Ambiente

| Variavel | Descricao | Valor Padrao |
|----------|-----------|--------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execucao | `Development` |
| `ConnectionStrings__DefaultConnection` | String de conexao PostgreSQL | Ver `appsettings.json` |

### Executando em Modo de Desenvolvimento

```bash
dotnet watch run
```

```bash
dotnet run
```

### Aplicando Migrations

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
dotnet ef migrations remove
```

---

## Contribuicao

Contribuicoes sao bem-vindas. Para contribuir:

1. Faca um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFeature`)
3. Commit suas mudancas (`git commit -m 'Adiciona NovaFeature'`)
4. Push para a branch (`git push origin feature/NovaFeature`)
5. Abra um Pull Request

### Padroes de Codigo

- Utilize nomes em portugues para entidades de negocio
- Siga o padrao de nomenclatura do C# (PascalCase para classes e metodos publicos)
- Documente metodos publicos complexos
- Escreva testes unitarios para novos recursos

---

## Licenca

Este projeto esta sob a licenca MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

## Equipe

Desenvolvido pela equipe **FATEC-ADS-AMS2025**.
