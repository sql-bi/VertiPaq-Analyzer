using static Dax.Vpax.CLI.Commands.Package.PackageDeobfuscateCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageDeobfuscateCommand : Command
{
    public static PackageDeobfuscateCommand Instance { get; } = new PackageDeobfuscateCommand();

    private PackageDeobfuscateCommand()
        : base(name: "deobfuscate", description: "Deobfuscate the DaxModel.json file from an obfuscated VPAX file using the provided dictionary")
    {
        AddAlias("d");
        AddArgument(VpaxArgument);
        AddArgument(DictionaryArgument);
        AddOption(OutputVpaxOption);
        AddOption(OverwriteOption);

        Handler = new PackageDeobfuscateCommandHandler();
    }
}

internal static class PackageDeobfuscateCommandOptions
{
    public static readonly Argument<string> VpaxArgument = new()
    {
        Name = "vpax",
        Description = "Path to the VPAX file to be deobfuscated",
    };

    public static readonly Argument<string> DictionaryArgument = new()
    {
        Name = "dictionary",
        Description = "Path to the dictitionary file to be used for deobfuscation",
    };

    public static readonly Option<string> OutputVpaxOption = new(name: "--output-vpax")
    {
        Description = "Path to write the deobfuscated VPAX file. If not specified, the file will be written to the same directory as the input VPAX file",
        ArgumentHelpName = "path",
        IsRequired = false,
    };

    public static readonly Option<bool> OverwriteOption = new(name: "--overwrite")
    {
        Description = "Allows output files to be overwritten. If not specified, the command will fail if the file already exists",
        IsRequired = false,
    };
}
