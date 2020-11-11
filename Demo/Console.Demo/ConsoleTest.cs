using System.Threading.Tasks;
using console = Console.Extensions.Console;
namespace Blazor.Console.Demo
{
    public class ConsoleTest
    {
        public static async Task StartAsync()
        {
            console.WriteLine("Please enter the name of your cat!");
            //string v = console.ReadLine();
            //for blazor wasm you need to replace console.ReadLine with console.ReadLineAsync()
            string v = await console.ReadLineAsync();
            console.WriteLine($"The name of your cat is {v}");
        }
    }
}
