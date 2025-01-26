namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseCommand : Command
{
    public static BrowseCommand Instance { get; } = new BrowseCommand();

    private BrowseCommand()
        : base(name: "browse", description: "(Experimental) Browse metadata of a tabular model in a VPAX package file")
    {
        AddCommand(BrowseModelCommand.Instance);
        AddCommand(BrowseTableCommand.Instance);
        AddCommand(BrowseColumnCommand.Instance);
        AddCommand(BrowseRelationshipCommand.Instance);
    }
}
