using System;

namespace ConsoleApp1
{
    //dont forget to make Program public
    public class Program
    {
        //dont forget to make Main public
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logic();
        }

        public static void Logic()
        {
            Console.WriteLine("Please enter something:");

            string v = Console.ReadLine();

            System.Console.WriteLine($"You enterted {v}");

        }
    }
}
