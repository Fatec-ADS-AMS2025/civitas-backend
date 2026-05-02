using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class SecretariaRepository : GenericRepository<Secretaria>, ISecretariaRepository
    {
        private readonly AppDbContext _appDbContext;

        public SecretariaRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null)
        {
            var query = _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.Cnpj == cnpj);

            if (ignoreId.HasValue)
            {
                query = query.Where(secretaria => secretaria.IdSecretaria != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null)
        {
            var normalizedEmail = email.ToLower();

            var query = _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.Email.ToLower() == normalizedEmail);

            if (ignoreId.HasValue)
            {
                query = query.Where(secretaria => secretaria.IdSecretaria != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<SecretariaGastosDTO?> GetGastosBySecretariaIdAsync(int secretariaId)
        {
            return await _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.IdSecretaria == secretariaId)
                .Select(secretaria => new SecretariaGastosDTO
                {
                    IdSecretaria = secretaria.IdSecretaria,
                    NomeSecretaria = secretaria.Nome,
                    QuantidadeInstituicoes = _appDbContext.Instituicoes.Count(instituicao => instituicao.IdSecretaria == secretaria.IdSecretaria),
                    QuantidadeDespesas = _appDbContext.Despesas.Count(despesa => despesa.Instituicao.IdSecretaria == secretaria.IdSecretaria),
                    TotalGastos = _appDbContext.Despesas
                        .Where(despesa => despesa.Instituicao.IdSecretaria == secretaria.IdSecretaria)
                        .Sum(despesa => despesa.ConsumoPrevisto)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<SecretariaOrcamentoDisponivelDTO?> GetOrcamentoDisponivelBySecretariaIdAsync(int secretariaId)
        {
            return await _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.IdSecretaria == secretariaId)
                .Select(secretaria => new SecretariaOrcamentoDisponivelDTO
                {
                    IdSecretaria = secretaria.IdSecretaria,
                    NomeSecretaria = secretaria.Nome,
                    QuantidadeInstituicoes = _appDbContext.Instituicoes.Count(instituicao => instituicao.IdSecretaria == secretaria.IdSecretaria),
                    TotalOrcamentoDisponivel = 
                        (
                            _appDbContext.Orcamentos
                                .Where(orcamento => orcamento.Instituicao.IdSecretaria == secretaria.IdSecretaria)
                                .Sum(orcamento => (decimal?)orcamento.ValorOrcamento) ?? 0
                        )
                        -
                        (
                            _appDbContext.Despesas
                                .Where(despesa => despesa.Instituicao.IdSecretaria == secretaria.IdSecretaria)
                                .Sum(despesa => (decimal?)despesa.ConsumoPrevisto) ?? 0
                        )
                })
                .FirstOrDefaultAsync();
        }
    }
}
