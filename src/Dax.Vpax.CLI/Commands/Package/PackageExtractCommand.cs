namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageExtractCommand : Command
{
    public static PackageExtractCommand Instance { get; } = new PackageExtractCommand();

    private PackageExtractCommand()
        : base(name: "extract", description: "Extract all files from a VPAX package")
    {
        AddArgument(PackageExtractCommandOptions.PathArgument);
        AddOption(CommonOptions.PathOption);
        AddOption(PackageExtractCommandOptions.OverwriteOption);

        Handler = new PackageExtractCommandHandler();
    }
}

internal static class PackageExtractCommandOptions
{
    public static readonly Argument<DirectoryInfo> PathArgument = new(
        name: "path",
        description: "Path to write the extracted files"
        );

    public static readonly Option<bool> OverwriteOption = new(
        name: "--overwrite",
        getDefaultValue: () => false,
        description: "Overwrite the extracted file if it already exists"
        );
}
