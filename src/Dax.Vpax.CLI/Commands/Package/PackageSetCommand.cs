namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageSetCommand : Command
{
    public static PackageSetCommand Instance { get; } = new PackageSetCommand();

    private PackageSetCommand()
        : base(name: "set", description: "Set a VPAX file to be the current active package")
    {
        AddArgument(PackageSetCommandOptions.PathArgument);

        Handler = new PackageSetCommandHandler();
    }
}

internal static class PackageSetCommandOptions
{
    public static readonly Argument<string> PathArgument = new(
        name: "path",
        description: "Path to the VPAX package file."
        );
}
