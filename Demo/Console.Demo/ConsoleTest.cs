﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using console = Console.Extensions.Console;
namespace Blazor.Console.Demo
{
    public class ConsoleTest
    {
        public static async Task StartAsync()
        {
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://gorest.co.in/public-api/users/123/posts");
            var content = await response.Content.ReadAsStringAsync();
            console.WriteLine(content);

            console.ForegroundColor = System.ConsoleColor.Red;

            console.WriteLine();
            console.WriteLine("Options:");
            console.WriteLine("  --h               show help.");
            console.WriteLine("  --info            show system info.");
            console.WriteLine("  --new             create a new instance.");
            console.WriteLine();

            console.WriteLine();

            console.Write("first peace"); console.Write("second peace");
            console.WriteLine();
            console.ResetColor();
            console.WriteLine("Please enter the name of your cat!");
            //string v = console.ReadLine();
            //for blazor wasm you need to replace console.ReadLine with console.ReadLineAsync()
            string v = await console.ReadLineAsync();
            console.WriteLine($"The name of your cat is {v}");

        }
    }
}
