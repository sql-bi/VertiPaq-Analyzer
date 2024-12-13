namespace Dax.Vpax.CLI;

internal sealed class Program
{
    public static async Task<int> Main(string[] args)
        => await Build().InvokeAsync(args).ConfigureAwait(false);

    private static RootCommand Build()
    {
        var command = new RootCommand("VertiPaq-Analyzer CLI");
        command.Name = "vpax"; // Name must match <ToolCommandName> in csproj
        command.AddCommand(ExportCommand.GetCommand());
        return command;
    }
}
