using System;
using System.Data;

namespace Dax.Model.Extractor;

public class ExtractorException : Exception
{
    public IDbConnection Connection { get; }
    public string DatabaseName { get; }

    public ExtractorException(IDbConnection connection)
    {
        Connection = connection;
    }

    public ExtractorException(IDbConnection connection, string databaseName)
    {
        Connection = connection;
        DatabaseName = databaseName;
    }
}
