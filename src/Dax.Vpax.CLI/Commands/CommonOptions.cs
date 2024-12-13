namespace Dax.Vpax.CLI.Commands;

internal static class CommonOptions
{
    public static readonly Option<string> PathOption = new(
        name: "--path",
        description: "Path to the VPAX package file. You can configure the default package using `vpax package set <path>`"
        );
}
