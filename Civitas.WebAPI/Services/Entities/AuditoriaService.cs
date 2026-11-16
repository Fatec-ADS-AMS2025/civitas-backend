using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço especializado na consulta dos logs de auditoria e no registro dos eventos de autenticação.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar a busca de históricos de alterações (Audit Trail).
    /// - Validar os filtros recebidos (Id, usuário, entidade e operação) com o <see cref="AuditoriaValidator"/>.
    /// - Registrar o LOGIN, já que a autenticação não altera nenhuma entidade.
    ///
    /// As auditorias de INSERT, UPDATE e DELETE são geradas pelo AppDbContext na mesma transação
    /// da operação; este serviço não oferece criação genérica, alteração nem exclusão.
    /// </remarks>
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IAuditoriaCapturaService _auditoriaCaptura;
        private readonly IMapper _mapper;

        public AuditoriaService(
            IAuditoriaRepository auditoriaRepository,
            IAuditoriaCapturaService auditoriaCaptura,
            IMapper mapper)
        {
            _auditoriaRepository = auditoriaRepository;
            _auditoriaCaptura = auditoriaCaptura;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<AuditoriaDTO>> GetPage(PaginationQuery paginationQuery)
        {
            var page = await _auditoriaRepository.GetPage(paginationQuery);

            return new PaginatedResult<AuditoriaDTO>
            {
                Items = _mapper.Map<List<AuditoriaDTO>>(page.Items),
                TotalRecords = page.TotalRecords,
                TotalPages = page.TotalPages,
                CurrentPage = page.CurrentPage,
                PageSize = page.PageSize
            };
        }

        /// <inheritdoc />
        public async Task<AuditoriaDTO?> GetById(int id)
        {
            ValidarId(id, "O Id da auditoria deve ser maior que zero.");

            var auditoria = await _auditoriaRepository.GetByIdSemRastreamento(id);
            return auditoria is null ? null : _mapper.Map<AuditoriaDTO>(auditoria);
        }

        /// <summary>
        /// Busca todo o histórico de atividades realizadas por um usuário específico.
        /// </summary>
        /// <remarks>
        /// Utilidade:
        /// - Rastrear ações de um funcionário específico.
        /// - Investigar comportamentos suspeitos ou erros operacionais atrelados a um login.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId)
        {
            ValidarId(usuarioId, "O Id do usuário deve ser maior que zero.");

            var auditorias = await _auditoriaRepository.GetByUsuarioId(usuarioId);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        /// <summary>
        /// Busca o histórico de alterações sofridas por uma entidade específica.
        /// </summary>
        /// <remarks>
        /// Utilidade:
        /// - Visualizar a "linha do tempo" de modificações de um módulo.
        /// - Identificar quando um registro específico foi alterado.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string? nomeEntidade)
        {
            if (string.IsNullOrWhiteSpace(nomeEntidade))
            {
                throw new AuditoriaValidationException(["O nome da entidade é obrigatório."]);
            }

            var entidade = AuditoriaValidator.NormalizarNomeEntidade(nomeEntidade)
                ?? throw new AuditoriaValidationException([AuditoriaValidator.MensagemEntidadeInvalida(nomeEntidade)]);

            var auditorias = await _auditoriaRepository.GetByEntidade(entidade);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        /// <summary>
        /// Busca logs filtrados pelo tipo de operação executada.
        /// </summary>
        /// <remarks>
        /// Utilidade:
        /// - Auditoria de segurança (ex: listar tudo que foi DELETADO recentemente).
        /// - Análise de volume de inserções ou atualizações no sistema.
        /// </remarks>
        public async Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string? operacao)
        {
            if (string.IsNullOrWhiteSpace(operacao))
            {
                throw new AuditoriaValidationException(["A operação é obrigatória."]);
            }

            var operacaoNormalizada = AuditoriaValidator.NormalizarOperacao(operacao)
                ?? throw new AuditoriaValidationException([AuditoriaValidator.MensagemOperacaoInvalida(operacao)]);

            var auditorias = await _auditoriaRepository.GetByOperacao(operacaoNormalizada);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        /// <inheritdoc />
        public async Task RegistrarLoginAsync(int usuarioId)
        {
            var auditoria = _auditoriaCaptura.CriarRegistro(
                AuditoriaOperacao.LOGIN,
                AuditoriaValidator.EntidadeAutenticacao,
                valoresAtingidos: string.Empty,
                novosValores: string.Empty,
                usuarioId);

            // Validação e notificação acontecem no AppDbContext, depois do commit.
            await _auditoriaRepository.Add(auditoria);
        }

        private static void ValidarId(int id, string mensagem)
        {
            if (id <= 0) throw new AuditoriaValidationException([mensagem]);
        }
    }
}
