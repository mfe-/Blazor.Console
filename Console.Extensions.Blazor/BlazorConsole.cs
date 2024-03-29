﻿using Console.Extensions;
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
    /// <summary>
    /// The component which will render the console terminal
    /// </summary>
    public partial class BlazorConsoleComponent : ComponentBase, IDisposable
    {
        private const string htmlWhiteSpace = "&nbsp;";
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput? Command { get; set; }
        internal string Output = string.Empty;
        protected string Placeholder { get; set; } = "Wait for input request.";
        protected string? Disabled { get; set; } = DisabledString;
        public static readonly string? DisabledString = null;
        public event EventHandler<string>? ConsoleInputEvent;
        /// <summary>
        /// Shows the github repository url of <see cref="BlazorConsoleComponent"/> when set to true. Set it to false to hide the github repository url for <see cref="BlazorConsoleComponent"/>
        /// </summary>
        [Parameter]
        public bool ShowRepositoryUrl { get; set; } = true;
        /// <summary>
        /// Get or sets a name for the Console which will be displayed when set.
        /// </summary>
        [Parameter]
        public string? Name { get; set; }
        /// <summary>
        /// Defines whether <see cref="console"/> should be used or <see cref="System.Console.WriteLine"/> (<see cref="System.Console.WriteLine"/> is experimental and under development). 
        /// </summary>
        [Parameter]
        public bool UseOriginalSystemConsole { get; set; } = false;
        /// <summary>
        /// Get or sets the value for <see cref="ReplaceWhiteString"/>. 
        /// If set to true any input string for <see cref="WriteAsync(string)"/> or <see cref="WriteLineAsync(string)"/> will replace" " by <see cref="htmlWhiteSpace"/> (&nbsp;). 
        /// </summary>
        [Parameter]
        public bool ReplaceWhiteString { get; set; } = false;
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
        private void BlazorConsoleComponent_CommandInputEvent(object? sender, string e)
        {
            CommandTaskCompletionSource?.SetResult(e);
        }
        TaskCompletionSource<string>? CommandTaskCompletionSource = new TaskCompletionSource<string>();
        /// <summary>
        /// Reads the input of the user in the web console from an input field
        /// </summary>
        /// <returns>the enterted string of the user</returns>
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
            throw new InvalidCastException("Could not cast JSRuntime to IJSInProcessRuntime");
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
        /// <summary>
        /// Generates the proper output for Console.Write or Console.WriteLine for the web browser
        /// </summary>
        /// <remarks>
        /// The generated output will be written to <see cref="Output"/>. 
        /// </remarks>
        /// <param name="consoleInput"></param>
        /// <param name="newline"></param>
        /// <returns></returns>
        private async Task WriteLinePrivate(string consoleInput, bool newline = true)
        {
            if (ReplaceWhiteString)
            {
                consoleInput = consoleInput.Replace(" ", htmlWhiteSpace);
            }
            if (isFirstUse)
            {
                //when the app writes the first line make a break
                consoleInput = $"</br>{consoleInput}";
                isFirstUse = false;
            }

            consoleInput = $"<span class=\"{CurrentConsoleColor}\">{consoleInput}</span>";
            consoleInput = consoleInput
              .Replace(Environment.NewLine, "</br>");

            if (newline)
            {
                _StringBuilder.AppendLine($"{consoleInput}</br>");
            }
            else
            {
                _StringBuilder.AppendLine(consoleInput);
            }
            Output = _StringBuilder.ToString();

            //force rerender of component
            StateHasChanged();
            if (AutoScroll && JSRuntime is IJSRuntime)
            {
                await JSRuntime.InvokeVoidAsync("BlazorConsole.scrollToBottom");
            }
            if (SetAutoFocusToConsoleInput && JSRuntime is IJSRuntime)
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
            Placeholder = "Enter input.";
        }

        private void DisableInput()
        {
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
