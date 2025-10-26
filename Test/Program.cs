using System.Threading.Tasks;
using MinecraftLauncherLinrary;

namespace Test
{
    internal class Program
    {
        static void Main(String[] args)
        {
            foreach (var item in JavaUtils.ScanJavaInstances())
            {
                Console.WriteLine($"{item.HomePath} {Environment.NewLine}\t Vonder:{item.Vendor} {Environment.NewLine}\t MajorVersion:{item.MajorVersion} {Environment.NewLine}\t Version:{item.Version}");
            }
        }
    }
}
