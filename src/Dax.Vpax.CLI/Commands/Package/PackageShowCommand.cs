using static Dax.Vpax.CLI.Commands.Package.PackageShowCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageShowCommand : Command
{
    public static PackageShowCommand Instance { get; } = new PackageShowCommand();

    private PackageShowCommand()
        : base(name: "show", description: "Show details of a VPAX package file")
    {
        AddOption(VpaxOption);

        Handler = new PackageShowCommandHandler();
    }
}

internal static class PackageShowCommandOptions
{
    public static readonly Option<FileInfo> VpaxOption = new(
        name: "--vpax",
        description: "Path to the VPAX file"
        )
    {
        IsRequired = true,
    };
}
