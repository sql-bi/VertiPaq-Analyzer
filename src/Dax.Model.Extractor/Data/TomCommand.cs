using Microsoft.AnalysisServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Dax.Model.Extractor.Data
{
    internal class TomCommand : IDbCommand, IDisposable
    {
        private readonly TomConnection _connection;
        private AmoDataReader _reader;

        public TomCommand(TomConnection connection)
        {
            _connection = connection;
        }

        public TomCommand(string commandText, TomConnection connection)
            : this(connection)
        {
            CommandText = commandText;
        }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public IDbConnection Connection { get; set; }

        public IDataReader ExecuteReader()
        {
            var command = new XElement("Statement", CommandText).ToString();
            var properties = string.IsNullOrEmpty(_connection.Database) ? null : new Dictionary<string, string> { { "Catalog", _connection.Database } };

            _reader = _connection.Server.ExecuteReader(command, out var results, properties);

            if (results != null && results.ContainsErrors)
                throw new AmoException(GetMessages(results));

            return _reader;

            static string GetMessages(XmlaResultCollection xmlaResults)
            {
                var messages = xmlaResults.OfType<XmlaResult>().SelectMany((r) => r.Messages.OfType<XmlaMessage>()).Select((r) => r.Description).ToArray();
                return string.Join(Environment.NewLine, messages);
            }
        }

        public void Dispose()
        {
            if (_reader != null && !_reader.IsClosed) {
                _reader.Close();
            }
        }

        #region Methods and properties that throw a NotImplementedException

        public CommandType CommandType {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IDataParameterCollection Parameters {
            get { throw new NotImplementedException(); }
        }

        public IDbTransaction Transaction {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public UpdateRowSource UpdatedRowSource {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}