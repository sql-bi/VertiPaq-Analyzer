namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseModelCommand : Command
{
    public static BrowseModelCommand Instance { get; } = new BrowseModelCommand();

    private BrowseModelCommand()
        : base(name: "model", description: "Display tabular model information")
    {
        AddAlias("m");
        Handler = new BrowseModelCommandHandler();
    }
}
