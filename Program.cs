using System.Windows.Forms;
namespace calculadoraTarea;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new examen2.Interfaz());
    }
}
