using Console.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using console = Console.Extensions.Console;

namespace Blzr.Console
{
    public partial class BlazorConsoleComponent : ComponentBase, IDisposable
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput? Command { get; set; }
        protected string Output = string.Empty;
        protected string Placeholder { get; set; } = "Wait for input request.";
        protected string? Disabled { get; set; } = DisabledString;
        public static readonly string? DisabledString = null;// "Disabled";
        public event EventHandler<string>? ConsoleInputEvent;

        [Parameter]
        public bool ShowRepositoryUrl { get; set; } = true;
        [Parameter]
        public string? Name { get; set; }
        [Parameter]
        public bool UseOriginalSystemConsole { get; set; } = false;
        public string? Version() => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();

        protected StringWriterRedirect? _stringWriterRedirect;
        protected StringReaderRedirect? _stringReaderRedirect;
        private Timer? _Timer;
        protected override void OnInitialized()
        {
            //refresh ui every 800ms
            _Timer = new Timer(_ => InvokeAsync(StateHasChanged), null, 800, 800);
            ConsoleInputEvent += BlazorConsoleComponent_CommandInputEvent;
            base.OnInitialized();
        }
        [Parameter]
        public bool SetAutoFocusToConsoleInput { get; set; } = true;
        /// <summary>
        /// Invoked when the component is initialized after having received its initial parameters 
        /// from its parent component in <seealso cref="OnParametersSetAsync"/>
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            if (!UseOriginalSystemConsole)
            {
                console.ReadRedirectTaskFunc = ReadLineAsync;
                console.StringWriterLineRedirectTaskFunc = WriteLineAsync;
                console.StringWriterRedirectTaskFunc = WriteAsync;
                console.OnForegroundColorChanged = OnForegroundColorChanged;
            }
            else
            {
                _stringReaderRedirect = new StringReaderRedirect(null, ReadLine);
                System.Console.SetIn(_stringReaderRedirect);
                _stringWriterRedirect = new StringWriterRedirect()
                {
                    StringWriterLineRedirectTaskFunc = WriteLineAsync,
                    StringWriterRedirectTaskFunc = WriteAsync
                };
                System.Console.SetOut(_stringWriterRedirect);
            }


            Command = new ConsoleInput();

            await base.OnInitializedAsync();
        }
        public ConsoleColor CurrentConsoleColor { get; set; } = ConsoleColor.Gray;
        protected void OnForegroundColorChanged(ConsoleColor consoleColor)
        {
            CurrentConsoleColor = consoleColor;
        }
        /// <summary>
        /// Event will be fired on user input.
        /// </summary>
        /// <param name="sender">The object which fired the event</param>
        /// <param name="e">The user input</param>
        private void BlazorConsoleComponent_CommandInputEvent(object sender, string e)
        {
            CommandTaskCompletionSource?.SetResult(e);
        }
        TaskCompletionSource<string>? CommandTaskCompletionSource = new TaskCompletionSource<string>();
        public async Task<string> ReadLineAsync()
        {
            CommandTaskCompletionSource = new TaskCompletionSource<string>();
            string result = "";
            await InvokeAsync(ToggleReadOnly);
            await InvokeAsync(async () =>
            {
                //wait until the user enterted the command
                result = await CommandTaskCompletionSource.Task;
                //display the users input
                await WriteLineAsync(result);
            });
            await InvokeAsync(ToggleReadOnly);
            CommandTaskCompletionSource = null;
            return result;
        }
        public string ReadLine()
        {
            if (JSRuntime is IJSInProcessRuntime inProcessRuntime)
            {
                return inProcessRuntime.Invoke<string>("BlazorConsole.readLine");
            }
            else throw new InvalidCastException("Could not cast JSRuntime to IJSInProcessRuntime");
        }
        /// <summary>
        /// Fires the <seealso cref="ConsoleInputEvent"/> event
        /// </summary>
        /// <param name="inputCommand">The user input</param>
        protected void OnConsoleInputEvent(string inputCommand)
        {
            ConsoleInputEvent?.Invoke(this, inputCommand);
        }
        /// <summary>
        /// Will be executed on user input
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected Task OnConsoleInputSubmit(EditContext context)
        {
            if (context?.Model is ConsoleInput consoleInput)
            {
                OnConsoleInputEvent(consoleInput.Text);
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
        [Parameter]
        public bool AutoScroll { get; set; } = true;
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        private bool isFirstUse = true;
        protected string ConsoleInputId = "consoleInput";
        private async Task WriteLinePrivate(string consoleInput, bool newline = true)
        {
            if (isFirstUse)
            {
                //when the app writes the first line make a break
                consoleInput = $"</br>{consoleInput}";
                isFirstUse = false;
            }

            consoleInput = $"<span class=\"{CurrentConsoleColor}\">{consoleInput}</span>";
            
            // TODO     support other ASCII characters:
            //      https://mailtrap.io/blog/nbsp/
            //      https://www.freeformatter.com/html-entities.html
            consoleInput = consoleInput
                .Replace(Environment.NewLine, "</br>")
                .Replace(" ", "&nbsp;");

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
            if(AutoScroll)
            {
                await JSRuntime.InvokeVoidAsync("BlazorConsole.scrollToBottom");
            }
            if (SetAutoFocusToConsoleInput)
            {
                await JSRuntime.InvokeVoidAsync("BlazorConsole.setFocusToElement", ConsoleInputId);
            }
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
