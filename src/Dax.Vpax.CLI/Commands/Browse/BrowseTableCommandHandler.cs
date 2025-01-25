namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseTableCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var vpax = context.ParseResult.GetValueForOption(CommonOptions.VpaxOption)!;

        var model = GetModel(context, vpax);
        if (model is not null)
        {
            AnsiConsole.Write(GetView(context, model));
        }

        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetView(InvocationContext context, Metadata.Model model)
    {
        var excludeHidden = context.ParseResult.GetValueForOption(CommonOptions.ExcludeHiddenOption);
        var orderBy = context.ParseResult.GetValueForOption(CommonOptions.OrderByOption);
        var top = context.ParseResult.GetValueForOption(CommonOptions.TopOption);

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

        var totalSize = model.Tables.Sum((t) => t.TableSize);
        var query = model.Tables.Select((t) => new Row(t, totalSize));
        if (orderBy.HasValue)
        {
            query = orderBy switch
            {
                -1 => query.OrderByDescending(_ => _.Name),
                +1 => query.OrderBy(_ => _.Name),
                2 => query.OrderByDescending(_ => _.Cardinality),
                3 => query.OrderByDescending(_ => _.Size),
                4 => query.OrderByDescending(_ => _.SizePercentage),
                5 => query.OrderByDescending(_ => _.DataSize),
                6 => query.OrderByDescending(_ => _.DictionarySize),
                7 => query.OrderByDescending(_ => _.HierarchiesSize),
                8 => query.OrderByDescending(_ => _.Columns),
                9 => query.OrderByDescending(_ => _.Partitions),
                _ => query.OrderByDescending(_ => 0), // ignore invalid order by
            };
        }
        if (excludeHidden) query = query.Where((r) => !r.IsHidden);
        if (top.HasValue) query = query.Take(top.Value);

        var rows = query.ToArray();
        foreach (var r in rows)
        {
            var style = r.IsHidden ? new Style(foreground: Color.Grey) : Style.Plain;

            table.AddRow(
                new Text(r.Name, style).LeftJustified(),
                new Text(r.Cardinality.ToString("N0"), style).RightJustified(),
                new Text(r.Size.ToString("N0"), style).RightJustified(),
                new Text(r.SizePercentage.ToString("P2"), style).RightJustified(),
                new Text(r.DataSize.ToString("N0"), style).RightJustified(),
                new Text(r.DictionarySize.ToString("N0"), style).RightJustified(),
                new Text(r.HierarchiesSize.ToString("N0"), style).RightJustified(),
                new Text(r.Columns.ToString("N0"), style).RightJustified(),
                new Text(r.Partitions.ToString("N0"), style).RightJustified()
                );
        }

        table.Columns[0].Footer = new Markup($"[grey]{rows.Length:N0} items[/]").LeftJustified();
        table.Columns[1].Footer = new Markup($"[grey]{rows.Sum(_ => _.Cardinality):N0}[/]").RightJustified();
        table.Columns[2].Footer = new Markup($"[grey]{rows.Sum(_ => _.Size).ToSizeString()}[/]").RightJustified();
        // table.Columns[3].Footer = // SizePercentage
        table.Columns[4].Footer = new Markup($"[grey]{rows.Sum(_ => _.DataSize).ToSizeString()}[/]").RightJustified();
        table.Columns[5].Footer = new Markup($"[grey]{rows.Sum(_ => _.DictionarySize).ToSizeString()}[/]").RightJustified();
        table.Columns[6].Footer = new Markup($"[grey]{rows.Sum(_ => _.HierarchiesSize).ToSizeString()}[/]").RightJustified();
        table.Columns[7].Footer = new Markup($"[grey]{rows.Sum(_ => _.Columns):N0}[/]").RightJustified();
        table.Columns[8].Footer = new Markup($"[grey]{rows.Sum(_ => _.Partitions):N0}[/]").RightJustified();

        return table;
    }

    private sealed record Row(Metadata.Table table, long totalSize)
    {
        public bool IsHidden { get; } = table.IsHidden();
        public string Name { get; } = table.TableName.Name;
        public long Cardinality { get; } = table.RowsCount;
        public long Size { get; } = table.TableSize;
        public double SizePercentage { get; } = (double)table.TableSize / totalSize;
        public long DataSize { get; } = table.ColumnsDataSize;
        public long DictionarySize { get; } = table.ColumnsDictionarySize;
        public long HierarchiesSize { get; } = table.ColumnsHierarchiesSize;
        public int Columns { get; } = table.Columns.Count;
        public int Partitions { get; } = table.Partitions.Count;
    }
}
