using static Dax.Vpax.CLI.Commands.Browse.BrowseColumnCommandOptions;

namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseColumnCommandHandler : CommandHandler
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
        var table = context.ParseResult.GetValueForOption(TableOption);

        var view = new Spectre.Console.Table().BorderColor(Color.Yellow)
            .AddColumn(new TableColumn(new Markup("[yellow]Name[/]").Centered()).NoWrap())
            .AddColumn(new TableColumn(new Markup("[yellow]Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Size %[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Data Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Dictionary Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Hierarchies Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Encoding[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Data Type[/]").Centered()));

        var totalSize = model.Tables.Sum((t) => t.TableSize);
        var query = model.Tables.Where((t) => table is null || table.Equals(t.TableName.Name, StringComparison.OrdinalIgnoreCase)).SelectMany((t) => t.Columns).Where((c) => !c.IsRowNumber).Select((t) => new Row(t, totalSize));
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
                -8 => query.OrderByDescending(_ => _.Encoding),
                +8 => query.OrderBy(_ => _.Encoding),
                -9 => query.OrderByDescending(_ => _.DataType),
                +9 => query.OrderBy(_ => _.DataType),
                _ => query.OrderByDescending(_ => 0), // ignore invalid order by
            };
        }
        if (excludeHidden) query = query.Where((r) => !r.IsHidden);
        if (top.HasValue) query = query.Take(top.Value);

        var rows = query.ToArray();
        foreach (var r in rows)
        {
            var style = r.IsHidden ? new Style(foreground: Color.Grey) : Style.Plain;

            view.AddRow(
                new Text(r.Name, style).LeftJustified(),
                new Text(r.Cardinality.ToString("N0"), style).RightJustified(),
                new Text(r.Size.ToString("N0"), style).RightJustified(),
                new Text(r.SizePercentage.ToString("P2"), style).RightJustified(),
                new Text(r.DataSize.ToString("N0"), style).RightJustified(),
                new Text(r.DictionarySize.ToString("N0"), style).RightJustified(),
                new Text(r.HierarchiesSize.ToString("N0"), style).RightJustified(),
                new Text(r.Encoding, style).RightJustified(),
                new Text(r.DataType, style).RightJustified()
                );
        }

        view.Columns[0].Footer = new Markup($"[grey]{rows.Length:N0} items[/]").LeftJustified();
        view.Columns[1].Footer = new Markup($"[grey]{rows.Sum(_ => _.Cardinality):N0}[/]").RightJustified();
        view.Columns[2].Footer = new Markup($"[grey]{rows.Sum(_ => _.Size).ToSizeString():N0}[/]").RightJustified();
        // table.Columns[3].Footer = // SizePercentage
        view.Columns[4].Footer = new Markup($"[grey]{rows.Sum(_ => _.DataSize).ToSizeString():N0}[/]").RightJustified();
        view.Columns[5].Footer = new Markup($"[grey]{rows.Sum(_ => _.DictionarySize).ToSizeString():N0}[/]").RightJustified();
        view.Columns[6].Footer = new Markup($"[grey]{rows.Sum(_ => _.HierarchiesSize).ToSizeString():N0}[/]").RightJustified();
        // table.Columns[7].Footer = // Encoding
        // table.Columns[8].Footer = // DataType

        return view;
    }

    private sealed record Row(Metadata.Column column, long totalSize)
    {
        public bool IsHidden => column.IsHidden;
        public string Name => column.ToDisplayName();
        public long Cardinality => column.ColumnCardinality;
        public long Size => column.TotalSize;
        public double SizePercentage => (double)column.TotalSize / totalSize;
        public long DataSize => column.DataSize;
        public long DictionarySize => column.DictionarySize;
        public long HierarchiesSize => column.HierarchiesSize;
        public string Encoding => column.Encoding;
        public string DataType => column.DataType;
    }
}
