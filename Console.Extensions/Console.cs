using System;
using System.Threading.Tasks;

namespace Console.Extensions
{
    public static class Console
    {
        static Console()
        {
            //set default redirecter to System.Console.Write/ReadLine
            StringWriterLineRedirectTaskFunc = new System.Func<string, Task>((s) =>
            {
                System.Console.WriteLine(s);
                return Task.CompletedTask;
            });
            StringWriterRedirectTaskFunc = new System.Func<string, Task>((s) =>
            {
                System.Console.Write(s);
                return Task.CompletedTask;
            });
            ReadRedirectTaskFunc = new System.Func<Task<string>>(() =>
            {
                var s = System.Console.ReadLine();
                return Task.FromResult(s);
            });

        }



        public static Func<Task<string>>? ReadRedirectTaskFunc
        {
            get { return In.ReadRedirectTaskFunc; }
            set { In.ReadRedirectTaskFunc = value; }
        }
        public static Func<String, Task>? StringWriterLineRedirectTaskFunc
        {
            get { return Out.StringWriterLineRedirectTaskFunc; }
            set { Out.StringWriterLineRedirectTaskFunc = value; }
        }
        public static Func<String, Task>? StringWriterRedirectTaskFunc
        {
            get { return Out.StringWriterRedirectTaskFunc; }
            set { Out.StringWriterRedirectTaskFunc = value; }
        }
        public static Action<ConsoleColor> OnForegroundColorChanged { get; set; }
        public static Action? OnClearAction { get; set; }
        //
        // Summary:
        //     Gets the standard input stream.
        //
        // Returns:
        //     A System.IO.TextReader that represents the standard input stream.
        public static StringReaderRedirect In { get; } = new StringReaderRedirect(null, null);
        public static StringWriterRedirect Out { get; } = new StringWriterRedirect();

        public static void Write(string text)
        {
            Out.Write(text);
        }
        public static void WriteLine()
        {
            Out.WriteLine("");
        }
        public static void WriteLine(string text)
        {
            Out.WriteLine(text);
        }
        public static void WriteLine(Exception text)
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
        public static void ResetColor()
        {
            try
            {
                ForegroundColor = ConsoleColor.Gray;
            }
            catch(PlatformNotSupportedException)
            {
                //nothing we can do here about this
            }
        }
        private static ConsoleColor _ForegroundColor;
        public static ConsoleColor ForegroundColor
        {
            get { return _ForegroundColor; }
            set
            {
                _ForegroundColor = value;
                try
                {
                    System.Console.ForegroundColor = _ForegroundColor;
                }
                catch (PlatformNotSupportedException)
                {
                    //nothing we can do here about this
                }
                OnForegroundColorChanged?.Invoke(_ForegroundColor);
            }
        }
        public static void Clear()
        {
            try
            {
                System.Console.Clear();
            }
            catch (PlatformNotSupportedException)
            {
                //nothing we can do here about this
            }
            OnClearAction?.Invoke();
        }

    }
}
