using Mastermind.Entities.Models;

namespace Mastermind.DataAccess.Interfaces;

public interface IConfiguracionRepository
{
    ConfiguracionJuego ObtenerConfiguracion();
    List<CodigoCheat> ObtenerCheatCodes();
}