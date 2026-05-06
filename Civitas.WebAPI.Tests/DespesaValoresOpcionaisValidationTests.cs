using System.Text.Json;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Validation;
using Xunit;

namespace Civitas.WebAPI.Tests;

public class DespesaValoresOpcionaisValidationTests
{
    [Fact]
    public void ValoresOpcionaisNull_NaoAdicionaErro()
    {
        var dto = new DespesaDTO { ValoresOpcionais = null };
        var tipo = TipoDespesaCom(new[] { "campo1" });
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValoresOpcionaisVazio_NaoAdicionaErro()
    {
        var dto = new DespesaDTO { ValoresOpcionais = new Dictionary<string, JsonElement>() };
        var tipo = TipoDespesaCom(new[] { "campo1" });
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void TipoDespesaNull_AdicionaErroEspecifico()
    {
        var dto = ValoresDTO(("campo1", "valor"));
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, null, errors);

        Assert.Single(errors);
        Assert.Contains("sem TipoDespesa associado", errors[0]);
    }

    [Fact]
    public void TipoDespesaSemCamposDeclarados_RejeitaValores()
    {
        var dto = ValoresDTO(("campo1", "valor"));
        var tipo = TipoDespesaCom(Array.Empty<string>());
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Single(errors);
        Assert.Contains("não declara nenhum campo opcional", errors[0]);
    }

    [Fact]
    public void TipoDespesaCamposCorrompidos_AdicionaErroCorrompidos()
    {
        var dto = ValoresDTO(("campo1", "valor"));
        var tipo = new TipoDespesa(1, "Teste", SolicitaUc.Não, Situacao.ATIVO)
        {
            CamposOpcionais = "{not valid json"
        };
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Single(errors);
        Assert.Contains("corrompidos", errors[0]);
    }

    [Fact]
    public void TodasChavesDeclaradas_NaoAdicionaErro()
    {
        var dto = ValoresDTO(("numeroNota", "12345"), ("fornecedor", "Papelaria"));
        var tipo = TipoDespesaCom(new[] { "numeroNota", "fornecedor", "centroCusto" });
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ChavesDesconhecidas_ListaTodasNaMensagem()
    {
        var dto = ValoresDTO(("numeroNota", "12345"), ("fantasma", "x"), ("outra", "y"));
        var tipo = TipoDespesaCom(new[] { "numeroNota" });
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Single(errors);
        Assert.Contains("fantasma", errors[0]);
        Assert.Contains("outra", errors[0]);
    }

    [Fact]
    public void ChaveCaseSensitiveDistinta_TratadaComoDesconhecida()
    {
        var dto = ValoresDTO(("NumeroNota", "12345"));
        var tipo = TipoDespesaCom(new[] { "numeroNota" });
        var errors = new List<string>();

        DespesaService.ValidarValoresOpcionais(dto, tipo, errors);

        Assert.Single(errors);
        Assert.Contains("NumeroNota", errors[0]);
    }

    private static TipoDespesa TipoDespesaCom(IEnumerable<string> campos)
    {
        return new TipoDespesa(1, "Teste", SolicitaUc.Não, Situacao.ATIVO)
        {
            CamposOpcionais = CamposOpcionaisJsonHelper.SerializeCamposDeclarados(campos)
        };
    }

    private static DespesaDTO ValoresDTO(params (string Key, string Value)[] pares)
    {
        var dict = new Dictionary<string, JsonElement>();
        foreach (var (k, v) in pares)
        {
            dict[k] = JsonDocument.Parse($"\"{v}\"").RootElement.Clone();
        }
        return new DespesaDTO { ValoresOpcionais = dict };
    }
}
