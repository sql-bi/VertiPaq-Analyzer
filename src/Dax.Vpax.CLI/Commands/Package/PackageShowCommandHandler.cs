using Spectre.Console;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageShowCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var file = GetCurrentPackage(context);
        if (file is null)
            return Task.FromResult(context.ExitCode);

        var grid = new Grid()
            .AddColumns(1)
            .AddRow(GetProperties(file))
            .AddEmptyRow()
            .AddRow(GetContent(file));

        AnsiConsole.Write(new Panel(grid));
        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetProperties(FileInfo file)
    {
        var table = new Spectre.Console.Table().HideHeaders().Expand().BorderColor(Color.Yellow)
            .AddColumn("Name")
            .AddColumn("Value");
        table.AddRow("[yellow]File[/]", file.FullName);
        table.AddRow("[yellow]Size[/]", file.Length.ToSizeString());
        table.AddRow("[yellow]Created[/]", file.CreationTime.ToString("o", CultureInfo.InvariantCulture));
        table.AddRow("[yellow]Modified[/]", file.LastWriteTime.ToString("o", CultureInfo.InvariantCulture));
        table.AddRow("[yellow]Accessed[/]", file.LastAccessTime.ToString("o", CultureInfo.InvariantCulture));

        return table;
    }

    private IRenderable GetContent(FileInfo file)
    {
        var table = new Spectre.Console.Table().HeavyBorder().Expand().BorderColor(Color.Yellow)
            .AddColumn(new TableColumn(new Markup("[yellow]Part[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Type[/]").Centered()));

        using var package = System.IO.Packaging.Package.Open(file.FullName, FileMode.Open, FileAccess.Read);
        var totalSize = 0L;
        var partsCount = 0;

        foreach (var part in package.GetParts())
        {
            using var stream = part.GetStream();
            var size = stream.Length;

            totalSize += size;
            partsCount++;

            table.AddRow(
                new Text(part.Uri.OriginalString).LeftJustified(),
                new Text(size.ToSizeString()).RightJustified(),
                new Text(part.ContentType).Centered()
                );
        }

        table.Columns[0].Footer($"[grey]{partsCount} items[/]");
        table.Columns[1].Footer = new Markup($"[grey]{totalSize.ToSizeString()}[/]").RightJustified();

        return table;
    }
}
