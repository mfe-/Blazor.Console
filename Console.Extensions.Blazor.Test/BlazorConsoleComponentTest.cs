using Blzr.Console;
using Bunit;
using Xunit;

namespace Blzr.Console.Test;

public class BlazorConsoleComponentTest
{
    [Fact]
    public void WriteLineAsync_with_ReplaceWhiteString_should_use_htmlnbsp()
    {
        string expectedOutput = "<span class=\"Gray\"></br>This&nbsp;is&nbsp;a&nbsp;test&nbsp;string</span></br>\r\n";

        using var ctx = new TestContext();
        var blazorConsoleComponentContext = ctx.RenderComponent<BlazorConsoleComponent>(parameters => parameters.Add(c => c.ReplaceWhiteString, true));

        blazorConsoleComponentContext.Instance.WriteLineAsync("This is a test string");

        Assert.Equal(expectedOutput, blazorConsoleComponentContext.Instance.Output);
    }
    [Fact]
    public void WriteLineAsync_should_produce_expected_output()
    {
        string expectedOutput = "<span class=\"Gray\"></br>This is a test string</span></br>\r\n";

        using var ctx = new TestContext();
        var blazorConsoleComponentContext = ctx.RenderComponent<BlazorConsoleComponent>(parameters => parameters.Add(c => c.ReplaceWhiteString, false));

        blazorConsoleComponentContext.Instance.WriteLineAsync("This is a test string");

        Assert.Equal(expectedOutput, blazorConsoleComponentContext.Instance.Output);
    }
}