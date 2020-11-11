using System;
using System.Threading.Tasks;

namespace Console.Extensions
{
    public static class Console
    {
        static Console()
        {
            //set default redirecter to System.Console.Write/ReadLine
            StringWriterRedirectTaskFunc = new System.Func<string, Task>((s) => 
            { 
                System.Console.WriteLine(s); 
                return Task.CompletedTask; 
            });
            ReadRedirectTaskFunc = new System.Func<Task<string>>(() => 
            {
                var s = System.Console.ReadLine(); 
                return Task.FromResult(s); 
            });

        }
        public static Func<Task<string>> ReadRedirectTaskFunc
        {
            get { return In.ReadRedirectTaskFunc; }
            set { In.ReadRedirectTaskFunc = value; }
        }
        public static Func<String, Task> StringWriterRedirectTaskFunc
        {
            get { return Out.StringWriterRedirectTaskFunc; }
            set { Out.StringWriterRedirectTaskFunc = value; }
        }
        //
        // Summary:
        //     Gets the standard input stream.
        //
        // Returns:
        //     A System.IO.TextReader that represents the standard input stream.
        public static StringReaderRedirect In { get; } = new StringReaderRedirect(null, null);
        public static StringWriterRedirect Out { get; } = new StringWriterRedirect();

        public static void WriteLine(string text)
        {
            Out.WriteLine(text);
        }
        public static string ReadLine()
        {
            return In.ReadLine();
        }
        public static Task<string> ReadLineAsync()
        {
            return In.ReadLineAsync();
        }
    }
}
