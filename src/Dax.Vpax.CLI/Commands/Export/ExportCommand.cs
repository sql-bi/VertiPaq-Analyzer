﻿using static Dax.Vpax.CLI.Commands.Export.ExportCommandOptions;

namespace Dax.Vpax.CLI.Commands.Export;

internal static class ExportCommand
{
    private static readonly ExportCommandHandler s_handler = new();

    internal static Command GetCommand()
    {
        var command = new Command("export", "Export a VPAX file from a tabular model")
        {
            PathArgument,
            ConnectionStringArgument,
            OverwriteOption,
            // advanced options
            ExcludeTomOption,
            ExcludeVpaOption,
            DirectQueryModeOption,
            DirectLakeModeOption,
            ColumnBatchSizeOption,
        };
        command.Handler = s_handler;
        return command;
    }
}