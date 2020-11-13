using System;
using System.IO;
using System.Threading.Tasks;

namespace Console.Extensions
{
    /// <summary>
    /// Redirects Stream
    /// </summary>
    /// <remarks>
    /// https://archive.codeplex.com/?p=consoleredirect
    /// </remarks>
    public class StringWriterRedirect : StringWriter
    {
        public Func<String, Task>? StringWriterLineRedirectTaskFunc;
        public Func<String, Task>? StringWriterRedirectTaskFunc;

        private Task WriteGeneric<T>(T value)
        {
            if (value == null) return Task.CompletedTask;
            return StringWriterRedirectTaskFunc?.Invoke(value.ToString());
        }

        public override async void Write(char value) { await WriteGeneric<char>(value); }
        public override async void Write(string value) { await WriteGeneric<string>(value); }
        public override async void Write(bool value) { await WriteGeneric<bool>(value); }
        public override async void Write(int value) { await WriteGeneric<int>(value); }
        public override async void Write(double value) { await WriteGeneric<double>(value); }
        public override async void Write(long value) { await WriteGeneric<long>(value); }

        private Task WriteLineGeneric<T>(T value)
        {
            if (value == null) return Task.CompletedTask;
            return StringWriterLineRedirectTaskFunc?.Invoke(value.ToString());
        }
        public override async void WriteLine(char value) { await WriteLineGeneric<char>(value); }
        public override async void WriteLine(string value) { await WriteLineGeneric<string>(value); }
        public override async void WriteLine(bool value) { await WriteLineGeneric<bool>(value); }
        public override async void WriteLine(int value) { await WriteLineGeneric<int>(value); }
        public override async void WriteLine(double value) { await WriteLineGeneric<double>(value); }
        public override async void WriteLine(long value) { await WriteLineGeneric<long>(value); }

        public override async void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            char[] buffer2 = new char[count]; //Ensures large buffers are not a problem
            for (int i = 0; i < count; i++) buffer2[i] = buffer[index + i];
            await WriteGeneric<char[]>(buffer2);
        }

        public override async void WriteLine(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            char[] buffer2 = new char[count]; //Ensures large buffers are not a problem
            for (int i = 0; i < count; i++) buffer2[i] = buffer[index + i];
            await WriteLineGeneric<char[]>(buffer2);
        }
    }
}
