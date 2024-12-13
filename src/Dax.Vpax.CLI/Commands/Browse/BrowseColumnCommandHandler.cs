namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseColumnCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var model = GetCurrentModel(context);
        if (model is null)
            return Task.FromResult(context.ExitCode);

        AnsiConsole.Write(GetColumns(context, model));
        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetColumns(InvocationContext context, Metadata.Model model)
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
            .AddColumn(new TableColumn(new Markup("[yellow]Encoding[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Data Type[/]").Centered()));

        var query = model.Tables.SelectMany((t) => t.Columns).Where(c => !c.IsRowNumber);

        query = orderBy switch
        {
            "name" or "n" => query.OrderBy((c) => c.ToDisplayName()),
            "cardinality" or "c" => query.OrderByDescending((c) => c.ColumnCardinality),
            "size" or "s" => query.OrderByDescending((c) => c.TotalSize),
            _ => query
        };

        if (excludeHidden) query = query.Where((c) => !c.IsHidden);
        if (top.HasValue) query = query.Take(top.Value);

        var modelSize = model.Tables.Sum((t) => t.TableSize);
        var columns = query.ToArray();

        foreach (var c in columns)
        {
            var style = c.IsHidden ? new Style(foreground: Color.Grey) : Style.Plain;
            var sizePercentage = (double)c.TotalSize / modelSize;

            table.AddRow(
                new Text(c.ToDisplayName(), style).LeftJustified(),
                new Text(c.ColumnCardinality.ToString("N0"), style).RightJustified(),
                new Text(c.TotalSize.ToString("N0"), style).RightJustified(),
                new Text(sizePercentage.ToString("P2"), style).RightJustified(),
                new Text(c.DataSize.ToString("N0"), style).RightJustified(),
                new Text(c.DictionarySize.ToString("N0"), style).RightJustified(),
                new Text(c.HierarchiesSize.ToString("N0"), style).RightJustified(),
                new Text(c.Encoding, style).RightJustified(),
                new Text(c.DataType, style).RightJustified()
                );
        }

        table.Columns[0].Footer = new Markup($"[grey]{columns.Length:N0} items[/]").LeftJustified();
        table.Columns[1].Footer = new Markup($"[grey]{columns.Sum(_ => _.ColumnCardinality):N0}[/]").RightJustified();
        table.Columns[2].Footer = new Markup($"[grey]{columns.Sum(_ => _.TotalSize).ToSizeString():N0}[/]").RightJustified();
        table.Columns[4].Footer = new Markup($"[grey]{columns.Sum(_ => _.DataSize).ToSizeString():N0}[/]").RightJustified();
        table.Columns[5].Footer = new Markup($"[grey]{columns.Sum(_ => _.DictionarySize).ToSizeString():N0}[/]").RightJustified();
        table.Columns[6].Footer = new Markup($"[grey]{columns.Sum(_ => _.HierarchiesSize).ToSizeString():N0}[/]").RightJustified();

        return table;
    }
}
