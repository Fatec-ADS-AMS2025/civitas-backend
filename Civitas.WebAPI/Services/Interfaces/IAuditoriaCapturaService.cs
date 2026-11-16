using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Gera os registros de auditoria a partir das alterações que o Entity Framework vai persistir.
    /// </summary>
    /// <remarks>
    /// Usado pelo <c>AppDbContext</c> dentro do <c>SaveChangesAsync</c>, para que a auditoria seja
    /// gravada na mesma transação da operação principal. Não depende do DbContext no construtor,
    /// evitando dependência circular.
    /// </remarks>
    public interface IAuditoriaCapturaService
    {
        /// <summary>
        /// Indica se as alterações atuais vêm de uma requisição da API e devem ser auditadas.
        /// Seeders e comandos de banco executados fora de requisição não geram auditoria automática.
        /// </summary>
        bool DeveAuditar { get; }

        /// <summary>
        /// Identifica as entidades auditáveis incluídas, alteradas ou excluídas e lê do banco
        /// o estado anterior real de cada registro alterado ou excluído.
        /// </summary>
        Task<IReadOnlyList<AuditoriaPendente>> CapturarAsync(DbContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Monta as auditorias depois que a operação principal foi gravada (quando os Ids já existem).
        /// </summary>
        IReadOnlyList<Auditoria> Gerar(IReadOnlyList<AuditoriaPendente> pendentes);

        /// <summary>
        /// Cria um registro com data, hora e autor definidos pelo backend.
        /// </summary>
        Auditoria CriarRegistro(string operacao, string nomeEntidade, string valoresAtingidos, string novosValores, int? usuarioId);

        /// <summary>
        /// Publica o evento de criação das auditorias já confirmadas no banco.
        /// </summary>
        Task PublicarAsync(IReadOnlyCollection<Auditoria> auditorias);
    }

    /// <summary>
    /// Alteração capturada antes da gravação, aguardando a geração da auditoria.
    /// </summary>
    /// <param name="Entrada">Entrada do ChangeTracker da entidade alterada.</param>
    /// <param name="Operacao">INSERT, UPDATE ou DELETE.</param>
    /// <param name="NomeEntidade">Nome da entidade auditada.</param>
    /// <param name="ValoresAnteriores">Valores lidos do banco antes da gravação (nulo em INSERT).</param>
    public sealed record AuditoriaPendente(
        EntityEntry Entrada,
        string Operacao,
        string NomeEntidade,
        IReadOnlyDictionary<string, object?>? ValoresAnteriores);
}
