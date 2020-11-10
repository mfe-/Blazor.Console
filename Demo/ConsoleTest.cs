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
                System.Console.WriteLine("Geben Sie den Namen ihrer Katze ein!");


                System.Console.WriteLine("GasdfKatze ein!");

                string v = System.Console.ReadLine();
                System.Console.WriteLine(v);
                if (v == "charly")
                {
                    ask = false;
                }
                else
                {
                    
                }
            }
            while (ask) ;
            return Task.CompletedTask;
        }
    }
}
