using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço especializado na recuperação e análise de logs de auditoria do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar a busca de históricos de alterações (Audit Trail).
    /// - Fornecer filtros específicos (por usuário, entidade ou operação) para relatórios de conformidade e segurança.
    /// 
    /// Dependências:
    /// - <see cref="IAuditoriaRepository"/>: Acesso otimizado aos dados de log.
    /// - <see cref="IMapper"/>: Transformação de objetos Entity para DTO.
    /// </remarks>
    public class AuditoriaService : GenericService<Auditoria, AuditoriaDTO>, IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Auditoria.
        /// </summary>
        /// <param name="auditoriaRepository">Repositório de auditoria injetado.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public AuditoriaService(IAuditoriaRepository auditoriaRepository, IMapper mapper)
            : base(auditoriaRepository, mapper)
        {
            _auditoriaRepository = auditoriaRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca todo o histórico de atividades realizadas por um usuário específico.
        /// </summary>
        /// <param name="usuarioId">O identificador único do usuário a ser auditado.</param>
        /// <returns>Uma coleção de registros de auditoria vinculados a este usuário.</returns>
        /// <remarks>
        /// Utilidade:
        /// - Rastrear ações de um funcionário específico.
        /// - Investigar comportamentos suspeitos ou erros operacionais atrelados a um login.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId)
        {
            var auditorias = await _auditoriaRepository.GetByUsuarioId(usuarioId);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        /// <summary>
        /// Busca o histórico de alterações sofridas por uma entidade (tabela) específica.
        /// </summary>
        /// <param name="nomeEntidade">O nome da entidade alvo (ex: "Despesa", "Fornecedor").</param>
        /// <returns>Uma coleção de logs filtrados pela entidade solicitada.</returns>
        /// <remarks>
        /// Utilidade:
        /// - Visualizar a "linha do tempo" de modificações de um módulo.
        /// - Identificar quando um registro específico foi alterado.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string nomeEntidade)
        {
            var auditorias = await _auditoriaRepository.GetByEntidade(nomeEntidade);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        /// <summary>
        /// Busca logs filtrados pelo tipo de operação executada no banco de dados.
        /// </summary>
        /// <param name="operacao">O tipo de comando executado (ex: "INSERT", "UPDATE", "DELETE").</param>
        /// <returns>Uma coleção de logs correspondentes ao tipo de operação.</returns>
        /// <remarks>
        /// Utilidade:
        /// - Auditoria de segurança (ex: listar tudo que foi DELETADO recentemente).
        /// - Análise de volume de inserções ou atualizações no sistema.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string operacao)
        {
            var auditorias = await _auditoriaRepository.GetByOperacao(operacao);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }
    }
}