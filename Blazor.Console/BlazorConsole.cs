using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Console
{
    public partial class BlazorConsoleComponent : ComponentBase
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput Command { get; set; }
        protected string Output = string.Empty;
        protected string Placeholder { get; set; } = "Wait for input request.";
        protected string Disabled { get; set; } = DisabledString;
        public static readonly string DisabledString = "Disabled";
        [Parameter] public string Name { get; set; }
        public string Version() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected StringWriterRedirect _stringWriterRedirect;
        protected StringReaderRedirect _stringReaderRedirect;
        protected override Task OnInitializedAsync()
        {
            _stringWriterRedirect = new StringWriterRedirect() { OnWrite = WriteLine };
            System.Console.SetOut(_stringWriterRedirect);
            _stringReaderRedirect = new StringReaderRedirect(Read);
            System.Console.SetIn(_stringReaderRedirect);

            Command = new ConsoleInput();
            Disabled = DisabledString;

            return base.OnInitializedAsync();
        }
        TaskCompletionSource<string> StringReadTaskCompletionSource = new TaskCompletionSource<string>();
        public Task<String> Read()
        {
            DisableInput();


            ////StringReadTaskCompletionSource = new TaskCompletionSource<string>();
            ////var t = StringReadTaskCompletionSource.Task.ConfigureAwait(false);

            //while (String.IsNullOrEmpty(Command.Text))
            //{
            //    if (!String.IsNullOrEmpty(Command.Text))
            //    {
            //        break;
            //    }
            //    //System.Threading.Thread.Yield();
            //}


            ////string r = t.GetAwaiter().GetResult();
            //Disabled = DisabledString;
            //StateHasChanged();
            ////return r;
            return Task.FromResult("oh");
        }
        

        protected Task Execute(EditContext context)
        {
            if (context?.Model is ConsoleInput consoleInput)
            {
                //StringReadTaskCompletionSource.SetResult(consoleInput.Text);
                consoleInput.Text = string.Empty;
            }
            return Task.CompletedTask;
        }

        public Task WriteLine(string consoleInput)
        {
            string readLineText = consoleInput;
            _StringBuilder.AppendLine($"<br>{readLineText}");
            Output = _StringBuilder.ToString();
            DisableInput();
            //force rerender of component
            StateHasChanged();
            return Task.CompletedTask;
        }
        public void ToggleReadOnly()
        {
            if (Disabled == null)
            {
                DisableInput();
            }
            else
            {
                EnableInput();
            }
        }

        private void EnableInput()
        {
            Disabled = null;
            Placeholder = "Enter input.";
        }

        private void DisableInput()
        {
            Disabled = DisabledString;
            Placeholder = "Wait for input request.";
        }
    }
}
