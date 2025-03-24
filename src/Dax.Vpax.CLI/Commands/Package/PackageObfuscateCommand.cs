using static Dax.Vpax.CLI.Commands.Package.PackageObfuscateCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageObfuscateCommand : Command
{
    public static PackageObfuscateCommand Instance { get; } = new PackageObfuscateCommand();

    private PackageObfuscateCommand()
        : base(name: "obfuscate", description: "Obfuscate the DaxModel.json file and delete all other contents from a VPAX package file.")
    {
        AddAlias("o");
        AddArgument(VpaxArgument);
        AddOption(DictionaryOption);
        AddOption(OutputVpaxOption);
        AddOption(OutputDictionaryOption);
        AddOption(OverwriteOption);

        Handler = new PackageObfuscateCommandHandler();
    }
}

internal static class PackageObfuscateCommandOptions
{
    public static readonly Argument<string> VpaxArgument = new()
    {
        Name = "vpax",
        Description = "Path to the VPAX file to be obfuscated.",
    };

    public static readonly Option<string?> DictionaryOption = new(name: "--dictionary")
    {
        Description = "Path to the dictionary file to be used for incremental obfuscation. If not specified, a new dictionary will be created",
        ArgumentHelpName = "path",
        IsRequired = false,
    };

    public static readonly Option<string?> OutputVpaxOption = new(name: "--output-vpax")
    {
        Description = "Path to write the obfuscated VPAX file. If not specified, the file will be written to the same directory as the input VPAX file",
        ArgumentHelpName = "path",
        IsRequired = false,
    };

    public static readonly Option<string?> OutputDictionaryOption = new(name: "--output-dictionary")
    {
        Description = "Path to write the obfuscation dictionary file. If not specified, the file will be written to the same directory as the input dictionary file",
        ArgumentHelpName = "path",
        IsRequired = false,
    };

    public static readonly Option<bool> OverwriteOption = new(name: "--overwrite")
    {
        Description = "Allows output files to be overwritten. If not specified, the command will fail if the file already exists",
        IsRequired = false,
    };
}
