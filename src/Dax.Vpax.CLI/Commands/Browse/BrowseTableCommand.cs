namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseTableCommand : Command
{
    public static BrowseTableCommand Instance { get; } = new BrowseTableCommand();

    private BrowseTableCommand()
        : base(name: "table", description: "Display table information")
    {
        AddAlias("t");
        AddOption(CommonOptions.VpaxOption);
        AddOption(CommonOptions.ExcludeHiddenOption);
        AddOption(CommonOptions.OrderByOption);
        AddOption(CommonOptions.TopOption);

        Handler = new BrowseTableCommandHandler();
    }
}
