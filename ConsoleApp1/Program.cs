using System;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logic();
        }

        public static void Logic()
        {
            Console.WriteLine("Geben Sie was ein:");

            string v = Console.ReadLine();

            Console.WriteLine(v);

        }
    }
}
