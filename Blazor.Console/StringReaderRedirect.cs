using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Console
{
    public class StringReaderRedirect : StringReader
    {
        private readonly Func<Task<string>> _ReadRedirectFunc;
        public StringReaderRedirect(Func<Task<string>> readredirect) : base("foo")
        {
            _ReadRedirectFunc = readredirect;
        }

        public override string ReadLine()
        {
            //return _ReadRedirectFunc?.Invoke();
            //return base.ReadLine();
            Task<string> task = _ReadRedirectFunc?.Invoke();

            return task?.GetAwaiter().GetResult();
        }
        public Task<string> GetString()
        {
            return Task.FromResult("okay);");
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
        public override Task<string> ReadLineAsync()
        {
            return base.ReadLineAsync();
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
