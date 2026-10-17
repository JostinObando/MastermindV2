namespace Mastermind.Entities.Models;

public class ResultadoIntento
{
    public int BienPosicionados { get; set; }
    public int ColorCorrecto { get; set; }
    public string MensajeCheat { get; set; } = string.Empty;
    public int? PosicionRevelada { get; set; }
    public string? ColorRevelado { get; set; }
}