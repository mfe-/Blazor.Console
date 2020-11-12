# Blazor.Console

The goal of this project is to make it easy to move existing console application to blazor web assembly.

[![Console.Extensions package in get-the-solution feed in Azure Artifacts](https://feeds.dev.azure.com/get-the-solution/_apis/public/Packaging/Feeds/3bf81259-ccfe-4071-b8f8-bb5f44d4a8fb/Packages/89bd64e7-4569-44b5-946f-6830e6ac5694/Badge)](https://dev.azure.com/get-the-solution/get-the-solution/_packaging?_a=package&feed=3bf81259-ccfe-4071-b8f8-bb5f44d4a8fb&package=89bd64e7-4569-44b5-946f-6830e6ac5694&preferRelease=true)
[![Blazor.Console package in get-the-solution feed in Azure Artifacts](https://feeds.dev.azure.com/get-the-solution/_apis/public/Packaging/Feeds/3bf81259-ccfe-4071-b8f8-bb5f44d4a8fb/Packages/a528640f-a94c-48f1-acf8-feadc8c46001/Badge)](https://dev.azure.com/get-the-solution/get-the-solution/_packaging?_a=package&feed=3bf81259-ccfe-4071-b8f8-bb5f44d4a8fb&package=a528640f-a94c-48f1-acf8-feadc8c46001&preferRelease=true)

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


