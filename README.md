# 🏛️ Civitas Backend

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-purple?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Sistema de Gestão Pública Municipal**

*API RESTful para gerenciamento de despesas, orçamentos, instituições e fornecedores da administração pública*

[Recursos](#-recursos) •
[Tecnologias](#-tecnologias) •
[Instalação](#-instalação) •
[Uso](#-uso) •
[API Endpoints](#-api-endpoints) •
[Estrutura](#-estrutura-do-projeto) •
[Contribuição](#-contribuição)

</div>

---

## 📋 Sobre o Projeto

O **Civitas** é um sistema ERP (Enterprise Resource Planning) desenvolvido para auxiliar na gestão pública municipal. O backend foi construído utilizando ASP.NET Core 9.0, seguindo os princípios de arquitetura limpa e boas práticas de desenvolvimento.

O sistema permite o gerenciamento completo de:
- 👤 **Usuários** - Controle de acesso com diferentes tipos de permissões
- 🏢 **Secretarias** - Gestão das secretarias municipais
- 🏫 **Instituições** - Cadastro e controle de instituições públicas
- 💰 **Orçamentos** - Planejamento e acompanhamento orçamentário
- 💸 **Despesas** - Registro e controle de despesas públicas
- 🤝 **Fornecedores** - Cadastro de fornecedores e prestadores de serviço
- 📄 **Documentos** - Gestão documental do sistema
- 🔄 **Fluxos** - Controle de fluxos e processos
- 📊 **Auditorias** - Rastreabilidade de operações

---

## ✨ Recursos

- ✅ API RESTful completa com documentação Swagger
- ✅ Arquitetura em camadas (Controllers, Services, Repositories)
- ✅ Entity Framework Core com PostgreSQL
- ✅ AutoMapper para mapeamento de DTOs
- ✅ Injeção de Dependência nativa do .NET
- ✅ Suporte a CORS para integração com frontend
- ✅ Migrations para versionamento do banco de dados
- ✅ Padrão Repository para acesso a dados
- ✅ Responses padronizadas para consistência da API

---

## 🛠️ Tecnologias

### Backend
| Tecnologia | Versão | Descrição |
|------------|--------|-----------|
| **.NET** | 9.0 | Framework principal |
| **ASP.NET Core** | 9.0 | Framework web |
| **Entity Framework Core** | 9.0.9 | ORM para acesso a dados |
| **PostgreSQL** | - | Banco de dados relacional |
| **Npgsql** | 9.0.4 | Provider PostgreSQL para EF Core |
| **AutoMapper** | 15.0.1 | Mapeamento objeto-objeto |
| **Swashbuckle** | 9.0.6 | Documentação Swagger/OpenAPI |

---

## 📦 Instalação

### Pré-requisitos

Certifique-se de ter instalado em sua máquina:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior
- [PostgreSQL](https://www.postgresql.org/download/) 13 ou superior
- [Git](https://git-scm.com/)

### Clonando o Repositório

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

### Instalando Dependências e Executando Migrations

```bash
cd Civitas.WebAPI

# Restaurar pacotes NuGet
dotnet restore

# Aplicar migrations ao banco de dados
dotnet ef database update

# Executar a aplicação
dotnet run
```

---

## 🚀 Uso

Após iniciar a aplicação, acesse:

- **Swagger UI**: `http://localhost:5000` ou `https://localhost:5001`
- **API Base URL**: `http://localhost:5000/api`

A documentação interativa do Swagger permite testar todos os endpoints diretamente no navegador.

---

## 📡 API Endpoints

### Usuários
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Usuario` | Lista todos os usuários |
| `GET` | `/api/Usuario/GetUsuarioById?id={id}` | Busca usuário por ID |
| `GET` | `/api/Usuario/GetUsuarioByCpf?cpf={cpf}` | Busca usuário por CPF |
| `POST` | `/api/Usuario` | Cria novo usuário |
| `PUT` | `/api/Usuario` | Atualiza usuário |
| `DELETE` | `/api/Usuario?id={id}` | Remove usuário |

### Secretarias
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Secretaria` | Lista todas as secretarias |
| `GET` | `/api/Secretaria/{id}` | Busca secretaria por ID |
| `POST` | `/api/Secretaria` | Cria nova secretaria |
| `PUT` | `/api/Secretaria` | Atualiza secretaria |
| `DELETE` | `/api/Secretaria/{id}` | Remove secretaria |

### Fornecedores
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Fornecedor` | Lista todos os fornecedores |
| `GET` | `/api/Fornecedor/{id}` | Busca fornecedor por ID |
| `POST` | `/api/Fornecedor` | Cria novo fornecedor |
| `PUT` | `/api/Fornecedor` | Atualiza fornecedor |
| `DELETE` | `/api/Fornecedor/{id}` | Remove fornecedor |

### Instituições
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Instituicao` | Lista todas as instituições |
| `GET` | `/api/Instituicao/{id}` | Busca instituição por ID |
| `POST` | `/api/Instituicao` | Cria nova instituição |
| `PUT` | `/api/Instituicao` | Atualiza instituição |
| `DELETE` | `/api/Instituicao/{id}` | Remove instituição |

### Orçamentos
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Orcamento` | Lista todos os orçamentos |
| `GET` | `/api/Orcamento/{id}` | Busca orçamento por ID |
| `POST` | `/api/Orcamento` | Cria novo orçamento |
| `PUT` | `/api/Orcamento` | Atualiza orçamento |
| `DELETE` | `/api/Orcamento/{id}` | Remove orçamento |

### Despesas
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Despesa` | Lista todas as despesas |
| `GET` | `/api/Despesa/{id}` | Busca despesa por ID |
| `POST` | `/api/Despesa` | Cria nova despesa |
| `PUT` | `/api/Despesa` | Atualiza despesa |
| `DELETE` | `/api/Despesa/{id}` | Remove despesa |

### Documentos
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Documento` | Lista todos os documentos |
| `GET` | `/api/Documento/{id}` | Busca documento por ID |
| `POST` | `/api/Documento` | Cria novo documento |
| `PUT` | `/api/Documento` | Atualiza documento |
| `DELETE` | `/api/Documento/{id}` | Remove documento |

### Fluxos
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Fluxo` | Lista todos os fluxos |
| `GET` | `/api/Fluxo/{id}` | Busca fluxo por ID |
| `POST` | `/api/Fluxo` | Cria novo fluxo |
| `PUT` | `/api/Fluxo` | Atualiza fluxo |
| `DELETE` | `/api/Fluxo/{id}` | Remove fluxo |

### Auditorias
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/Auditoria` | Lista todas as auditorias |
| `GET` | `/api/Auditoria/{id}` | Busca auditoria por ID |
| `POST` | `/api/Auditoria` | Cria nova auditoria |

### Tipos de Instituição
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/TipoInstituicao` | Lista todos os tipos |
| `POST` | `/api/TipoInstituicao` | Cria novo tipo |

### Tipos de Despesa
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/TipoDespesa` | Lista todos os tipos |
| `POST` | `/api/TipoDespesa` | Cria novo tipo |

### Unidades de Medida
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/UnidadeMedida` | Lista todas as unidades |
| `POST` | `/api/UnidadeMedida` | Cria nova unidade |

---

## 📁 Estrutura do Projeto

```
civitas-backend/
├── 📂 Civitas.WebAPI/
│   ├── 📂 Controllers/          # Controladores da API
│   │   ├── AuditoriaController.cs
│   │   ├── DespesaController.cs
│   │   ├── DocumentoController.cs
│   │   ├── FluxoController.cs
│   │   ├── FornecedorController.cs
│   │   ├── InstituicaoController.cs
│   │   ├── OrcamentoController.cs
│   │   ├── SecretariaController.cs
│   │   ├── TipoDespesaController.cs
│   │   ├── TipoInstituicaoController.cs
│   │   ├── UnidadeMedidaController.cs
│   │   └── UsuarioController.cs
│   │
│   ├── 📂 Data/                 # Camada de dados
│   │   ├── AppDbContext.cs      # Contexto do Entity Framework
│   │   ├── 📂 Builders/         # Configurações de entidades
│   │   ├── 📂 Interfaces/       # Interfaces de repositórios
│   │   └── 📂 Repositories/     # Implementações dos repositórios
│   │
│   ├── 📂 Migrations/           # Migrations do EF Core
│   │
│   ├── 📂 Objects/              # Objetos do domínio
│   │   ├── 📂 Contracts/        # Contratos de resposta
│   │   ├── 📂 Dtos/             # Data Transfer Objects
│   │   ├── 📂 Enums/            # Enumeradores
│   │   │   ├── Situacao.cs
│   │   │   ├── SolicitaUc.cs
│   │   │   ├── Status.cs
│   │   │   └── TipoUsuario.cs
│   │   └── 📂 Models/           # Entidades do domínio
│   │       ├── Auditoria.cs
│   │       ├── Despesa.cs
│   │       ├── Documento.cs
│   │       ├── Fluxo.cs
│   │       ├── Fornecedor.cs
│   │       ├── Instituicao.cs
│   │       ├── Orcamento.cs
│   │       ├── Secretaria.cs
│   │       ├── TipoDespesa.cs
│   │       ├── TipoInstituicao.cs
│   │       ├── UnidadeMedida.cs
│   │       └── Usuario.cs
│   │
│   ├── 📂 Services/             # Camada de serviços
│   │   ├── 📂 Entities/         # Implementações dos serviços
│   │   └── 📂 Interfaces/       # Interfaces dos serviços
│   │
│   ├── 📂 sql/                  # Scripts SQL auxiliares
│   │
│   ├── appsettings.json         # Configurações da aplicação
│   ├── Program.cs               # Ponto de entrada da aplicação
│   └── Civitas.WebAPI.csproj    # Arquivo de projeto
│
├── 📂 documentation/            # Documentação do projeto
│   ├── 📂 ClassDiagram/         # Diagramas de classe
│   └── 📂 template/             # Templates de documentação
│
└── README.md                    # Este arquivo
```

---

## 🗃️ Modelo de Dados

### Entidades Principais

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│    Secretaria   │     │   Instituição   │     │   Fornecedor    │
├─────────────────┤     ├─────────────────┤     ├─────────────────┤
│ IdSecretaria    │     │ Id              │     │ IdFornecedor    │
│ Nome            │     │ Nome            │     │ NomeFantasia    │
│ CNPJ            │     │ CNPJ            │     │ CNPJ            │
│ Descrição       │     │ IdTipoInstituição│    │ Nome            │
│ Situação        │     │ Situação        │     │ Situação        │
└─────────────────┘     └────────┬────────┘     └────────┬────────┘
                                 │                       │
                                 ▼                       ▼
                        ┌─────────────────┐     ┌─────────────────┐
                        │    Orçamento    │     │    Documento    │
                        ├─────────────────┤     ├─────────────────┤
                        │ IdOrcamento     │     │ Id              │
                        │ AnoOrcamento    │     │ Nome            │
                        │ ValorOrcamento  │     │ IdFornecedor    │
                        │ IdInstituição   │     └─────────────────┘
                        │ IdTipoDespesa   │
                        └────────┬────────┘
                                 │
                                 ▼
                        ┌─────────────────┐
                        │     Despesa     │
                        ├─────────────────┤
                        │ Id              │
                        │ NumeroDocumento │
                        │ UC              │
                        │ DataEmissão     │
                        │ ConsumoPrevisto │
                        │ DataVencimento  │
                        │ IdOrcamento     │
                        │ IdInstituição   │
                        │ IdFornecedor    │
                        │ IdUsuário       │
                        └─────────────────┘
```

---

## 🔧 Configuração de Desenvolvimento

### Variáveis de Ambiente

Para desenvolvimento local, você pode configurar as seguintes variáveis:

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | `Development` |
| `ConnectionStrings__DefaultConnection` | String de conexão PostgreSQL | Ver `appsettings.json` |

### Executando em Modo de Desenvolvimento

```bash
# Modo watch (recompila automaticamente)
dotnet watch run

# Modo normal
dotnet run
```

### Aplicando Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations pendentes
dotnet ef database update

# Reverter última migration
dotnet ef migrations remove
```

---

## 🤝 Contribuição

Contribuições são bem-vindas! Para contribuir:

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona NovaFeature'`)
4. Push para a branch (`git push origin feature/NovaFeature`)
5. Abra um Pull Request

### Padrões de Código

- Utilize nomes em português para entidades de negócio
- Siga o padrão de nomenclatura do C# (PascalCase para classes e métodos públicos)
- Documente métodos públicos complexos
- Escreva testes unitários para novos recursos

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👥 Equipe

Desenvolvido com ❤️ pela equipe **FATEC-ADS-AMS2025**

---

<div align="center">

**[⬆ Voltar ao topo](#️-civitas-backend)**

</div>