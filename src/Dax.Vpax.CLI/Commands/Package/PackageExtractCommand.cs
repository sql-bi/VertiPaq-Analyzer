using static Dax.Vpax.CLI.Commands.Package.PackageExtractCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageExtractCommand : Command
{
    public static PackageExtractCommand Instance { get; } = new PackageExtractCommand();

    private PackageExtractCommand()
        : base(name: "extract", description: "Extract all files from a VPAX package")
    {
        AddAlias("e");
        AddArgument(PackageExtractCommandOptions.PathArgument);
        AddOption(CommonOptions.PathOption);
        AddOption(PackageExtractCommandOptions.OverwriteOption);

        Handler = new PackageExtractCommandHandler();
    }
}

internal static class PackageExtractCommandOptions
{
    public static readonly Option<FileInfo> VpaxOption = new(
        name: "--vpax",
        description: "Path to the VPAX file"
        )
    {
        IsRequired = true,
    };

    public static readonly Option<DirectoryInfo> OutputOption = new(
        name: "--output",
        description: "Path for the output directory. If not specified, the VPAX directory will be used"
        );

    public static readonly Option<bool> OverwriteOption = new(
        name: "--overwrite",
        getDefaultValue: () => false,
        description: "Overwrite the extracted file if it already exists"
        );
}
