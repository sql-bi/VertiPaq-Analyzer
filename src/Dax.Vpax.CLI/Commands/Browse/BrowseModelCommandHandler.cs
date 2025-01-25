namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseModelCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var vpax = context.ParseResult.GetValueForOption(CommonOptions.VpaxOption)!;

        var model = GetModel(context, vpax);
        if (model is not null)
        {
            var grid = new Grid()
                .AddColumns(1)
                .AddRow(GetTableView(model))
                .AddEmptyRow()
                .AddRow(GetChartView(model));

            AnsiConsole.Write(new Panel(grid));
        }

        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetTableView(Metadata.Model model)
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
            .AddRow("[yellow]Columns[/]", model.Tables.SelectMany((t) => t.Columns).Count(c => !c.IsRowNumber).ToString())
            .AddRow("[yellow]Size (in memory)[/]", model.Tables.Sum((t) => t.TableSize).ToSizeString());

        return table;
    }

    private IRenderable GetChartView(Metadata.Model model)
    {
        var dataSize = model.Tables.Sum((t) => t.ColumnsDataSize);
        var dictionarySize = model.Tables.Sum((t) => t.ColumnsDictionarySize);
        var hierarchiesSize = model.Tables.Sum((t) => t.ColumnsHierarchiesSize);
        var totalSize = dataSize + dictionarySize + hierarchiesSize;

        var dataPercentage = Math.Floor((double)dataSize / totalSize * 100);
        var dictionaryPercentage = Math.Floor((double)dictionarySize / totalSize * 100);
        var hierarchiesPercentage = 100 - dataPercentage - dictionaryPercentage;

        var chart = new BreakdownChart().ShowPercentage().FullSize()
            .AddItem("Data", dataPercentage, Color.Red)
            .AddItem("Dictionary", dictionaryPercentage, Color.Green)
            .AddItem("Hierarchy", hierarchiesPercentage, Color.Blue);

        return chart;
    }
}
