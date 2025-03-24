using Dax.Vpax.Obfuscator;
using Dax.Vpax.Obfuscator.Common;
using static Dax.Vpax.CLI.Commands.Package.PackageObfuscateCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageObfuscateCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var vpaxPath = context.ParseResult.GetValueForArgument(VpaxArgument);
        var dictionaryPath = context.ParseResult.GetValueForOption(DictionaryOption);
        var outputVpaxPath = context.ParseResult.GetValueForOption(OutputVpaxOption);
        var outputDictionaryPath = context.ParseResult.GetValueForOption(OutputDictionaryOption);
        var overwrite = context.ParseResult.GetValueForOption(OverwriteOption);

        using var vpaxStream = new MemoryStream(File.ReadAllBytes(vpaxPath));

        var dictionary = dictionaryPath is not null ? ObfuscationDictionary.ReadFrom(dictionaryPath) : null;
        var outputDictionary = new VpaxObfuscator().Obfuscate(vpaxStream, dictionary);

        // notify user if there are unobfuscated values in the dictionary
        if (outputDictionary.UnobfuscatedValues.Count > 0)
            AnsiConsole.MarkupLine($"[yellow]Obfuscation dictionary contains unobfuscated values. [[{outputDictionary.UnobfuscatedValues.Count}]][/]");

        outputDictionaryPath ??= Path.ChangeExtension(vpaxPath, ".dict");
        outputVpaxPath ??= Path.ChangeExtension(vpaxPath, ".ovpax");

        outputDictionary.WriteTo(outputDictionaryPath, overwrite, indented: true);

        var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
        using var outputVpaxStream = new FileStream(outputVpaxPath, mode, FileAccess.Write, FileShare.Read);
        vpaxStream.WriteTo(outputVpaxStream);

        return Task.FromResult(context.ExitCode);
    }
}
