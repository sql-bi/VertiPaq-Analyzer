namespace Dax.Vpax.CLI.Commands;

internal abstract class CommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context) => throw new NotSupportedException("Use InvokeAsync instead.");
    public abstract Task<int> InvokeAsync(InvocationContext context);

    protected FileInfo? GetCurrentPackage(InvocationContext context)
    {
        var path = context.ParseResult.GetValueForOption(CommonOptions.PathOption) ?? UserSession.GetPackagePath();
        if (path is null)
        {
            AnsiConsole.MarkupLine($"[red]No package set. Use `vpax package set` to set a package.[/]");
            context.ExitCode = 2;
            return null;
        }
        var file = new FileInfo(path);
        if (!file.Exists)
        {
            AnsiConsole.MarkupLine($"[red]Package file does not exist or is not accessible. [[{path}]][/]");
            context.ExitCode = 3;
            return null;
        }
        return file;
    }

    protected Metadata.Model? GetCurrentModel(InvocationContext context)
    {
        var file = GetCurrentPackage(context);
        if (file is null)
            return null;

        var model = AnsiConsole.Status().AutoRefresh(true).Spinner(Spinner.Known.Default).Start<Metadata.Model>("[yellow]Loading VPAX package...[/]", (context) =>
        {
            var content = VpaxTools.ImportVpax(file.FullName, importDatabase: false);
            return content.DaxModel;
        });

        if (model is null)
        {
            AnsiConsole.MarkupLine($"[red]Package does not contain {VpaxFormat.DAXMODEL}. Verify the package and try again.[/]");
            context.ExitCode = 3;
            return null;
        }

        return model;
    }
}
