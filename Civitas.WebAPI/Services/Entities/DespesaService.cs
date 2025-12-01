using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio e operações de gerenciamento de Despesas.
    /// </summary>
    /// <remarks>
    /// Esta classe estende <see cref="GenericService{T, TDto}"/> para fornecer operações padrão de CRUD (Create, Read, Update, Delete).
    /// 
    /// Dependências e Integrações:
    /// - Utiliza <see cref="IDespesaRepository"/> para persistência de dados.
    /// - Utiliza <see cref="IMapper"/> para conversão entre Despesa (Entity) e DespesaDTO.
    /// 
    /// Regras de Negócio Centrais (Implementadas ou Herdadas):
    /// - O serviço garante que a despesa esteja vinculada a um Orçamento válido.
    /// - Valida a integridade referencial com Fornecedor e Instituição.
    /// </remarks>
    public class DespesaService : GenericService<Despesa, DespesaDTO>, IDespesaService
    {
        private readonly IDespesaRepository _despesaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância do serviço de Despesas com as dependências necessárias.
        /// </summary>
        /// <param name="despesaRepository">Instância do repositório de despesas (Injeção de Dependência).</param>
        /// <param name="mapper">Instância do AutoMapper para transformação de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada se os repositórios ou mapper injetados forem nulos.</exception>
        public DespesaService(IDespesaRepository despesaRepository, IMapper mapper)
            : base(despesaRepository, mapper)
        {
            _despesaRepository = despesaRepository;
            _mapper = mapper;
        }
    }
}