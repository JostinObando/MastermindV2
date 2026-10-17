using Mastermind.BusinessLogic.Services;

Console.WriteLine("==================================");
Console.WriteLine("          MASTERMIND V2");
Console.WriteLine("==================================");

Console.Write("\nIngrese el patron secreto: ");
string patron = Console.ReadLine() ?? string.Empty;

Console.Write("Ingrese el intento del jugador: ");
string intento = Console.ReadLine() ?? string.Empty;

try
{
    var evaluador = new EvaluadorMastermind();

    var resultado = evaluador.EvaluarIntento(
        patron,
        intento
    );

    Console.WriteLine("\n------ RESULTADO ------");
    Console.WriteLine($"Bien posicionados: {resultado.BienPosicionados}");
    Console.WriteLine($"Color correcto: {resultado.ColorCorrecto}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}