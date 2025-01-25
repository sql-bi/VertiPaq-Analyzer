namespace Dax.Vpax.CLI.Commands.Browse;

internal static class CommonOptions
{
    public static readonly Option<FileInfo> VpaxOption = new(
        name: "--vpax",
        description: "Path to the VPAX file"
    )
    {
        ArgumentHelpName = "path",
        IsRequired = true,
    };

    public static readonly Option<bool> ExcludeHiddenOption = new(
        name: "--exclude-hidden",
        description: "Specify whether to exclude hidden objects"
        );

    public static readonly Option<int?> OrderByOption = new(
        name: "--order-by",
        description: "Specify the column number to order by"
        )
    {
        ArgumentHelpName = "number"
    };

    public static readonly Option<int?> TopOption = new(
        name: "--top",
        description: "Specify the maximum number of objects to display"
        )
    {
        ArgumentHelpName = "number"
    };

    //static CommonOptions()
    //{
    //    OrderByOption.AddCompletions(OrderByColumns);
    //}
}
