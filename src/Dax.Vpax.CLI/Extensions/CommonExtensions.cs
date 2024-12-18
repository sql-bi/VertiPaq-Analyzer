namespace Dax.Vpax.CLI.Extensions;

internal static class CommonExtensions
{
    public static string ToEmptyIfNull(this string? value)
    {
        return value ?? string.Empty;
    }

    public static string ToSizeString(this long size)
    {
        const double KB = 1024;
        const double MB = KB * 1024;
        const double GB = MB * 1024;

        if (size < KB) return $"{size} B";
        if (size < MB) return $"{size / KB:F2} KB";
        if (size < GB) return $"{size / MB:F2} MB";

        return $"{size / GB:F2} GB";
    }

    public static bool IsHidden(this Dax.Metadata.Relationship relationship)
    {
        return relationship.FromColumn.Table.IsHidden() || relationship.ToColumn.Table.IsHidden();
    }

    public static bool IsHidden(this Dax.Metadata.Table table)
    {
        return table.IsHidden || table.IsPrivate;
    }

    public static string ToDisplayName(this Dax.Metadata.Column column)
    {
        return $"{column.Table.TableName.Name}[{column.ColumnName.Name}]";
    }

    public static string ToDisplayName(this Dax.Metadata.Relationship relationship)
    {
        var from = relationship.FromColumn.ToDisplayName();
        var to = relationship.ToColumn.ToDisplayName();

        // Code copied from Dax.ViewModel.VpaRelationship.RelationshipFromToName
        var p1 = relationship.FromCardinalityType == null ? "<" : relationship.FromCardinalityType == "Many" ? "*" : "1";
        var p2 = relationship.CrossFilteringBehavior == null ? "=" : relationship.CrossFilteringBehavior == "BothDirections" ? "<>" : "<";
        var p3 = relationship.ToCardinalityType == null ? "=" : relationship.ToCardinalityType == "Many" ? "*" : "1";

        return $"{from} {p1}{p2}{p3} {to}";
    }
}
