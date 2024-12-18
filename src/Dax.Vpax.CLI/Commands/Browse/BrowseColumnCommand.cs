using Dax.Vpax.CLI.Commands.Package;

namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseColumnCommand : Command
{
    public static BrowseColumnCommand Instance { get; } = new BrowseColumnCommand();

    private BrowseColumnCommand()
        : base(name: "column", description: "Display columns information")
    {
        AddAlias("c");
        Handler = new BrowseColumnCommandHandler();
    }
}
