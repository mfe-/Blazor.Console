using Console.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using console = Console.Extensions.Console;

namespace Blazor.Console
{
    public partial class BlazorConsoleComponent : ComponentBase, IDisposable
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput Command { get; set; }
        protected string Output = string.Empty;
        protected string Placeholder { get; set; } = "Wait for input request.";
        protected string Disabled { get; set; } = DisabledString;
        public static readonly string DisabledString = null;// "Disabled";
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
            console.ReadRedirectTaskFunc = ReadLineAsync;
            console.StringWriterLineRedirectTaskFunc = WriteLineAsync;
            console.StringWriterRedirectTaskFunc = WriteAsync;
            console.OnForegroundColorChanged = OnForegroundColorChanged;

            Command = new ConsoleInput();

            return base.OnInitializedAsync();
        }
        public ConsoleColor CurrentConsoleColor { get; set; }
        protected void OnForegroundColorChanged(ConsoleColor consoleColor)
        {
            CurrentConsoleColor = consoleColor;
        }
        private void BlazorConsoleComponent_CommandInputEvent(object sender, string e)
        {
            //event will be fired on user input.
            CommandTaskCompletionSource?.SetResult(e);
        }
        TaskCompletionSource<string> CommandTaskCompletionSource = new TaskCompletionSource<string>();
        public async Task<string> ReadLineAsync()
        {
            CommandTaskCompletionSource = new TaskCompletionSource<string>();
            string result = "";
            await InvokeAsync(ToggleReadOnly);
            await InvokeAsync(async () =>
            {
                //wait until the user enterted the command
                result = await CommandTaskCompletionSource.Task;
            });
            await InvokeAsync(ToggleReadOnly);
            CommandTaskCompletionSource = null;
            return result;
        }

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
        public Task WriteLineAsync(string consoleInput = "")
        {
            return InvokeAsync(() => WriteLinePrivate(consoleInput));
        }
        public Task WriteAsync(string consoleInput)
        {
            return InvokeAsync(() => WriteLinePrivate(consoleInput, false));
        }
        private bool isFirstUse = true;
        private Task WriteLinePrivate(string consoleInput, bool newline = true)
        {
            if (isFirstUse)
            {
                //when the app writes the first line make a break
                consoleInput = $"{consoleInput}{Environment.NewLine}";
                isFirstUse = false;
            }

            consoleInput = $"<span class=\"{CurrentConsoleColor}\">{consoleInput}</span>";
            if (newline)
            {
                _StringBuilder.AppendLine($"{consoleInput}</br>");
            }
            else
            {
                _StringBuilder.AppendLine(consoleInput);
            }
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
            StateHasChanged();
        }

        private void EnableInput()
        {
            //Disabled = null;
            Placeholder = "Enter input.";
        }

        private void DisableInput()
        {
            //Disabled = DisabledString;
            Placeholder = "Wait for input request.";
        }

        public void Dispose()
        {
            _Timer?.Dispose();
            _Timer = null;
            _stringWriterRedirect?.Dispose();
            _stringWriterRedirect = null;
            _stringReaderRedirect?.Dispose();
            _stringReaderRedirect = null;
        }
    }
}
