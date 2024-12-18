using Spectre.Console;

namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseModelCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var model = GetCurrentModel(context);
        if (model is null)
            return Task.FromResult(context.ExitCode);

        var grid = new Grid()
            .AddColumns(1)
            .AddRow(GetProperties(model))
            .AddEmptyRow()
            .AddRow(GetSizeChart(model));

        AnsiConsole.Write(new Panel(grid));
        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetProperties(Metadata.Model model)
    {
        var table = new Spectre.Console.Table().HideHeaders().Expand().BorderColor(Color.Yellow)
            .AddColumn("Name")
            .AddColumn("Value")
            .AddRow("[yellow]Model[/]", model.ModelName.Name)
            .AddRow("[yellow]Compatibility Level[/]", model.CompatibilityLevel.ToString())
            .AddRow("[yellow]Compatibility Mode[/]", model.CompatibilityMode.ToEmptyIfNull())
            .AddRow("[yellow]Version[/]", model.Version.ToString())
            .AddRow("[yellow]Culture[/]", model.Culture.ToEmptyIfNull())
            .AddRow("[yellow]Last Refresh[/]", model.LastDataRefresh.ToString("o", CultureInfo.InvariantCulture))
            .AddRow("[yellow]Last Process[/]", model.LastProcessed.ToString("o", CultureInfo.InvariantCulture))
            .AddRow("[yellow]Last Update[/]", model.LastUpdate.ToString("o", CultureInfo.InvariantCulture))
            .AddRow("[yellow]Tables[/]", model.Tables.Count.ToString())
            .AddRow("[yellow]Columns[/]", model.Tables.SelectMany((t) => t.Columns).Where(c => !c.IsRowNumber).Count().ToString())
            .AddRow("[yellow]Size (in memory)[/]", model.Tables.Sum((t) => t.TableSize).ToSizeString());

        return table;
    }

    private IRenderable GetSizeChart(Metadata.Model model)
    {
        var dataSize = model.Tables.Sum((t) => t.ColumnsDataSize);
        var dictionariesSize = model.Tables.Sum((t) => t.ColumnsDictionarySize);
        var hierarchiesSize = model.Tables.Sum((t) => t.ColumnsHierarchiesSize);
        var totalSize = dataSize + dictionariesSize + hierarchiesSize;

        var dataPercentage = Math.Floor((double)dataSize / totalSize * 100);
        var dictionariePercentage = Math.Floor((double)dictionariesSize / totalSize * 100);
        var hierarchiesPercentage = 100 - dataPercentage - dictionariePercentage;

        var chart = new BreakdownChart().ShowPercentage().FullSize()
            .AddItem("Data", dataPercentage, Color.Red)
            .AddItem("Dictionary", dictionariePercentage, Color.Green)
            .AddItem("Hierarchy", hierarchiesPercentage, Color.Blue);

        return chart;
    }
}
