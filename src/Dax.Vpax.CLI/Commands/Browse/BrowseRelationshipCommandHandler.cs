namespace Dax.Vpax.CLI.Commands.Browse;

internal sealed class BrowseRelationshipCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var model = GetCurrentModel(context);
        if (model is null)
            return Task.FromResult(context.ExitCode);

        AnsiConsole.Write(GetRelationshipView(context, model));
        return Task.FromResult(context.ExitCode);
    }

    private IRenderable GetRelationshipView(InvocationContext context, Metadata.Model model)
    {
        var top = context.ParseResult.GetValueForOption(CommonOptions.TopOption);
        var excludeHidden = context.ParseResult.GetValueForOption(CommonOptions.ExcludeHiddenOption);
        var orderBy = context.ParseResult.GetValueForOption(CommonOptions.OrderByOption);

        var table = new Spectre.Console.Table().BorderColor(Color.Yellow)
            .AddColumn(new TableColumn(new Markup("[yellow]Name[/]").Centered()).NoWrap())
            .AddColumn(new TableColumn(new Markup("[yellow]Size[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Max From Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Max To Cardinality[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Missing Keys[/]").Centered()))
            .AddColumn(new TableColumn(new Markup("[yellow]Invalid Rows[/]").Centered()));

        var query = orderBy switch
        {
            "name" or "n" => model.Relationships.OrderBy((r) => r.ToDisplayName()),
            "cardinality" or "c" => model.Relationships.OrderByDescending((r) => r.FromColumn.ColumnCardinality),
            "size" or "s" => model.Relationships.OrderByDescending((r) => r.UsedSize),
            _ => model.Relationships.AsEnumerable()
        };

        if (excludeHidden) query = query.Where((r) => !r.IsHidden());
        if (top.HasValue) query = query.Take(top.Value);

        var modelSize = model.Tables.Sum((t) => t.TableSize);
        var relationships = query.ToArray();

        foreach (var r in relationships)
        {
            var style = r.IsHidden() ? new Style(foreground: Color.Grey) : Style.Plain;

            table.AddRow(
                new Text(r.ToDisplayName(), style).LeftJustified(),
                new Text(r.UsedSize.ToString("N0"), style).RightJustified(),
                new Text(r.FromColumn.ColumnCardinality.ToString("N0"), style).RightJustified(),
                new Text(r.ToColumn.ColumnCardinality.ToString("N0"), style).RightJustified(),
                new Text(r.MissingKeys.ToString("N0"), style).RightJustified(),
                new Text(r.InvalidRows.ToString("N0"), style).RightJustified()
                );
        }

        table.Columns[0].Footer = new Markup($"[grey]{relationships.Length:N0} items[/]").LeftJustified();
        table.Columns[1].Footer = new Markup($"[grey]{relationships.Sum((r) => r.UsedSize).ToSizeString():N0}[/]").RightJustified();
        table.Columns[4].Footer = new Markup($"[grey]{relationships.Sum((r) => r.MissingKeys):N0}[/]").RightJustified();
        table.Columns[5].Footer = new Markup($"[grey]{relationships.Sum((r) => r.InvalidRows):N0}[/]").RightJustified();

        return table;
    }
}
