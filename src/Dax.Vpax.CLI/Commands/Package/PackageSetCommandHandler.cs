using static Dax.Vpax.CLI.Commands.Package.PackageSetCommandOptions;

namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageSetCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var path = context.ParseResult.GetValueForArgument(PathArgument);

        if (!File.Exists(path))
        {
            AnsiConsole.MarkupLine($"[red]The package file does not exist or is not accessible. [[{path}]][/]");
            return Task.FromResult(context.ExitCode = 1);
        }

        var session = UserSession.Load();
        session.Package.Path = path;
        session.Save();

        return Task.FromResult(context.ExitCode);
    }
}
