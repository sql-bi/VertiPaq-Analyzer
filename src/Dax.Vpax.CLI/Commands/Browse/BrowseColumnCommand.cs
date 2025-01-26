using static Dax.Vpax.CLI.Commands.Browse.BrowseColumnCommandOptions;

namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseColumnCommand : Command
{
    public static BrowseColumnCommand Instance { get; } = new BrowseColumnCommand();

    private BrowseColumnCommand()
        : base(name: "column", description: "Display column information")
    {
        AddAlias("c");
        AddOption(CommonOptions.VpaxOption);
        AddOption(TableOption);
        AddOption(CommonOptions.ExcludeHiddenOption);
        AddOption(CommonOptions.OrderByOption);
        AddOption(CommonOptions.TopOption);

        Handler = new BrowseColumnCommandHandler();
    }
}

internal static class BrowseColumnCommandOptions
{
    public static readonly Option<string> TableOption = new(
        name: "--table",
        description: "Specify the table name"
    )
    {
        ArgumentHelpName = "name"
    };
}
