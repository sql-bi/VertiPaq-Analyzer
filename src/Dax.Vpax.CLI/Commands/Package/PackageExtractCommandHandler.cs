namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageExtractCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var file = GetCurrentPackage(context);
        if (file is null)
            return Task.FromResult(context.ExitCode);

        using var package = System.IO.Packaging.Package.Open(file.FullName, FileMode.Open, FileAccess.Read);

        var overwrite = context.ParseResult.GetValueForOption(PackageExtractCommandOptions.OverwriteOption);
        var path = context.ParseResult.GetValueForArgument(PackageExtractCommandOptions.PathArgument);
        _ = Directory.CreateDirectory(path.FullName);

        AnsiConsole.Status().AutoRefresh(true).Spinner(Spinner.Known.Default).Start($"[yellow]Extracting package {Markup.Escape(file.FullName)}...[/]", (context) =>
        {
            foreach (var part in package.GetParts())
            {
                var partName = part.Uri.OriginalString.TrimStart('/');
                AnsiConsole.MarkupLine($"[grey]Extracting {Markup.Escape(partName)}...[/]");

                var filePath = Path.Combine(path.FullName, partName);
                var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
                using var fileStream = new FileStream(filePath, fileMode, FileAccess.Write);
                using var partStream = part.GetStream();
                partStream.CopyTo(fileStream);
            }

            //AnsiConsole.MarkupLine("[green]Completed[/]");
        });

        return Task.FromResult(context.ExitCode);
    }
}
