using static Dax.Vpax.CLI.Commands.Export.ExportCommandOptions;

namespace Dax.Vpax.CLI.Commands.Export;

internal sealed class ExportCommand : Command
{
    public static ExportCommand Instance { get; } = new ExportCommand();

    private ExportCommand()
        : base(name: "export", description: "Export a VPAX file from a tabular model")
    {
        AddArgument(PathArgument);
        AddArgument(ConnectionStringArgument);
        AddOption(OverwriteOption);
        AddOption(ExcludeTomOption);
        AddOption(ExcludeVpaOption);
        AddOption(DirectQueryModeOption);
        AddOption(DirectLakeModeOption);
        AddOption(ColumnBatchSizeOption);

        Handler = new ExportCommandHandler();
    }
}
