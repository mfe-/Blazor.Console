# Blazor.Console

The goal of this project is to make it easy to move existing console application to blazor web assembly.

## Sample Demo



```csharp
using System;

namespace ConsoleApp1
{
    //dont forget to make Program public
    public class Program
    {
        //dont forget to make Main public
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logic();
        }

        public static void Logic()
        {
            Console.WriteLine("Please enter something:");

            string v = Console.ReadLine();

            System.Console.WriteLine($"You enterted {v}");

        }
    }
}
```

1. Exchange `using System;` to `using console = Console.Extensions.Console;`
2. Replace `Console.WriteLine` to `console.WriteLine`
3. Replace `string v = Console.ReadLine();` to `string v = await console.ReadLineAsync();`
4. Exchanging this function will still call the orignal methods of `System.Console` 

4. Create the razor page
5. Include the console component for example:

```razor
<p><button @onclick="OnStartConsoleAppClick">start console app</button></p>

<p><Blazor.Console.BlazorConsole Name="fooTest" @ref="c" /></p>


@code
{
    BlazorConsole c;

    private void OnStartConsoleAppClick(MouseEventArgs mouseEventArgs)
    {
        InvokeAsync(ConsoleTest.StartAsync).ContinueWith(a =>
        {
            if (a.Exception != null)
            {
                _logger.LogError(a.Exception, nameof(OnStartConsoleAppClick));
            }
        });
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }
}
```

done.


