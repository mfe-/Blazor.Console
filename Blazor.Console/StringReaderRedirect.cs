using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Console
{
    public class StringReaderRedirect : StringReader
    {
        private readonly Func<Task<string>> _ReadRedirectTaskFunc;
        private readonly Func<string> _ReadRedirectFunc;
        public StringReaderRedirect(Func<Task<string>> readredirect, Func<string> readTask) : base("foo")
        {
            _ReadRedirectTaskFunc = readredirect;
            _ReadRedirectFunc = readTask;
        }

        public override string ReadLine()
        {
            //Task.Yield();
            if(_ReadRedirectFunc!=null)
            {
                return _ReadRedirectFunc.Invoke();
            }
            else
            {
                Task<string> task = _ReadRedirectTaskFunc?.Invoke();
                return task?.ConfigureAwait(false).GetAwaiter().GetResult();
            }

            //return base.ReadLine();

            //return _ReadRedirectFunc?.Invoke();



            //return "asdf";
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
