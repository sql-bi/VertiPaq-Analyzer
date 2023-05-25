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

        #region Methods and properties that throw a NotImplementedException

        public string ConnectionString {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int ConnectionTimeout {
            get { throw new NotImplementedException(); }
        }

        public ConnectionState State {
            get { throw new NotImplementedException(); }
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
