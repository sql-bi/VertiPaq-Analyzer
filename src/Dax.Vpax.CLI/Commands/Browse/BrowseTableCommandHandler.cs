namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseTableCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var model = GetCurrentModel(context);
        if (model is null)
            return Task.FromResult(context.ExitCode);

        AnsiConsole.Write(GetTables(context, model));
        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetTables(InvocationContext context, Metadata.Model model)
    {
        var top = context.ParseResult.GetValueForOption(CommonOptions.TopOption);
        var excludeHidden = context.ParseResult.GetValueForOption(CommonOptions.ExcludeHiddenOption);
        var orderBy = context.ParseResult.GetValueForOption(CommonOptions.OrderByOption);

        var table = new Spectre.Console.Table().BorderColor(Color.Yellow)
            .AddColumn(new TableColumn(new Markup("[yellow]Name[/]").Centered()).NoWrap())
            .AddColumn(new TableColumn(new Markup("[yellow]Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Size %[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Data Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Dictionary Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Hierarchies Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Columns[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Partitions[/]").Centered()));

        var query = orderBy switch
        {
            "name" or "n" => model.Tables.OrderBy((t) => t.TableName.Name),
            "cardinality" or "c" => model.Tables.OrderByDescending((t) => t.RowsCount),
            "size" or "s" => model.Tables.OrderByDescending((t) => t.TableSize),
            _ => model.Tables.AsEnumerable()
        };

        if (excludeHidden) query = query.Where((t) => !t.IsHidden());
        if (top.HasValue) query = query.Take(top.Value);

        var modelSize = model.Tables.Sum((t) => t.TableSize);
        var tables = query.ToArray();

        foreach (var t in query)
        {
            var style = t.IsHidden() ? new Style(foreground: Color.Grey) : Style.Plain;
            var sizePercentage = (double)t.TableSize / modelSize;

            table.AddRow(
                new Text(t.TableName.Name, style).LeftJustified(),
                new Text(t.RowsCount.ToString("N0"), style).RightJustified(),
                new Text(t.TableSize.ToString("N0"), style).RightJustified(),
                new Text(sizePercentage.ToString("P2"), style).RightJustified(),
                new Text(t.ColumnsDataSize.ToString("N0"), style).RightJustified(),
                new Text(t.ColumnsDictionarySize.ToString("N0"), style).RightJustified(),
                new Text(t.ColumnsHierarchiesSize.ToString("N0"), style).RightJustified(),
                new Text(t.Columns.Count.ToString("N0"), style).RightJustified(),
                new Text(t.Partitions.Count.ToString("N0"), style).RightJustified()
                );
        }

        table.Columns[0].Footer = new Markup($"[grey]{tables.Length:N0} items[/]").LeftJustified();
        table.Columns[1].Footer = new Markup($"[grey]{tables.Sum(_ => _.RowsCount):N0}[/]").RightJustified();
        table.Columns[2].Footer = new Markup($"[grey]{tables.Sum(_ => _.TableSize).ToSizeString()}[/]").RightJustified();
        table.Columns[4].Footer = new Markup($"[grey]{tables.Sum(_ => _.ColumnsDataSize).ToSizeString()}[/]").RightJustified();
        table.Columns[5].Footer = new Markup($"[grey]{tables.Sum(_ => _.ColumnsDictionarySize).ToSizeString()}[/]").RightJustified();
        table.Columns[6].Footer = new Markup($"[grey]{tables.Sum(_ => _.ColumnsHierarchiesSize).ToSizeString()}[/]").RightJustified();
        table.Columns[7].Footer = new Markup($"[grey]{tables.Sum(_ => _.Columns.Count):N0}[/]").RightJustified();
        table.Columns[8].Footer = new Markup($"[grey]{tables.Sum(_ => _.Partitions.Count):N0}[/]").RightJustified();

        return table;
    }
}
