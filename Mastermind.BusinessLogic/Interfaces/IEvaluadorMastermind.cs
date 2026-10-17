using Mastermind.Entities.Models;

namespace Mastermind.BusinessLogic.Interfaces;

public interface IEvaluadorMastermind
{
    ResultadoIntento EvaluarIntento(
        string patronSecreto,
        string intentoJugador,
        string? cheatCode = null);
}