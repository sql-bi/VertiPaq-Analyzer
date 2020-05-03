using Microsoft.Identity.Client;
using Caliburn.Micro;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TestWpfPowerBI.PowerBI;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using TestWpfPowerBI.ViewModels;
using Microsoft.Win32;

namespace TestWpfPowerBI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Logout.Visibility = Visibility.Hidden;
            this.Export.Visibility = Visibility.Hidden;
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger();

            // Create viewmodel classes (simulate Caliburn)
            Log.Information("Binding PbiMetadata");
            PbiMetadataBinding = new PbiMetadataViewModel(new EventAggregator(), null);
            ViewModelBinder.Bind(PbiMetadataBinding, this.PbiView, null);
        }

        PbiMetadataViewModel PbiMetadataBinding = null;
        VertiPaqAnalyzerViewModel VertiPaqAnalyzerBinding = null;
        public Dax.ViewModel.VpaModel CurrentVpaModel {
            get {
                return VertiPaqAnalyzerBinding?.ViewModel;
            }
            set {
                if (VertiPaqAnalyzerBinding == null) 
                {
                    Log.Information("Binding VertiPaq Analyzer");
                    VertiPaqAnalyzerBinding = new VertiPaqAnalyzerViewModel(value, new EventAggregator(), null, null);
                    ViewModelBinder.Bind(VertiPaqAnalyzerBinding, this.VpaxView, null);
                }
                else
                {
                    VertiPaqAnalyzerBinding.ViewModel = value;
                }
                this.Export.Visibility = (VertiPaqAnalyzerBinding.ViewModel == null) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private string AccessToken = null;
        MetadataTools PowerBI_Tools = null;

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var result = await Authentication.LoginAAD();
            if (result != null)
            {
                AccessToken = result.AccessToken;
                PowerBI_Tools = new MetadataTools(AccessToken);
                this.Logout.Visibility = Visibility.Visible;
                this.Login.Content = result.Account.Username;
                this.Login.IsEnabled = false;
                LoadGroups();
                // LoadDatasets(); // load all datasets from all groups
            }
        }

        private async void LoadGroups()
        {
            await Task.Run(() =>
            {
                PbiMetadataBinding.PbiGroups = new BindableCollection<Group>(
                    from g in PowerBI_Tools.GetGroups() 
                    orderby g.Name 
                    select g
                );
            });
        }

        private async void LoadDatasets()
        {
            await Task.Run(() =>
            {
                PbiMetadataBinding.PbiDatasets = new BindableCollection<Dataset>(
                    from g in PowerBI_Tools.GetDatasets()
                    orderby g.Name
                    select g
                );
            });
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await Authentication.LogoutAAD();
            AccessToken = null;
            PowerBI_Tools = null;
            this.Logout.Visibility = Visibility.Hidden;
            this.Login.Content = "Sign in";
            this.Login.IsEnabled = true;
        }

        private async void PbiView_GroupChanged(object sender, EventArgs e)
        {
            var selectedGroup = PbiView.SelectedGroup;
            if (selectedGroup == null) return;
            Log.Information($"Group:{selectedGroup.Name}");

            DisplayStatus("Loading datasets...");
            await Task.Run(() =>
            {
                PbiMetadataBinding.PbiDatasets = new BindableCollection<Dataset>(
                    from g in PowerBI_Tools.GetDatasets(selectedGroup)
                    orderby g.Name
                    select g
                );
            });
            DisplayStatus();
        }

        private void DisplayStatus( string status = "" )
        {
            Logging.Text = status;
        }
        private async void PbiView_DatasetChanged(object sender, EventArgs e)
        {
            var selectedDataset = PbiView.SelectedDataset;
            if (selectedDataset == null) return;
            Log.Information($"Dataset:{selectedDataset.Name}");

            // Loader and show VPAX
            DisplayStatus($"Loading VertiPaq Analyzer from dataset {selectedDataset.Name} ...");
            Dax.ViewModel.VpaModel newModel = null;
            await Task.Run(() =>
            {
                newModel = TestPbiShared(selectedDataset.Name, selectedDataset.Id);
            });
            if (newModel != null)
            {
                CurrentVpaModel = newModel;
                DisplayStatus("");
            }
            else
            {
                DisplayStatus($"Error reading VertiPaq Analyzer from dataset {selectedDataset.Name}");
            }
        }

        private Dax.ViewModel.VpaModel TestPbiShared(string name, string id)
        {
            const string dataSource = "pbiazure://api.powerbi.com";
            const string identityProvider = "https://login.microsoftonline.com/common, https://analysis.windows.net/powerbi/api, 929d0ec0-7a41-4b1e-bc7c-b754a28bddcc;";
            string initialCatalog = id;
            string databaseName = "sobe_wowvirtualserver-" + initialCatalog;

            const string serverName = dataSource;

            var connStr = String.Format(
                "Provider=MSOLAP;Identity Provider={0};Data Source={1};Initial Catalog={2};",
                identityProvider,
                dataSource,
                initialCatalog
                );

            var conn = new System.Data.OleDb.OleDbConnection(connStr);

            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(m, conn, serverName, databaseName, "Test", "0.1");
            return new Dax.ViewModel.VpaModel(m);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVpaModel != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "VPAX file (*.vpax)|*.vpax";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    DisplayStatus($"Saving {filename} ...");
                    Dax.Vpax.Tools.VpaxTools.ExportVpax(filename, CurrentVpaModel.Model);
                    DisplayStatus($"VPAX saved to {filename}");
                }
            }
        }
    }
}
