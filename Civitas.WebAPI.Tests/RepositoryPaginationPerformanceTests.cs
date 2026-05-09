using System.Diagnostics;
using System.Reflection;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Data.Repositories;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class RepositoryPaginationPerformanceTests
{
    [Fact]
    public async Task RepositoryPagination_IsFasterThanLoadingEveryFornecedor()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var setupContext = new AppDbContext(options))
        {
            await setupContext.Database.EnsureCreatedAsync();
            setupContext.Fornecedores.AddRange(CreateFornecedores(5000));
            await setupContext.SaveChangesAsync();
        }

        var repositoryType = typeof(FornecedorRepository);
        var paginationMethod = repositoryType.GetMethod("GetPage");

        Assert.NotNull(paginationMethod);

        var pageQuery = CreatePaginationQuery(paginationMethod!);

        await WarmUpAsync(options, paginationMethod!, pageQuery);

        var fullElapsed = await MeasureAsync(async () =>
        {
            await using var context = new AppDbContext(options);
            var repository = new FornecedorRepository(context);
            await repository.Get();
        });

        var pagedResult = await MeasureAsync(async () =>
        {
            await using var context = new AppDbContext(options);
            var repository = new FornecedorRepository(context);
            return await InvokeAsync(repository, paginationMethod!, pageQuery);
        });

        var pagedItems = (IEnumerable<object>)pagedResult.Result.GetType().GetProperty("Items")!.GetValue(pagedResult.Result)!;

        Assert.Equal(20, pagedItems.Count());
        Assert.True(
            pagedResult.Elapsed < fullElapsed,
            $"Consulta paginada demorou {pagedResult.Elapsed.TotalMilliseconds:F2} ms e a consulta completa {fullElapsed.TotalMilliseconds:F2} ms.");
    }

    private static async Task WarmUpAsync(
        DbContextOptions<AppDbContext> options,
        MethodInfo paginationMethod,
        object pageQuery)
    {
        await using var context = new AppDbContext(options);
        var repository = new FornecedorRepository(context);

        await repository.Get();
        await InvokeAsync(repository, paginationMethod, pageQuery);
    }

    private static async Task<TimeSpan> MeasureAsync(Func<Task> action)
    {
        var samples = new List<TimeSpan>();

        for (var index = 0; index < 3; index++)
        {
            var stopwatch = Stopwatch.StartNew();
            await action();
            stopwatch.Stop();
            samples.Add(stopwatch.Elapsed);
        }

        return TimeSpan.FromTicks((long)samples.Average(sample => sample.Ticks));
    }

    private static async Task<(TimeSpan Elapsed, object Result)> MeasureAsync(Func<Task<object>> action)
    {
        var samples = new List<TimeSpan>();
        object? result = null;

        for (var index = 0; index < 3; index++)
        {
            var stopwatch = Stopwatch.StartNew();
            result = await action();
            stopwatch.Stop();
            samples.Add(stopwatch.Elapsed);
        }

        return (TimeSpan.FromTicks((long)samples.Average(sample => sample.Ticks)), result!);
    }

    private static object CreatePaginationQuery(MethodInfo paginationMethod)
    {
        var paginationType = paginationMethod.GetParameters()[0].ParameterType;
        var paginationQuery = Activator.CreateInstance(paginationType)
            ?? throw new InvalidOperationException("Não foi possível criar a query de paginação.");

        paginationType.GetProperty("Page")?.SetValue(paginationQuery, 1);
        paginationType.GetProperty("Size")?.SetValue(paginationQuery, 20);
        paginationType.GetProperty("SortBy")?.SetValue(paginationQuery, "NomeFantasia");
        paginationType.GetProperty("SortDirection")?.SetValue(paginationQuery, "asc");

        return paginationQuery;
    }

    private static async Task<object> InvokeAsync(object repository, MethodInfo paginationMethod, object pageQuery)
    {
        var task = (Task)paginationMethod.Invoke(repository, [pageQuery])!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task)
            ?? throw new InvalidOperationException("A consulta paginada não retornou resultado.");
    }

    private static IEnumerable<Fornecedor> CreateFornecedores(int total)
    {
        for (var index = 1; index <= total; index++)
        {
            yield return new Fornecedor(
                index,
                $"Fornecedor {index:D5}",
                Situacao.ATIVO,
                $"{index:D14}",
                $"Fornecedor Razao {index:D5}",
                "Rua A",
                index.ToString(),
                "Centro",
                "00000000",
                "44999999999",
                $"fornecedor{index:D5}@example.com",
                "Cidade",
                "PR", 1);
        }
    }
}
