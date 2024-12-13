namespace Dax.Vpax.CLI.Commands;

internal sealed class RootVpaxCommand : RootCommand
{
    public RootVpaxCommand()
        : base(description: "VertiPaq-Analyzer CLI")
    {
        Name = "vpax"; // Name must match <ToolCommandName> in csproj

        AddCommand(Browse.BrowseCommand.Instance);
        AddCommand(Export.ExportCommand.Instance);
        AddCommand(Package.PackageCommand.Instance);
    }
}
