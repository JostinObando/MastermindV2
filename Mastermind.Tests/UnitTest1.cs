using Mastermind.BusinessLogic.Services;
using Mastermind.DataAccess.Interfaces;
using Mastermind.Entities.Models;

namespace Mastermind.Tests;

public class EvaluadorMastermindTests
{
    private static EvaluadorMastermind CrearEvaluador(
        int posicionCheat = 0)
    {
        return new EvaluadorMastermind(
            new ConfiguracionPrueba(),
            new RegistroPrueba(),
            _ => posicionCheat
        );
    }

    [Fact]
    public void CoincidenciaCompleta()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("RGBY", "RGBY");

        Assert.Equal(4, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void CoincidenciaParcial()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("RGBY", "RBYG");

        Assert.Equal(1, resultado.BienPosicionados);
        Assert.Equal(3, resultado.ColorCorrecto);
    }

    [Fact]
    public void NingunaCoincidencia()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("RGBY", "CCWW");

        Assert.Equal(0, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void ManejoDeDuplicados()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("RRGB", "RRRR");

        Assert.Equal(2, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void DuplicadosEnPosicionesIncorrectas()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("RRGB", "BBRR");

        Assert.Equal(0, resultado.BienPosicionados);
        Assert.Equal(3, resultado.ColorCorrecto);
    }


    [Fact]
    public void ValidarTamanoIncorrecto()
    {
        var evaluador = CrearEvaluador();

        Assert.Throws<ArgumentException>(() =>
            evaluador.EvaluarIntento("RGB", "RGB")
        );
    }

    [Fact]
    public void ValidarTamanoDistinto()
    {
        var evaluador = CrearEvaluador();

        Assert.Throws<ArgumentException>(() =>
            evaluador.EvaluarIntento("RGBY", "RGBYCP")
        );
    }

    [Fact]
    public void ValidarColoresInvalidos()
    {
        var evaluador = CrearEvaluador();

        Assert.Throws<ArgumentException>(() =>
            evaluador.EvaluarIntento("RGBY", "RZBY")
        );
    }

    [Fact]
    public void ValidarEntradasVacias()
    {
        var evaluador = CrearEvaluador();

        Assert.Throws<ArgumentException>(() =>
            evaluador.EvaluarIntento("", "")
        );
    }

    [Fact]
    public void ValidarSeisColores()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento(
            "RGBYCP",
            "RGBYCP"
        );

        Assert.Equal(6, resultado.BienPosicionados);
        Assert.Equal(0, resultado.ColorCorrecto);
    }

    [Fact]
    public void ValidarMinusculas()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento("rgby", "RGBY");

        Assert.Equal(4, resultado.BienPosicionados);
    }



    [Fact]
    public void CheatValido()
    {
        var evaluador = CrearEvaluador(1);

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        Assert.Equal(
            "Cheat aplicado correctamente",
            resultado.MensajeCheat
        );

        Assert.Equal(2, resultado.PosicionRevelada);
        Assert.Equal("G", resultado.ColorRevelado);
    }

    [Fact]
    public void CheatRepetido()
    {
        var evaluador = CrearEvaluador(1);

        evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "AYUDA123"
        );

        Assert.Equal(
            "Ya utilizo un cheat en esta partida",
            resultado.MensajeCheat
        );
    }

    [Fact]
    public void CheatInutil()
    {
        var evaluador = CrearEvaluador(0);

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        Assert.Equal("Sin suerte", resultado.MensajeCheat);
        Assert.Null(resultado.PosicionRevelada);
        Assert.Null(resultado.ColorRevelado);
    }

    [Fact]
    public void CheatInutilSeConsume()
    {
        var evaluador = CrearEvaluador(0);

        evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        Assert.Equal(
            "Ya utilizo un cheat en esta partida",
            resultado.MensajeCheat
        );
    }

    [Fact]
    public void CheatInvalido()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "NOEXISTE"
        );

        Assert.Equal(
            "Cheat code invalido",
            resultado.MensajeCheat
        );
    }

    [Fact]
    public void CheatSinIntentosDisponibles()
    {
        var evaluador = CrearEvaluador();

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTERPRO"
        );

        Assert.Equal(
            "Cheat code sin intentos disponibles",
            resultado.MensajeCheat
        );
    }

    [Fact]
    public void CheatInvalidoNoConsume()
    {
        var evaluador = CrearEvaluador(1);

        evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "NOEXISTE"
        );

        var resultado = evaluador.EvaluarIntento(
            "RGBY",
            "RBYG",
            "MASTER2026"
        );

        Assert.Equal(
            "Cheat aplicado correctamente",
            resultado.MensajeCheat
        );
    }

    [Fact]
    public void RegistrarEvaluacion()
    {
        var registro = new RegistroPrueba();

        var evaluador = new EvaluadorMastermind(
            new ConfiguracionPrueba(),
            registro
        );

        evaluador.EvaluarIntento("RGBY", "RBYG");

        Assert.Equal(1, registro.CantidadRegistros);
        Assert.Equal("RGBY", registro.UltimoPatron);
        Assert.Equal("RBYG", registro.UltimoIntento);
        Assert.Equal(1, registro.BienPosicionados);
        Assert.Equal(3, registro.ColorCorrecto);
    }

    [Fact]
    public void EntradaInvalidaNoSeRegistra()
    {
        var registro = new RegistroPrueba();

        var evaluador = new EvaluadorMastermind(
            new ConfiguracionPrueba(),
            registro
        );

        Assert.Throws<ArgumentException>(() =>
            evaluador.EvaluarIntento("RGB", "RGB")
        );

        Assert.Equal(0, registro.CantidadRegistros);
    }


    private class ConfiguracionPrueba : IConfiguracionRepository
    {
        public ConfiguracionJuego ObtenerConfiguracion()
        {
            return new ConfiguracionJuego
            {
                Colores = new List<string>
                {
                    "Y", "C", "B", "P", "W",
                    "I", "R", "N", "G"
                }
            };
        }

        public List<CodigoCheat> ObtenerCheatCodes()
        {
            return new List<CodigoCheat>
            {
                new()
                {
                    Codigo = "MASTER2026",
                    IntentosDisponibles = 1
                },
                new()
                {
                    Codigo = "AYUDA123",
                    IntentosDisponibles = 2
                },
                new()
                {
                    Codigo = "MASTERPRO",
                    IntentosDisponibles = 0
                }
            };
        }
    }

   

    private class RegistroPrueba : IRegistroRepository
    {
        public int CantidadRegistros { get; private set; }
        public string UltimoPatron { get; private set; } = string.Empty;
        public string UltimoIntento { get; private set; } = string.Empty;
        public int BienPosicionados { get; private set; }
        public int ColorCorrecto { get; private set; }

        public void Registrar(
            string patronSecreto,
            string intentoJugador,
            int bienPosicionados,
            int colorCorrecto)
        {
            CantidadRegistros++;
            UltimoPatron = patronSecreto;
            UltimoIntento = intentoJugador;
            BienPosicionados = bienPosicionados;
            ColorCorrecto = colorCorrecto;
        }
    }
}