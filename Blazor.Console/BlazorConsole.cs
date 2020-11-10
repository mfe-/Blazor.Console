using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Console
{
    public partial class BlazorConsoleComponent : ComponentBase, IDisposable
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput Command { get; set; }
        protected string Output = string.Empty;
        protected string Placeholder { get; set; } //= "Wait for input request.";
        protected string Disabled { get; set; } = null;// DisabledString;
        public static readonly string DisabledString = "Disabled";
        public event EventHandler<string> CommandInputEvent;

        [Parameter] public string Name { get; set; }
        public string Version() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected StringWriterRedirect _stringWriterRedirect;
        protected StringReaderRedirect _stringReaderRedirect;
        private Timer _Timer;
        protected override void OnInitialized()
        {
            //refresh ui every 800ms
            _Timer = new Timer(_ => InvokeAsync(StateHasChanged), null, 800, 800);
            CommandInputEvent += BlazorConsoleComponent_CommandInputEvent;
            base.OnInitialized();
        }

        protected override Task OnInitializedAsync()
        {
            _stringWriterRedirect = new StringWriterRedirect() { OnWrite = WriteLine };
            System.Console.SetOut(_stringWriterRedirect);
            _stringReaderRedirect = new StringReaderRedirect(ReadAsync, Read);
            System.Console.SetIn(_stringReaderRedirect);

            Command = new ConsoleInput();

            return base.OnInitializedAsync();
        }
        private bool RecievedInput = false;
        private void BlazorConsoleComponent_CommandInputEvent(object sender, string e)
        {
            //event will be fired on user input. the user input should be returned by the Read function
            RecievedInput = true;
        }
        private ManualResetEvent manualResetEvent;
        public string Read()
        {
            manualResetEvent = new ManualResetEvent(false);
            
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
            manualResetEvent.WaitOne(1000);

            //wait until user enterted command and return value.
            return "ok";
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private string ReadPrivate()
        {
            return "ok";
        }
        public async Task<string> ReadAsync()
        {
            string result = "";
            //await Task.Yield(); monitor exception
            await InvokeAsync(async () =>
            {
                //await Task.Yield();
                result = "muh";
            });
            return result;
        }
        private async Task<string> ReadPrivateAsync()
        {
            this.RecievedInput = false;
            string result = "";
            await InvokeAsync(async () =>
            {
                do
                {
                    result = "okfromloop";
                } while (this.RecievedInput);
            });
            return result;
        }


        //public async Task<String> ReadAsync()
        //{
        //    await InvokeAsync(ReadAsync);

        //    //await Task.Yield();
        //    ////DisableInput();

        //    //await InvokeAsync(() => Task.Delay(800));

        //    //////string r = t.GetAwaiter().GetResult();
        //    ////Disabled = DisabledString;
        //    ////StateHasChanged();
        //    //////return r;
        //    //return Command.Text + "1";

        //}

        protected void OnCommandInputEvent(string inputCommand)
        {
            CommandInputEvent?.Invoke(this, inputCommand);
        }
        protected Task Execute(EditContext context)
        {
            if (context?.Model is ConsoleInput consoleInput)
            {
                OnCommandInputEvent(consoleInput.Text);
                consoleInput.Text = string.Empty;
            }
            return Task.CompletedTask;
        }
        public Task WriteLine(string consoleInput)
        {
            return InvokeAsync(() => WriteLinePrivate(consoleInput));
        }
        private Task WriteLinePrivate(string consoleInput)
        {
            string readLineText = consoleInput;
            _StringBuilder.AppendLine($"<br>{readLineText}");
            Output = _StringBuilder.ToString();
            //DisableInput();
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

        public void Dispose()
        {
            _Timer?.Dispose();
            _Timer = null;
        }
    }
}
