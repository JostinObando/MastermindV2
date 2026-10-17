namespace Mastermind.DataAccess.Interfaces;

public interface IRegistroRepository
{
    void Registrar(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionados,
        int colorCorrecto);
}