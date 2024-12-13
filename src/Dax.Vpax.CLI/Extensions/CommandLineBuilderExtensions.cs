using System.CommandLine.Builder;
using System.CommandLine.Help;
using Dax.Vpax.CLI.Commands.Export;

namespace Dax.Vpax.CLI.Extensions;

internal static class CommandLineBuilderExtensions
{
    public static CommandLineBuilder UseVpaxDefaults(this CommandLineBuilder builder)
    {
        return builder.UseVersionOption().UseParseErrorReporting().CancelOnProcessTermination().UseCustomHelp();
        //.UseEnvironmentVariableDirective().UseParseDirective().UseSuggestDirective().RegisterWithDotnetSuggest().UseTypoCorrections().UseExceptionHandler()
    }

    private static CommandLineBuilder UseCustomHelp(this CommandLineBuilder builder)
    {
        return builder.UseHelp((context) =>
        {
            if (context.Command is RootVpaxCommand)
            {
                // Removes the arguments from the output help
                var subcommand = context.Command.Children.OfType<ExportCommand>().Single();
                context.HelpBuilder.CustomizeSymbol(subcommand, firstColumnText: subcommand.Name);
            }

            //if (context.Command.Handler is null)
            //{
            //    foreach (var child in context.Command.Children)
            //    {
            //        var isExperimental =
            //            child is Commands.Browse.BrowseCommand ||
            //            child is Commands.Package.PackageCommand;

            //        // Removes the arguments from the output help for all commands, adds [Experimental] to the command name if applicable
            //        var text = isExperimental ? $"{child.Name} [Experimental]" : child.Name;
            //        context.HelpBuilder.CustomizeSymbol(child, firstColumnText: text);
            //    }
            //}
        });
    }
}
