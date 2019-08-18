using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Newtonsoft.Json;

namespace TestDaxWpf
{
    static class TestDaxModelHelper
    {
        public static Dax.Model.Model GetDaxModel(string serverName, string databaseName, bool readStatisticsFromData = true)
        {
            Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
            server.Connect(serverName);
            Microsoft.AnalysisServices.Database db = server.Databases[databaseName];
            Microsoft.AnalysisServices.Tabular.Model tomModel = db.Model;
            var daxModel = Dax.Model.Extractor.TomExtractor.GetDaxModel(tomModel, "TestDaxModel", "0.1");

            var connectionString = GetConnectionString(serverName, databaseName);

            using (var connection = new OleDbConnection(connectionString)) {
                // Populate statistics from DMV
                Dax.Model.Extractor.DmvExtractor.PopulateFromDmv(daxModel, connection, databaseName, "TestDaxModel", "0.1");

                // Populate statistics by querying the data model
                if (readStatisticsFromData) {
                    Dax.Model.Extractor.StatExtractor.UpdateStatisticsModel(daxModel, connection);
                }
            }
            return daxModel;
        }

        private static string GetConnectionString(string dataSourceOrConnectionString, string databaseName)
        {
            var csb = new OleDbConnectionStringBuilder();
            try {
                csb.ConnectionString = dataSourceOrConnectionString;
            }
            catch {
                // Assume servername
                csb.Provider = "MSOLAP";
                csb.DataSource = dataSourceOrConnectionString;
            }
            csb["Initial Catalog"] = databaseName;
            return csb.ConnectionString;
        }
    }
}
