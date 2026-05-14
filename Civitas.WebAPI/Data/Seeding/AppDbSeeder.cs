using System.Text;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Security;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Seeding
{
    public class AppDbSeeder
    {
        private const string DevPassword = "123456";

        private readonly AppDbContext _context;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ILogger<AppDbSeeder> _logger;

        public AppDbSeeder(
            AppDbContext context,
            IPasswordHashService passwordHashService,
            ILogger<AppDbSeeder> logger)
        {
            _context = context;
            _passwordHashService = passwordHashService;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.Usuarios.AnyAsync(cancellationToken) ||
                await _context.Fornecedores.AnyAsync(cancellationToken) ||
                await _context.Secretarias.AnyAsync(cancellationToken) ||
                await _context.Instituicoes.AnyAsync(cancellationToken) ||
                await _context.Orcamentos.AnyAsync(cancellationToken) ||
                await _context.UnidadesConsumidoras.AnyAsync(cancellationToken) ||
                await _context.Despesas.AnyAsync(cancellationToken) ||
                await _context.Auditorias.AnyAsync(cancellationToken) ||
                await _context.TipoInstituicoes.AnyAsync(cancellationToken) ||
                await _context.UnidadesMedida.AnyAsync(cancellationToken) ||
                await _context.TiposDespesa.AnyAsync(cancellationToken) ||
                await _context.TipoCodigos.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Seed de desenvolvimento ignorado: o banco ja possui dados.");
                return;
            }

            var admin = new Usuario(
                0, "12345678901", "Administrador Dev", "123456789",
                "Rua das Palmeiras", "100", "Curitiba", "PR", "80000000",
                "admin@civitas.dev",
                _passwordHashService.Hash(DevPassword),
                Situacao.ATIVO,
                "ADM-0001",
                TipoUsuario.ADMINISTRADOR,
                "Centro");

            var funcionario = new Usuario(
                0, "98765432100", "Funcionario Dev", "987654321",
                "Avenida das Araucarias", "250", "Curitiba", "PR", "80010000",
                "funcionario@civitas.dev",
                _passwordHashService.Hash(DevPassword),
                Situacao.ATIVO,
                "FUN-0001",
                TipoUsuario.FUNCIONARIO,
                "Batel");

            await _context.Usuarios.AddRangeAsync([admin, funcionario], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var tipoCodigoConsumo = new TipoCodigo(0, "CONSUMO", "Despesas recorrentes vinculadas ao consumo medido.");
            var tipoCodigoServico = new TipoCodigo(0, "SERVICO", "Despesas operacionais e contratos de servicos.");

            var unidadeKwh = new UnidadeMedida(0, "Quilowatt-hora", "kWh", Situacao.ATIVO);
            var unidadeUnidade = new UnidadeMedida(0, "Unidade", "un", Situacao.ATIVO);

            var tipoInstituicaoEscola = new TipoInstituicao(0, "Escola Municipal", Situacao.ATIVO);
            var tipoInstituicaoSaude = new TipoInstituicao(0, "Unidade Basica de Saude", Situacao.ATIVO);

            await _context.TipoCodigos.AddRangeAsync([tipoCodigoConsumo, tipoCodigoServico], cancellationToken);
            await _context.UnidadesMedida.AddRangeAsync([unidadeKwh, unidadeUnidade], cancellationToken);
            await _context.TipoInstituicoes.AddRangeAsync([tipoInstituicaoEscola, tipoInstituicaoSaude], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var tipoDespesaEnergia = new TipoDespesa(0, "Energia Eletrica", SolicitaUc.Sim, Situacao.ATIVO)
            {
                IdTipoCodigo = tipoCodigoConsumo.Id,
                IdUnidadeMedida = unidadeKwh.Id
            };

            var tipoDespesaManutencao = new TipoDespesa(0, "Manutencao Predial", SolicitaUc.Não, Situacao.ATIVO)
            {
                IdTipoCodigo = tipoCodigoServico.Id,
                IdUnidadeMedida = unidadeUnidade.Id
            };

            await _context.TiposDespesa.AddRangeAsync([tipoDespesaEnergia, tipoDespesaManutencao], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var secretariaEducacao = new Secretaria(
                0, Situacao.ATIVO,
                "Responsavel pela gestao da rede municipal de ensino.",
                "11222333000144",
                "Secretaria Municipal de Educacao",
                "Rua da Prefeitura",
                "500",
                "Centro",
                "80020000",
                "Secretaria Municipal de Educacao de Curitiba",
                "educacao@civitas.dev",
                "4133330001",
                "Curitiba",
                "PR");

            var secretariaSaude = new Secretaria(
                0, Situacao.ATIVO,
                "Responsavel pela gestao das unidades de atencao primaria.",
                "22333444000155",
                "Secretaria Municipal de Saude",
                "Rua da Cidadania",
                "700",
                "Centro Civico",
                "80030000",
                "Secretaria Municipal de Saude de Curitiba",
                "saude@civitas.dev",
                "4133330002",
                "Curitiba",
                "PR");

            await _context.Secretarias.AddRangeAsync([secretariaEducacao, secretariaSaude], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var fornecedorEnergia = new Fornecedor(
                0,
                "Copel Dev",
                Situacao.ATIVO,
                "33444555000166",
                "Companhia Paranaense de Energia Dev",
                "Rua da Energia",
                "1200",
                "Jardim Botanico",
                "80210000",
                "4132221000",
                "faturamento@copel.dev",
                "Curitiba",
                "PR",
                tipoDespesaEnergia.Id);

            var fornecedorManutencao = new Fornecedor(
                0,
                "Manutencao Escolar Dev",
                Situacao.ATIVO,
                "44555666000177",
                "Manutencao Escolar LTDA",
                "Rua das Oficinas",
                "321",
                "Boqueirao",
                "81730000",
                "4132222000",
                "contato@manutencao.dev",
                "Curitiba",
                "PR",
                tipoDespesaManutencao.Id);

            await _context.Fornecedores.AddRangeAsync([fornecedorEnergia, fornecedorManutencao], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var escola = new Instituicao(
                0,
                "55666777000188",
                "Escola Municipal Horizonte",
                "Rua do Saber",
                "45",
                "Cajuru",
                "82900000",
                "Escola Municipal Horizonte",
                "4131110001",
                "horizonte@civitas.dev",
                "Curitiba",
                "PR",
                Situacao.ATIVO)
            {
                IdSecretaria = secretariaEducacao.IdSecretaria,
                IdTipoInstituicao = tipoInstituicaoEscola.Id
            };

            var ubs = new Instituicao(
                0,
                "66777888000199",
                "UBS Vila Aurora",
                "Rua da Saude",
                "88",
                "Portao",
                "81070000",
                "UBS Vila Aurora",
                "4131110002",
                "ubs-aurora@civitas.dev",
                "Curitiba",
                "PR",
                Situacao.ATIVO)
            {
                IdSecretaria = secretariaSaude.IdSecretaria,
                IdTipoInstituicao = tipoInstituicaoSaude.Id
            };

           

            await _context.Instituicoes.AddRangeAsync([escola, ubs], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var orcamentoEscola = new Orcamento(
                0,
                DateTime.UtcNow.Year,
                350000.00m,
                escola.Id,
                tipoDespesaEnergia.Id,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m);

            var orcamentoUbs = new Orcamento(
                0,
                DateTime.UtcNow.Year,
                420000.00m,
                ubs.Id,
                tipoDespesaManutencao.Id,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m,
                0m, 0m);

            await _context.Orcamentos.AddRangeAsync([orcamentoEscola, orcamentoUbs], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var unidadeConsumidoraEscola = new UnidadeConsumidora(
                0,
                "UC-998877",
                escola.Id,
                tipoDespesaEnergia.Id,
                secretariaEducacao.IdSecretaria,
                orcamentoEscola.IdOrcamento,
                fornecedorEnergia.IdFornecedor,
                false,
                null);

            var unidadeConsumidoraUbs = new UnidadeConsumidora(
                0,
                "UC-776655",
                ubs.Id,
                tipoDespesaManutencao.Id,
                secretariaSaude.IdSecretaria,
                orcamentoUbs.IdOrcamento,
                fornecedorManutencao.IdFornecedor,
                false,
                null);

            await _context.UnidadesConsumidoras.AddRangeAsync([unidadeConsumidoraEscola, unidadeConsumidoraUbs], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var despesaEnergia = new Despesa(
                0,
                "NF-2026-0001",
                "1001",
                "Documento 1",
                "e64d18547b406ae3b00152234dc5327e800d8e7921142b6adcc0ffb8d3be7f3c",
                new DateOnly(2026, 4, 10),
                18450.75m,
                18450.75m,
                1320.50m,
                1320.50m,
                new DateOnly(2026, 4, 25),
                Status.PAGA,
                admin.Id,
                unidadeConsumidoraEscola.Id);

            var despesaManutencao = new Despesa(
                0,
                "OS-2026-0042",
                "2001",
                "Documento 2",
                "b08e9c91a71899cca2136f9caa6b04321361ed52f801cb6ff4c7d5128cf8be8e",
                new DateOnly(2026, 4, 15),
                9200.00m,
                0m,
                18m,
                0m,
                new DateOnly(2026, 5, 5),
                Status.A_PAGAR,
                funcionario.Id,
                unidadeConsumidoraUbs.Id);

            await _context.Despesas.AddRangeAsync([despesaEnergia, despesaManutencao], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            

            var auditoriaAdmin = new Auditoria(
                0,
                "2026-04-22",
                "09:00:00",
                "INSERT",
                "Despesa",
                "{}",
                "{\"numeroDocumento\":\"NF-2026-0001\",\"status\":\"PAGA\"}",
                Situacao.ATIVO,
                admin.Id);

            var auditoriaFuncionario = new Auditoria(
                0,
                "2026-04-22",
                "10:15:00",
                "INSERT",
                "Despesa",
                "{}",
                "{\"numeroDocumento\":\"OS-2026-0042\",\"status\":\"A_PAGAR\"}",
                Situacao.ATIVO,
                funcionario.Id);

            await _context.Auditorias.AddRangeAsync([auditoriaAdmin, auditoriaFuncionario], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Seed de desenvolvimento concluido. Usuarios de teste: {AdminEmail} / {FuncionarioEmail}. Senha padrao: {Senha}",
                admin.Email,
                funcionario.Email,
                DevPassword);
        }

        private static byte[] CreateDocumentContent(string content)
        {
            return Encoding.UTF8.GetBytes(content);
        }
    }
}