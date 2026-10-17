using Mastermind.Entities.Models;

namespace Mastermind.BusinessLogic.Services;

public class EvaluadorMastermind
{
    public ResultadoIntento EvaluarIntento(
        string patronSecreto,
        string intentoJugador)
    {
        if (string.IsNullOrWhiteSpace(patronSecreto) ||
            string.IsNullOrWhiteSpace(intentoJugador))
        {
            throw new ArgumentException("Los valores son obligatorios");
        }

        if (patronSecreto.Length != intentoJugador.Length)
        {
            throw new ArgumentException(
                "El patron y el intento deben tener el mismo tamaño"
            );
        }

        patronSecreto = patronSecreto.ToUpperInvariant();
        intentoJugador = intentoJugador.ToUpperInvariant();

        int bienPosicionados = 0;
        int colorCorrecto = 0;

        Dictionary<char, int> coloresRestantes = new();

        // Primero contamos los colores bien posicionados
        for (int i = 0; i < patronSecreto.Length; i++)
        {
            if (patronSecreto[i] == intentoJugador[i])
            {
                bienPosicionados++;
                continue;
            }

            char color = patronSecreto[i];

            coloresRestantes[color] =
                coloresRestantes.GetValueOrDefault(color) + 1;
        }

        // Luego contamos los colores en posiciones incorrectas
        for (int i = 0; i < intentoJugador.Length; i++)
        {
            if (patronSecreto[i] == intentoJugador[i])
                continue;

            char color = intentoJugador[i];

            if (coloresRestantes.GetValueOrDefault(color) > 0)
            {
                colorCorrecto++;
                coloresRestantes[color]--;
            }
        }

        return new ResultadoIntento
        {
            BienPosicionados = bienPosicionados,
            ColorCorrecto = colorCorrecto
        };
    }
}