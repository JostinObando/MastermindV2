using Mastermind.DataAccess.Interfaces;

namespace Mastermind.DataAccess.Repositories;

public class RegistroRepository : IRegistroRepository
{
    private readonly string rutaArchivo;

    public RegistroRepository(string rutaArchivo)
    {
        this.rutaArchivo = rutaArchivo;
    }

    public void Registrar(
        string patronSecreto,
        string intentoJugador,
        int bienPosicionados,
        int colorCorrecto)
    {
        string registro =
            $"En la fecha {DateTime.Now:dd-MM-yyyy HH:mm:ss} " +
            $"se evaluó el intento {intentoJugador} contra el patrón " +
            $"{patronSecreto}. Resultado: {bienPosicionados} bien " +
            $"posicionados – {colorCorrecto} con color correcto.";

        string? directorio = Path.GetDirectoryName(rutaArchivo);

        if (!string.IsNullOrWhiteSpace(directorio))
            Directory.CreateDirectory(directorio);

        File.AppendAllText(rutaArchivo, registro + Environment.NewLine);
    }
}