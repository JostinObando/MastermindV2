using Mastermind.BusinessLogic.Services;

namespace Mastermind.Tests;

public class EvaluadorMastermindTests
{
    [Fact]
    public void CoincidenciaCompleta()
    {
        var evaluador = new EvaluadorMastermind();

        var resultado = evaluador.EvaluarIntento("RGBY", "RGBY");

        Assert.Equal(4, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void CoincidenciaParcial()
    {
        var evaluador = new EvaluadorMastermind();

        var resultado = evaluador.EvaluarIntento("RGBY", "RBYG");

        Assert.Equal(1, resultado.BienPosicionados);
        Assert.Equal(3, resultado.ColorCorrecto);
    }

    [Fact]
    public void NingunaCoincidencia()
    {
        var evaluador = new EvaluadorMastermind();

        var resultado = evaluador.EvaluarIntento("RGBY", "CCWW");

        Assert.Equal(0, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void ManejoDeDuplicados()
    {
        var evaluador = new EvaluadorMastermind();

        var resultado = evaluador.EvaluarIntento("RRGB", "RRRR");

        Assert.Equal(2, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }
}