using System.Text.Json;
using Mastermind.DataAccess.Interfaces;
using Mastermind.Entities.Models;

namespace Mastermind.DataAccess.Repositories;

public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly string rutaConfiguracion;

    public ConfiguracionRepository(string rutaConfiguracion)
    {
        this.rutaConfiguracion = rutaConfiguracion;
    }

    public ConfiguracionJuego ObtenerConfiguracion()
    {
        string ruta = Path.Combine(rutaConfiguracion, "colores.json");
        string contenido = File.ReadAllText(ruta);

        return JsonSerializer.Deserialize<ConfiguracionJuego>(
            contenido,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? throw new InvalidOperationException("Configuracion invalida");
    }

    public List<CodigoCheat> ObtenerCheatCodes()
    {
        string ruta = Path.Combine(rutaConfiguracion, "cheatcodes.json");
        string contenido = File.ReadAllText(ruta);

        return JsonSerializer.Deserialize<List<CodigoCheat>>(
            contenido,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? throw new InvalidOperationException("Cheat codes invalidos");
    }
}