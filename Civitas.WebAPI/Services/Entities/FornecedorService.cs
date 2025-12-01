using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento de Fornecedores (Credores) do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar as operações de cadastro e atualização de empresas prestadoras de serviço.
    /// 
    /// Regras de Negócio:
    /// - Atua como validador para garantir que apenas fornecedores com dados fiscais consistentes (CNPJ) sejam mantidos.
    /// - É essencial para a integridade financeira, pois sem um fornecedor válido, não se pode lançar despesas.
    /// 
    /// Dependências:
    /// - <see cref="IFornecedorRepository"/>: Persistência de dados.
    /// - <see cref="IMapper"/>: Transformação de objetos (DTO/Entity).
    /// </remarks>
    public class FornecedorService : GenericService<Fornecedor, FornecedorDTO>, IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Fornecedores com as dependências necessárias.
        /// </summary>
        /// <param name="fornecedorRepository">Repositório de acesso a dados de fornecedores.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada caso os parâmetros injetados sejam nulos.</exception>
        public FornecedorService(IFornecedorRepository fornecedorRepository, IMapper mapper)
            : base(fornecedorRepository, mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }
    }
}