using static Dax.Vpax.CLI.Commands.Package.PackageExtractCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageExtractCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var vpax = context.ParseResult.GetValueForOption(VpaxOption)!;
        var overwrite = context.ParseResult.GetValueForOption(OverwriteOption);
        var output = context.ParseResult.GetValueForOption(OutputOption);

        using var package = System.IO.Packaging.Package.Open(vpax.FullName, FileMode.Open, FileAccess.Read);
        
        if (output is null)
            output = new DirectoryInfo(path: Path.Combine(vpax.DirectoryName!, Path.GetFileNameWithoutExtension(vpax.Name)));

        if (!output.Exists)
            output.Create();

        AnsiConsole.Status().AutoRefresh(true).Spinner(Spinner.Known.Default).Start($"[yellow]Extracting package {Markup.Escape(vpax.Name)}...[/]", (context) =>
        {
            foreach (var part in package.GetParts())
            {
                var partName = part.Uri.OriginalString.TrimStart('/');
                context.Status($"[yellow]Extracting {Markup.Escape(partName)}...[/]");

                var filePath = Path.Combine(output.FullName, partName);
                var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
                using var fileStream = new FileStream(filePath, fileMode, FileAccess.Write);
                using var partStream = part.GetStream();
                partStream.CopyTo(fileStream);

                AnsiConsole.MarkupLine($"[green]Extracted {Markup.Escape(partName)}[/]");
            }

            AnsiConsole.MarkupLine("[green]Completed[/]");
        });

        return Task.FromResult(context.ExitCode);
    }
}
