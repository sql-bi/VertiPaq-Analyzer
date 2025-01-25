namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseRelationshipCommand : Command
{
    public static BrowseRelationshipCommand Instance { get; } = new BrowseRelationshipCommand();

    private BrowseRelationshipCommand()
        : base(name: "relationship", description: "Display relationship information")
    {
        AddAlias("r");
        AddOption(CommonOptions.VpaxOption);
        AddOption(CommonOptions.ExcludeHiddenOption);
        AddOption(CommonOptions.OrderByOption);
        AddOption(CommonOptions.TopOption);

        Handler = new BrowseRelationshipCommandHandler();
    }
}
