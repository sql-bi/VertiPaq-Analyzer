using System.CommandLine;
using System.Data.Common;

namespace Dax.Vpax.CLI.Commands;

internal static class ExportCommandOptions
{
    public static readonly Argument<string> ConnectionStringArgument = new(
        name: "connection-string",
        description: "Connection string to the tabular model",
        parse: (result) =>
        {
            var connectionString = result.Tokens.Single().Value;
            {
                var builder = new DbConnectionStringBuilder(useOdbcRules: false);
                builder.ConnectionString = connectionString;

                if (!builder.ContainsKey("Initial Catalog"))
                    result.ErrorMessage = "The connection string does not contain the 'Initial Catalog' property.";
            }
            return connectionString; // always return the original value
        });

    public static readonly Argument<string> PathArgument = new(
        name: "path",
        description: "Path to write the VPAX file"
        );

    public static readonly Option<bool> OverwriteOption = new(
        name: "--overwrite",
        getDefaultValue: () => false,
        description: "Overwrite the VPAX file if it already exists"
        );

    public static readonly Option<bool> ExcludeTomOption = new(
        name: "--exclude-bim",
        getDefaultValue: () => false,
        description: "Exclude the BIM model (Model.bim) from the export"
        );

    public static readonly Option<bool> ExcludeVpaOption = new(
        name: "--exclude-vpa",
        getDefaultValue: () => false,
        description: "Exclude the VPA model (DaxVpaView.json) from the export"
        );
}
