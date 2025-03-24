using Dax.Vpax.Obfuscator;
using Dax.Vpax.Obfuscator.Common;
using static Dax.Vpax.CLI.Commands.Package.PackageDeobfuscateCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageDeobfuscateCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var vpaxPath = context.ParseResult.GetValueForArgument(VpaxArgument);
        var dictionaryPath = context.ParseResult.GetValueForArgument(DictionaryArgument);
        var outputVpaxPath = context.ParseResult.GetValueForOption(OutputVpaxOption);
        var overwrite = context.ParseResult.GetValueForOption(OverwriteOption);

        using var vpaxStream = new MemoryStream(File.ReadAllBytes(vpaxPath));

        var dictionary = ObfuscationDictionary.ReadFrom(dictionaryPath);
        new VpaxObfuscator().Deobfuscate(vpaxStream, dictionary);

        outputVpaxPath ??= Path.ChangeExtension(vpaxPath, ".vpax");

        var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
        using var outputVpaxStream = new FileStream(outputVpaxPath, mode, FileAccess.Write, FileShare.Read);
        vpaxStream.WriteTo(outputVpaxStream);

        return Task.FromResult(context.ExitCode);
    }
}
