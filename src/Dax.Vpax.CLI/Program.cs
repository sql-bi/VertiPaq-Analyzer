using System.CommandLine;
using Dax.Vpax.CLI.Commands;

namespace Dax.Vpax.CLI;

internal sealed class Program
{
    public static async Task<int> Main(string[] args)
        => await Build().InvokeAsync(args).ConfigureAwait(false);

    private static RootCommand Build()
    {
        var command = new RootCommand("VertiPaq-Analyzer CLI")
        {
            ExportCommand.GetCommand()
        };
        return command;
    }
}
