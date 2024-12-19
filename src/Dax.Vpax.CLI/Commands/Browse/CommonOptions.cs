namespace Dax.Vpax.CLI.Commands.Browse;

internal static class CommonOptions
{
    private static string[] OrderByColumns => ["c", "cardinality", "n", "name", "s", "size"];

    public static readonly Option<bool> ExcludeHiddenOption = new(
        name: "--exclude-hidden",
        description: "Specify whether to exclude hidden objects"
        );

    public static readonly Option<string> OrderByOption = new(
        name: "--orderby",
        getDefaultValue: () => "size",
        description: "Specify the column to sort by"
        );

    public static readonly Option<int?> TopOption = new(
        name: "--top",
        description: "Specify the maximum number of objects to display"
        );

    static CommonOptions()
    {
        OrderByOption.AddCompletions(OrderByColumns);
    }
}
