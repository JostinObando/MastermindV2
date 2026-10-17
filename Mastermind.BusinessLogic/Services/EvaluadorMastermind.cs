using Mastermind.BusinessLogic.Interfaces;
using Mastermind.DataAccess.Interfaces;
using Mastermind.Entities.Models;

namespace Mastermind.BusinessLogic.Services;

public class EvaluadorMastermind : IEvaluadorMastermind
{
    private readonly IRegistroRepository registroRepository;
    private readonly HashSet<char> coloresValidos;
    private readonly Dictionary<string, int> cheatCodes;
    private readonly Func<int, int> seleccionarPosicion;

    private bool cheatUtilizado;

    public EvaluadorMastermind()
        : this(new ConfiguracionPredeterminada(), new RegistroNulo())
    {
    }

    public EvaluadorMastermind(
        IConfiguracionRepository configuracionRepository,
        IRegistroRepository registroRepository,
        Func<int, int>? seleccionarPosicion = null)
    {
        this.registroRepository = registroRepository;
        this.seleccionarPosicion = seleccionarPosicion ?? Random.Shared.Next;

        ConfiguracionJuego configuracion =
            configuracionRepository.ObtenerConfiguracion();

        if (configuracion.Colores == null ||
            configuracion.Colores.Count == 0 ||
            configuracion.Colores.Any(color =>
                string.IsNullOrWhiteSpace(color) ||
                color.Length != 1))
        {
            throw new InvalidOperationException(
                "La configuracion de colores es invalida"
            );
        }

        coloresValidos = configuracion.Colores
            .Select(color => char.ToUpperInvariant(color[0]))
            .ToHashSet();

        if (coloresValidos.Count != configuracion.Colores.Count)
        {
            throw new InvalidOperationException(
                "Existen colores duplicados"
            );
        }

        List<CodigoCheat> codigos =
            configuracionRepository.ObtenerCheatCodes();

        if (codigos.Any(codigo =>
            string.IsNullOrWhiteSpace(codigo.Codigo) ||
            codigo.IntentosDisponibles < 0))
        {
            throw new InvalidOperationException(
                "La configuracion de cheat codes es invalida"
            );
        }

        cheatCodes = codigos.ToDictionary(
            codigo => codigo.Codigo.Trim().ToUpperInvariant(),
            codigo => codigo.IntentosDisponibles
        );
    }

    public ResultadoIntento EvaluarIntento(
        string patronSecreto,
        string intentoJugador,
        string? cheatCode = null)
    {
        ValidarEntradas(patronSecreto, intentoJugador);

        patronSecreto = patronSecreto.ToUpperInvariant();
        intentoJugador = intentoJugador.ToUpperInvariant();

        ResultadoIntento resultado = CalcularResultado(
            patronSecreto,
            intentoJugador
        );

        if (!string.IsNullOrWhiteSpace(cheatCode))
        {
            AplicarCheat(
                patronSecreto,
                intentoJugador,
                cheatCode,
                resultado
            );
        }

        registroRepository.Registrar(
            patronSecreto,
            intentoJugador,
            resultado.BienPosicionados,
            resultado.ColorCorrecto
        );

        return resultado;
    }

    private void ValidarEntradas(string patron, string intento)
    {
        if (string.IsNullOrWhiteSpace(patron) ||
            string.IsNullOrWhiteSpace(intento))
        {
            throw new ArgumentException(
                "El patron y el intento son obligatorios"
            );
        }

        if (patron.Length != intento.Length ||
            (patron.Length != 4 && patron.Length != 6))
        {
            throw new ArgumentException(
                "El patron y el intento deben tener 4 o 6 colores"
            );
        }

        if (patron.ToUpperInvariant().Any(color =>
                !coloresValidos.Contains(color)) ||
            intento.ToUpperInvariant().Any(color =>
                !coloresValidos.Contains(color)))
        {
            throw new ArgumentException(
                "Se encontraron colores no permitidos"
            );
        }
    }

    private static ResultadoIntento CalcularResultado(
        string patron,
        string intento)
    {
        int bienPosicionados = 0;
        int colorCorrecto = 0;

        Dictionary<char, int> restantes = new();

        for (int i = 0; i < patron.Length; i++)
        {
            if (patron[i] == intento[i])
            {
                bienPosicionados++;
                continue;
            }

            restantes[patron[i]] =
                restantes.GetValueOrDefault(patron[i]) + 1;
        }

        for (int i = 0; i < intento.Length; i++)
        {
            if (patron[i] == intento[i])
                continue;

            if (restantes.GetValueOrDefault(intento[i]) > 0)
            {
                colorCorrecto++;
                restantes[intento[i]]--;
            }
        }

        return new ResultadoIntento
        {
            BienPosicionados = bienPosicionados,
            ColorCorrecto = colorCorrecto
        };
    }

    private void AplicarCheat(
        string patron,
        string intento,
        string codigo,
        ResultadoIntento resultado)
    {
        codigo = codigo.Trim().ToUpperInvariant();

        if (cheatUtilizado)
        {
            resultado.MensajeCheat =
                "Ya utilizo un cheat en esta partida";
            return;
        }

        if (!cheatCodes.TryGetValue(codigo, out int disponibles))
        {
            resultado.MensajeCheat = "Cheat code invalido";
            return;
        }

        if (disponibles <= 0)
        {
            resultado.MensajeCheat =
                "Cheat code sin intentos disponibles";
            return;
        }

        int posicion = seleccionarPosicion(patron.Length);

        if (posicion < 0 || posicion >= patron.Length)
        {
            throw new InvalidOperationException(
                "La posicion seleccionada es invalida"
            );
        }

        cheatUtilizado = true;
        cheatCodes[codigo]--;

        if (patron[posicion] == intento[posicion])
        {
            resultado.MensajeCheat = "Sin suerte";
            return;
        }

        resultado.PosicionRevelada = posicion + 1;
        resultado.ColorRevelado = patron[posicion].ToString();
        resultado.MensajeCheat = "Cheat aplicado correctamente";
    }

    private class ConfiguracionPredeterminada : IConfiguracionRepository
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
            return new List<CodigoCheat>();
        }
    }

    private class RegistroNulo : IRegistroRepository
    {
        public void Registrar(
            string patronSecreto,
            string intentoJugador,
            int bienPosicionados,
            int colorCorrecto)
        {
        }
    }
}