using System;
using System.Data.Common;
using System.Globalization;

namespace Dax.Model.Extractor.Data
{
    public static class ConnectionStringUtils
    {
        public static string GetDataSource(string connectionString) => GetValue(connectionString, ConnectionStringKeywords.DataSource);

        public static string GetInitialCatalog(string connectionString) => GetValue(connectionString, ConnectionStringKeywords.InitialCatalog);

        public static string GetConnectionString(string serverNameOrConnectionString, string databaseName)
        {
            var builder = new DbConnectionStringBuilder(useOdbcRules: false);
            try {
                builder.ConnectionString = serverNameOrConnectionString;
            }
            catch {
                // Assume servername
                builder[ConnectionStringKeywords.Provider] = "MSOLAP";
                builder[ConnectionStringKeywords.DataSource] = serverNameOrConnectionString;
            }
            builder[ConnectionStringKeywords.InitialCatalog] = databaseName;
            return builder.ConnectionString;
        }

        private static string GetValue(string connectionString, string keyword)
        {
            var builder = new DbConnectionStringBuilder(useOdbcRules: false);
            builder.ConnectionString = connectionString;

            if (builder.TryGetValue(keyword, out object value))
                return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);

            return string.Empty; // default to empty string to keep the result consistent with the OleDbConnectionStringBuilder
        }
    }

    internal static class ConnectionStringKeywords
    {
        public const string Provider = "Provider";
        public const string DataSource = "Data Source";
        public const string InitialCatalog = "Initial Catalog";
    }
}
