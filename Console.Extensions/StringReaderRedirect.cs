using System;
using System.IO;
using System.Threading.Tasks;

namespace Console.Extensions
{
    public class StringReaderRedirect : StringReader
    {
        public StringReaderRedirect(Func<Task<string>> readredirectAsync) : this(readredirectAsync,null)
        {
            ReadRedirectTaskFunc = readredirectAsync;
        }
        public StringReaderRedirect(Func<Task<string>>? readredirectAsync, Func<string>? readTask) : base("foo")
        {
            ReadRedirectTaskFunc = readredirectAsync;
            ReadRedirectFunc = readTask;
        }
        public Func<Task<string>>? ReadRedirectTaskFunc { get; set; }
        public Func<string>? ReadRedirectFunc { get; set; }

        public override string ReadLine()
        {
            if (ReadRedirectFunc != null)
            {
                return ReadRedirectFunc.Invoke();
            }
            //determine which platform is calling this method
            if ("web".Equals(System.Runtime.InteropServices.RuntimeInformation.OSDescription, StringComparison.OrdinalIgnoreCase))
            {
                throw new PlatformNotSupportedException("Please exchange the call console.ReadLine with await console.ReadLineAsync!");
            }
            if (ReadRedirectTaskFunc != null)
            {
                if (ReadRedirectTaskFunc == null) throw new ArgumentNullException($"Please set {nameof(ReadRedirectTaskFunc)}", nameof(ReadRedirectTaskFunc));
                Task<string> task = ReadRedirectTaskFunc.Invoke();
                return task.ConfigureAwait(false).GetAwaiter().GetResult();
            }
            throw new ArgumentNullException($"Please set {nameof(ReadRedirectTaskFunc)} or {nameof(ReadRedirectFunc)}");
        }
        public override Task<string> ReadLineAsync()
        {
            if (ReadRedirectTaskFunc == null) throw new ArgumentNullException($"Please set {nameof(ReadRedirectTaskFunc)}", nameof(ReadRedirectTaskFunc));
            return ReadRedirectTaskFunc.Invoke();
        }
        public override int Read()
        {
            return base.Read();
        }
        public override int Read(char[] buffer, int index, int count)
        {
            return base.Read(buffer, index, count);
        }
        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            return base.ReadAsync(buffer, index, count);
        }
        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return base.ReadBlock(buffer, index, count);
        }
        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            return base.ReadBlockAsync(buffer, index, count);
        }

        public override string ReadToEnd()
        {
            return base.ReadToEnd();
        }
        public override Task<string> ReadToEndAsync()
        {
            return base.ReadToEndAsync();
        }

    }
}
