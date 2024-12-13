namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageUnsetCommand : Command
{
    public static PackageUnsetCommand Instance { get; } = new PackageUnsetCommand();

    private PackageUnsetCommand()
        : base(name: "unset", description: "Unset the current VPAX package file")
    {
        Handler = new PackageUnsetCommandHandler();
    }
}
