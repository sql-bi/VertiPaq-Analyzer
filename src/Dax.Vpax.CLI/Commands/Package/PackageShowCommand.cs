namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageShowCommand : Command
{
    public static PackageShowCommand Instance { get; } = new PackageShowCommand();

    private PackageShowCommand()
        : base(name: "show", description: "Show details of a VPAX package file")
    {
        AddOption(Commands.CommonOptions.PathOption);

        Handler = new PackageShowCommandHandler();
    }
}
