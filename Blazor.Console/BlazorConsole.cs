using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Console
{
    public partial class BlazorConsoleComponent : ComponentBase
    {
        private readonly StringBuilder _StringBuilder = new StringBuilder();
        protected ConsoleInput Command { get; set; }
        protected string Output = string.Empty;
        protected string Placeholder { get; set; } = "Enter a command, type 'help' for avaliable commands.";
        protected string Disabled { get; set; } = null;
        [Parameter] public string Name { get; set; }
        public string Version() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        protected override Task OnInitializedAsync()
        {
            Command = new ConsoleInput();
            System.Console.SetOut(new StringWriterRedirect() { OnWrite = WriteLine });

            return base.OnInitializedAsync();
        }

        protected Task Execute(EditContext context)
        {
            Placeholder = "Please wait for command to be completed.";
            if(context?.Model is ConsoleInput consoleInput)
            {
                WriteLine(consoleInput.Text);
                consoleInput.Text = string.Empty;
            }
            return Task.CompletedTask;
        }
        
        public void WriteLine(string consoleInput)
        {
            string readLineText = consoleInput;
            _StringBuilder.AppendLine($"<br>{readLineText}");
            Output = _StringBuilder.ToString();
            //force rerender of component
            StateHasChanged();
        }
    }
}
