using System;
using System.Data;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace Dax.Model.Extractor.Data
{
    public class TomConnection : IDbConnection, IDisposable
    {
        public TomConnection(Tom.Server server, string database)
        {
            Server = server;
            Database = database;
        }

        public Tom.Server Server { get; }

        public string Database { get; }

        public IDbCommand CreateCommand()
        {
            return new TomCommand(this);
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        public ConnectionState State => Server.GetConnectionState(false);

        public string ConnectionString {
            get { return Server.ConnectionString; }
            set { throw new InvalidOperationException(); }
        }
        public int ConnectionTimeout => Server.ConnectionInfo.Timeout;

        #region Methods and properties that throw a NotSupportedException

        public IDbTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotSupportedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
