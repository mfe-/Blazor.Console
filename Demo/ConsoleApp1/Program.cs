using console = Console.Extensions.Console;

namespace ConsoleApp1
{
    //dont forget to make Program public
    public class Program
    {
        //dont forget to make Main public
        public static void Main(string[] args)
        {
            console.WriteLine("Hello World!");
            Logic();
        }

        public static async void Logic()
        {
            console.WriteLine("Please enter something:");
            //wasm blazor requires to replace ReadLine with ReadLineAsync
            string v = console.ReadLine();

            console.WriteLine($"You enterted {v}");

        }
    }
}
