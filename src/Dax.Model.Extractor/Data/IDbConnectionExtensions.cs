using Microsoft.AnalysisServices.AdomdClient;
using System.Data;
using System.Data.OleDb;

namespace Dax.Model.Extractor.Data
{
    internal static class IDbConnectionExtensions
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText)
        {
            return connection switch
            {
                AdomdConnection adomdConnection => new AdomdCommand(commandText, adomdConnection),
                OleDbConnection oledbConnection => new OleDbCommand(commandText, oledbConnection),
                TomConnection tomConnection => new TomCommand(commandText, tomConnection),
                _ => throw new ExtractorException(connection),
            };
        }
    }
}
