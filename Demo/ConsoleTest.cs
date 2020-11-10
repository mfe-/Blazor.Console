using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Console.Demo
{
    public class ConsoleTest
    {
        public static Task StartAsync()
        {
            bool ask = false;
            do
            {
                System.Console.WriteLine("Please enter the name of your cat!");
                string v = System.Console.ReadLine();
                System.Console.WriteLine($"The name of your cat is {v}");
                if (v == "charly")
                {
                    ask = false;
                }
            }
            while (ask) ;

            System.Console.WriteLine("After ReadLine loop");

            return Task.CompletedTask;
        }
    }
}
