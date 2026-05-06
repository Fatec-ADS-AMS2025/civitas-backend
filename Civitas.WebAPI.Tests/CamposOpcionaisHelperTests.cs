using System.Text.Json;
using Civitas.WebAPI.Services.Validation;
using Xunit;

namespace Civitas.WebAPI.Tests;

public class CamposOpcionaisHelperTests
{
    [Fact]
    public void ParseCamposDeclarados_NullOrWhitespace_ReturnsEmpty()
    {
        Assert.Empty(CamposOpcionaisJsonHelper.ParseCamposDeclarados(null));
        Assert.Empty(CamposOpcionaisJsonHelper.ParseCamposDeclarados("   "));
    }

    [Fact]
    public void ParseCamposDeclarados_ValidEnvelope_ReturnsList()
    {
        var json = """{"camposOpcionais":["numeroNota","fornecedor","centroCusto"]}""";
        var result = CamposOpcionaisJsonHelper.ParseCamposDeclarados(json);
        Assert.Equal(new[] { "numeroNota", "fornecedor", "centroCusto" }, result);
    }

    [Fact]
    public void ParseCamposDeclarados_MissingEnvelopeKey_Throws()
    {
        var json = """{"outros":["a"]}""";
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados(json));
    }

    [Fact]
    public void ParseCamposDeclarados_NotAnArray_Throws()
    {
        var json = """{"camposOpcionais":"x"}""";
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados(json));
    }

    [Fact]
    public void ParseCamposDeclarados_DuplicateNames_Throws()
    {
        var json = """{"camposOpcionais":["a","A"]}""";
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados(json));
    }

    [Fact]
    public void ParseCamposDeclarados_EmptyName_Throws()
    {
        var json = """{"camposOpcionais":["", "valido"]}""";
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados(json));
    }

    [Fact]
    public void ParseCamposDeclarados_NonStringElement_Throws()
    {
        var json = """{"camposOpcionais":[123]}""";
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados(json));
    }

    [Fact]
    public void ParseCamposDeclarados_Malformed_ThrowsJsonException()
    {
        Assert.ThrowsAny<JsonException>(() => CamposOpcionaisJsonHelper.ParseCamposDeclarados("{not json"));
    }

    [Fact]
    public void SerializeCamposDeclarados_NullOrEmpty_ReturnsNull()
    {
        Assert.Null(CamposOpcionaisJsonHelper.SerializeCamposDeclarados(null));
        Assert.Null(CamposOpcionaisJsonHelper.SerializeCamposDeclarados(Array.Empty<string>()));
        Assert.Null(CamposOpcionaisJsonHelper.SerializeCamposDeclarados(new[] { " ", "" }));
    }

    [Fact]
    public void SerializeCamposDeclarados_ValidNames_ProducesEnvelope()
    {
        var json = CamposOpcionaisJsonHelper.SerializeCamposDeclarados(new[] { "numeroNota", "fornecedor" });
        Assert.NotNull(json);
        var parsed = CamposOpcionaisJsonHelper.ParseCamposDeclarados(json);
        Assert.Equal(new[] { "numeroNota", "fornecedor" }, parsed);
    }

    [Fact]
    public void SerializeCamposDeclarados_Duplicate_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            CamposOpcionaisJsonHelper.SerializeCamposDeclarados(new[] { "a", "A" }));
    }

    [Fact]
    public void ParseValoresPreenchidos_NullOrWhitespace_ReturnsEmpty()
    {
        Assert.Empty(CamposOpcionaisJsonHelper.ParseValoresPreenchidos(null));
        Assert.Empty(CamposOpcionaisJsonHelper.ParseValoresPreenchidos(""));
    }

    [Fact]
    public void ParseValoresPreenchidos_ValidObject_ReturnsDict()
    {
        var json = """{"numeroNota":"12345","centroCusto":null,"qtde":3}""";
        var result = CamposOpcionaisJsonHelper.ParseValoresPreenchidos(json);
        Assert.Equal(3, result.Count);
        Assert.Equal("12345", result["numeroNota"].GetString());
        Assert.Equal(JsonValueKind.Null, result["centroCusto"].ValueKind);
        Assert.Equal(3, result["qtde"].GetInt32());
    }

    [Fact]
    public void ParseValoresPreenchidos_NotAnObject_Throws()
    {
        Assert.Throws<ArgumentException>(() => CamposOpcionaisJsonHelper.ParseValoresPreenchidos("[1,2]"));
    }

    [Fact]
    public void EncontrarChavesDesconhecidas_AllKnown_ReturnsEmpty()
    {
        var unknown = CamposOpcionaisJsonHelper.EncontrarChavesDesconhecidas(
            new[] { "numeroNota", "fornecedor" },
            new[] { "numeroNota", "fornecedor", "centroCusto" });
        Assert.Empty(unknown);
    }

    [Fact]
    public void EncontrarChavesDesconhecidas_SomeUnknown_ReturnsThem()
    {
        var unknown = CamposOpcionaisJsonHelper.EncontrarChavesDesconhecidas(
            new[] { "numeroNota", "fantasma" },
            new[] { "numeroNota" });
        Assert.Equal(new[] { "fantasma" }, unknown);
    }

    [Fact]
    public void EncontrarChavesDesconhecidas_CaseSensitive()
    {
        var unknown = CamposOpcionaisJsonHelper.EncontrarChavesDesconhecidas(
            new[] { "NumeroNota" },
            new[] { "numeroNota" });
        Assert.Single(unknown);
    }
}
