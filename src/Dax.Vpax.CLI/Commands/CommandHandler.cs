namespace Dax.Vpax.CLI.Commands;

internal abstract class CommandHandler : ICommandHandler
{
    public const int ErrorPackageInvalid = 1001;

    public int Invoke(InvocationContext context) => throw new NotSupportedException("Use InvokeAsync instead.");

    public abstract Task<int> InvokeAsync(InvocationContext context);

    protected static Metadata.Model? GetModel(InvocationContext context, FileInfo vpax)
    {
        var model = AnsiConsole.Status().AutoRefresh(true).Spinner(Spinner.Known.Default).Start("[yellow]Loading VPAX file...[/]", (context) =>
        {
            var content = VpaxTools.ImportVpax(vpax.FullName, importDatabase: false);
            return content.DaxModel;
        });

        if (model is null)
        {
            AnsiConsole.MarkupLine($"[red]VPAX file does not contain {VpaxFormat.DAXMODEL}. Verify the package and try again.[/]");
            context.ExitCode = ErrorPackageInvalid;
            return null;
        }

        return model;
    }
}
