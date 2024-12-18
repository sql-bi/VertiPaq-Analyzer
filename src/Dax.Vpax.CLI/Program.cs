using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace Dax.Vpax.CLI;

internal sealed class Program
{
    public static async Task<int> Main(string[] args)
        => await Build().InvokeAsync(args).ConfigureAwait(false);

    private static Parser Build()
    {
        var command = new RootVpaxCommand();
        var builder = new CommandLineBuilder(command);
        _ = builder.UseVpaxDefaults();

        return builder.Build();
    }
}
