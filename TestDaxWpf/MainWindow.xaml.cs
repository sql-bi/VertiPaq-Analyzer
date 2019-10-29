using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.OleDb;
using Newtonsoft.Json;
using Microsoft.AnalysisServices.AdomdClient;

namespace TestDaxWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            // Read Dax.Model from JSON file
            string json = System.IO.File.ReadAllText(@"c:\temp\model.json");
            var m = JsonConvert.DeserializeObject<Dax.Model.Model>(json, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });
            */

            // Extract Dax.Model from SSAS connection
            var m = TestDaxModelHelper.GetDaxModel(@"localhost\tab17", "AdventureWorks");

            var vm = new Dax.ViewModel.VpaModel(m);
            treeviewTables.DataContext = vm;
            treeviewColumns.DataContext = vm;
            treeviewRelationhsips.DataContext = vm;
        }


        static class TestDaxModelHelper
        {
            public static Dax.Metadata.Model GetDaxModel(string serverName, string databaseName, bool readStatisticsFromData = true)
            {
                Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
                server.Connect(serverName);
                Microsoft.AnalysisServices.Database db = server.Databases[databaseName];
                Microsoft.AnalysisServices.Tabular.Model tomModel = db.Model;
                var daxModel = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(tomModel, "TestDaxModel", "0.1");

                var connectionString = GetConnectionString(serverName, databaseName);

                using (var connection = new AdomdConnection(connectionString)) {
                    // Populate statistics from DMV
                    Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(daxModel, connection, serverName, databaseName, "TestDaxModel", "0.1");

                    // Populate statistics by querying the data model
                    if (readStatisticsFromData) {
                        Dax.Metadata.Extractor.StatExtractor.UpdateStatisticsModel(daxModel, connection);
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
}
