using Dax.Vpax.CLI.Commands.Package;

namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseRelationshipCommand : Command
{
    public static BrowseRelationshipCommand Instance { get; } = new BrowseRelationshipCommand();

    private BrowseRelationshipCommand()
        : base(name: "relationship", description: "Display relationships information")
    {
        AddAlias("r");
        Handler = new BrowseRelationshipCommandHandler();
    }
}
