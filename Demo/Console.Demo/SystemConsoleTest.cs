using System;
using System.Net.Http;
using System.Threading.Tasks;
namespace Blazor.Console.Demo
{
    public class SystemConsoleTest
    {
        public static async Task StartAsync()
        {
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://gorest.co.in/public-api/users/123/posts");
            var content = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine(content);

            System.Console.ForegroundColor = System.ConsoleColor.Red;

            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("  --h               show help.");
            System.Console.WriteLine("  --info            show system info.");
            System.Console.WriteLine("  --new             create a new instance.");
            System.Console.WriteLine();
            
            System.Console.WriteLine();
         
            System.Console.Write("first peace"); System.Console.Write("second peace");
            System.Console.WriteLine();
            System.Console.ResetColor();
            System.Console.WriteLine("Please enter the name of your cat!");
            //string v = console.ReadLine();
            //for blazor wasm you need to replace console.ReadLine with console.ReadLineAsync()
            string v = System.Console.ReadLine();
            System.Console.WriteLine($"The name of your cat is {v}");

        }
    }
}
