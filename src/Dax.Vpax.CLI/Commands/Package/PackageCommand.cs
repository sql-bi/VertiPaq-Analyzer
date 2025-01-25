namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageCommand : Command
{
    public static PackageCommand Instance { get; } = new PackageCommand();

    private PackageCommand()
        : base(name: "package", description: "(Experimental) Manage a VPAX package file")
    {
        AddCommand(PackageExtractCommand.Instance);
        AddCommand(PackageShowCommand.Instance);
    }
}
