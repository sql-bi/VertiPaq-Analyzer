namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseRelationshipCommandHandler : CommandHandler
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
            .AddColumn(new TableColumn(new Markup("[yellow]Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Max From Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Max To Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Missing Keys[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Invalid Rows[/]").Centered()));

        var query = model.Relationships.Select((r) => new Row(r));
        if (orderBy.HasValue)
        {
            query = orderBy switch
            {
                -1 => query.OrderByDescending(_ => _.Name),
                +1 => query.OrderBy(_ => _.Name),
                2 => query.OrderByDescending(_ => _.Size),
                3 => query.OrderByDescending(_ => _.MaxFromCardinality),
                4 => query.OrderByDescending(_ => _.MaxToCardinality),
                5 => query.OrderByDescending(_ => _.MissingKeys),
                6 => query.OrderByDescending(_ => _.InvalidRows),
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
                new Text(r.Size.ToString("N0"), style).RightJustified(),
                new Text(r.MaxFromCardinality.ToString("N0"), style).RightJustified(),
                new Text(r.MaxToCardinality.ToString("N0"), style).RightJustified(),
                new Text(r.MissingKeys.ToString("N0"), style).RightJustified(),
                new Text(r.InvalidRows.ToString("N0"), style).RightJustified()
                );
        }

        table.Columns[0].Footer = new Markup($"[grey]{rows.Length:N0} items[/]").LeftJustified();
        table.Columns[1].Footer = new Markup($"[grey]{rows.Sum(_ => _.Size).ToSizeString():N0}[/]").RightJustified();
        // table.Columns[2].Footer = // MaxFromCardinality
        // table.Columns[3].Footer = // MaxToCardinality
        table.Columns[4].Footer = new Markup($"[grey]{rows.Sum(_ => _.MissingKeys):N0}[/]").RightJustified();
        table.Columns[5].Footer = new Markup($"[grey]{rows.Sum(_ => _.InvalidRows):N0}[/]").RightJustified();

        return table;
    }

    private sealed record Row(Metadata.Relationship relationship)
    {
        public bool IsHidden { get; } = relationship.IsHidden();
        public string Name { get; } = relationship.ToDisplayName();
        public long Size { get; } = relationship.UsedSize;
        public long MaxFromCardinality { get; } = relationship.FromColumn.ColumnCardinality;
        public long MaxToCardinality { get; } = relationship.ToColumn.ColumnCardinality;
        public long MissingKeys { get; } = relationship.MissingKeys;
        public long InvalidRows { get; } = relationship.InvalidRows;
    }
}
