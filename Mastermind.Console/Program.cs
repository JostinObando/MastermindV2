using Mastermind.BusinessLogic.Services;
using Mastermind.DataAccess.Repositories;

string rutaBase = AppContext.BaseDirectory;
string rutaConfiguracion = Path.Combine(rutaBase, "Configuracion");
string rutaRegistro = Path.Combine(rutaBase, "Registros", "mastermind.log");

var configuracionRepository = new ConfiguracionRepository(rutaConfiguracion);
var registroRepository = new RegistroRepository(rutaRegistro);

bool salir = false;

while (!salir)
{
    Console.Clear();

    Console.WriteLine("            MASTERMIND V2");
    Console.WriteLine();
    Console.WriteLine("1. Nueva partida");
    Console.WriteLine("2. Consultar colores disponibles");
    Console.WriteLine("3. Consultar historial");
    Console.WriteLine("4. Salir");
    Console.WriteLine();
    Console.Write("Seleccione una opcion: ");

    string opcion = Console.ReadLine() ?? string.Empty;

    switch (opcion)
    {
        case "1":
            IniciarPartida();
            break;

        case "2":
            MostrarColores();
            break;

        case "3":
            MostrarHistorial();
            break;

        case "4":
            salir = true;
            break;

        default:
            Console.WriteLine("Opcion invalida");
            Pausar();
            break;
    }
}

void IniciarPartida()
{
    Console.Clear();
    Console.WriteLine(" NUEVA PARTIDA");

    MostrarColores(false);

    Console.Write("\nIngrese el patron secreto (4 o 6 letras): ");
    string patron = (Console.ReadLine() ?? string.Empty).Trim();

    var evaluador = new EvaluadorMastermind(
        configuracionRepository,
        registroRepository
    );

    bool continuar = true;

    while (continuar)
    {
        Console.Write("\nIngrese su intento: ");
        string intento = (Console.ReadLine() ?? string.Empty).Trim();

        Console.Write("Cheat code (Enter para omitir): ");
        string? cheat = Console.ReadLine();

        try
        {
            var resultado = evaluador.EvaluarIntento(
                patron,
                intento,
                cheat
            );

            Console.WriteLine("\n RESULTADO ");
            Console.WriteLine($"Bien posicionados: {resultado.BienPosicionados}");
            Console.WriteLine($"Color correcto: {resultado.ColorCorrecto}");

            if (!string.IsNullOrWhiteSpace(resultado.MensajeCheat))
                Console.WriteLine($"Cheat: {resultado.MensajeCheat}");

            if (resultado.PosicionRevelada.HasValue)
            {
                Console.WriteLine($"Posicion: {resultado.PosicionRevelada}");
                Console.WriteLine($"Color revelado: {resultado.ColorRevelado}");
            }

            if (resultado.BienPosicionados == patron.Length)
            {
                Console.WriteLine("\nFelicidades, adivinaste el patron");
                continuar = false;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        if (continuar)
        {
            Console.Write("\n¿Realizar otro intento? (S/N): ");
            continuar = Console.ReadLine()?.Trim().ToUpperInvariant() == "S";
        }
    }

    Pausar();
}

void MostrarColores(bool pausar = true)
{
    Console.WriteLine("\nColores disponibles:");

    var configuracion = configuracionRepository.ObtenerConfiguracion();

    foreach (string color in configuracion.Colores)
        Console.Write($"{color} ");

    Console.WriteLine();

    if (pausar)
        Pausar();
}

void MostrarHistorial()
{
    Console.Clear();
    Console.WriteLine(" HISTORIAL ");

    if (!File.Exists(rutaRegistro))
    {
        Console.WriteLine("No existen evaluaciones registradas");
    }
    else
    {
        foreach (string registro in File.ReadAllLines(rutaRegistro))
            Console.WriteLine(registro);
    }

    Pausar();
}

void Pausar()
{
    Console.WriteLine("\nPresione Enter para continuar...");
    Console.ReadLine();
}