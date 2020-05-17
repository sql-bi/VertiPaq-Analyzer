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
using TestWpfPowerBI.Model;

using Microsoft.Win32;
using TestWpfPowerBI.Views;

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
            //PbiMetadataBinding = new PbiMetadataViewModel(new EventAggregator(), null);
            //ViewModelBinder.Bind(PbiMetadataBinding, this.PbiView, null);

            PbiMetadataTreeBinding = new PbiMetadataTreeViewModel(new EventAggregator(), null);
            ViewModelBinder.Bind(PbiMetadataTreeBinding, this.PbiTreeView, null);
        }

        // PbiMetadataViewModel PbiMetadataBinding = null;
        PbiMetadataTreeViewModel PbiMetadataTreeBinding = null;
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

                LoadGroups(PreloadDatasets.IsChecked.Value);
                // LoadDatasets(); // load all datasets from all groups
            }
        }

        private async void LoadGroups(bool preloadDatasets)
        {
            await Task.Run(() =>
            {
                DisplayStatus($"Loading groups...");
                var groups =
                    // from g in PowerBI_Tools.GetGroups()
                    from g in PowerBI_Tools.GetPbiGroups( null )
                    orderby g.Name
                    select g;

                if (preloadDatasets)
                {
                    foreach (var g in groups)
                    {
                        DisplayStatus($"Loading datasets for group {g.Name}...");
                        g.Group.Datasets = new BindableCollection<Dataset>(
                            from d in PowerBI_Tools.GetDatasets(g.Group)
                            orderby d.Name
                            select d
                        );
                    }
                    groups =
                        from g in groups
                        where g.Group.Datasets.Count > 0
                        orderby g.Name
                        select g;
                    DisplayStatus($"Loaded {groups.Count()} groups.", 3000);
                }

                // PbiMetadataBinding.PbiGroups = new BindableCollection<Group>(groups);
                PbiMetadataTreeBinding.PbiGroups = new BindableCollection<TreeViewPbiGroup>(groups);
            });
        }
        /*
        private async void LoadDatasets()
        {
            await Task.Run(() =>
            {
                PbiMetadataTreeBinding.PbiDatasets = new BindableCollection<Dataset>(
                    from g in PowerBI_Tools.GetDatasets()
                    orderby g.Name
                    select g
                );

                // PbiMetadataBinding.PbiDatasets = PbiMetadataTreeBinding.PbiDatasets;
            });
        }
        */
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await Authentication.LogoutAAD();
            AccessToken = null;
            PowerBI_Tools = null;
            this.Logout.Visibility = Visibility.Hidden;
            this.Login.Content = "Sign in";
            this.Login.IsEnabled = true;
        }

        /*
        private async void PbiView_GroupChanged(object sender, EventArgs e)
        {
            var selectedGroup = PbiView.SelectedGroup;
            if (selectedGroup == null) return;
            Log.Information($"Group:{selectedGroup.Name}");

            if (selectedGroup.Datasets == null)
            {
                await Task.Run(() =>
                {
                    DisplayStatus($"Loading datasets for group {selectedGroup.Name}...");
                    selectedGroup.Datasets = new BindableCollection<Dataset>(
                            from d in PowerBI_Tools.GetDatasets(selectedGroup)
                            orderby d.Name
                            select d
                        );
                    DisplayStatus();
                });
            }
            PbiMetadataBinding.PbiDatasets = selectedGroup.Datasets as BindableCollection<Dataset>;
            PbiMetadataTreeBinding.PbiDatasets = selectedGroup.Datasets as BindableCollection<Dataset>;
        }
        */
        private async void PbiTreeView_GroupChanged(object sender, EventArgs e)
        {
            var selectedGroup = PbiTreeView.SelectedGroup;
            if (selectedGroup == null) return;
            Log.Information($"Group:{selectedGroup.Name}");

            if (selectedGroup.Datasets == null)
            {
                await Task.Run(() =>
                {
                    DisplayStatus($"Loading datasets for group {selectedGroup.Name}...");
                    selectedGroup.Datasets = new BindableCollection<Dataset>(
                            from d in PowerBI_Tools.GetDatasets(selectedGroup)
                            orderby d.Name
                            select d
                        );
                    DisplayStatus();
                });
            }
        }
        
        private void DisplayStatus( string status = "", int millisecondsToHide = -1 )
        {
            this.Dispatcher.Invoke(new System.Action(() =>
               {
                   Logging.Text = status;

                   if (millisecondsToHide >= 0)
                   {
                       Task.Run(new System.Action(() =>
                       {
                           Task.Delay(millisecondsToHide).Wait();
                           this.Dispatcher.Invoke(new System.Action(() => Logging.Text = ""));
                       }));
                   }
               }));
        }

        private System.Collections.Concurrent.ConcurrentDictionary<Dataset,Dax.ViewModel.VpaModel> CacheVpaModels = 
            new System.Collections.Concurrent.ConcurrentDictionary<Dataset,Dax.ViewModel.VpaModel>();

        /*
        private async void PbiView_DatasetChanged(object sender, EventArgs e)
        {
            var selectedDataset = PbiView.SelectedDataset;
            if (selectedDataset == null) return;
            Log.Information($"Dataset:{selectedDataset.Name}");

            Dax.ViewModel.VpaModel newModel = null;
            if (CacheVpaModels.TryGetValue(selectedDataset, out newModel))
            {
                CurrentVpaModel = newModel;
                return;
            }

            // Loader and show VPAX
            DisplayStatus($"Loading VertiPaq Analyzer from dataset {selectedDataset.Name} ...");
            await Task.Run(() =>
            {
                newModel = GetVpaModel(selectedDataset.Name, selectedDataset.Id);
            });
            if (newModel != null)
            {
                CacheVpaModels.AddOrUpdate(selectedDataset, newModel, (key, oldValue) => newModel);
                CurrentVpaModel = newModel;
                DisplayStatus("");
            }
            else
            {
                DisplayStatus($"Error reading VertiPaq Analyzer from dataset {selectedDataset.Name}");
            }
        }
        */

        private async void PbiTreeView_DatasetChanged(object sender, EventArgs e)
        {
            var selectedDataset = PbiTreeView.SelectedDataset;
            if (selectedDataset == null) return;
            Log.Information($"Dataset:{selectedDataset.Name}");

            Dax.ViewModel.VpaModel newModel = null;
            if (CacheVpaModels.TryGetValue(selectedDataset, out newModel))
            {
                CurrentVpaModel = newModel;
                return;
            }

            // Loader and show VPAX
            DisplayStatus($"Loading VertiPaq Analyzer from dataset {selectedDataset.Name} ...");
            await Task.Run(() =>
            {
                newModel = GetVpaModel(selectedDataset.Name, selectedDataset.Id);
            });
            if (newModel != null)
            {
                CacheVpaModels.AddOrUpdate(selectedDataset, newModel, (key, oldValue) => newModel);
                CurrentVpaModel = newModel;
                DisplayStatus("");
            }
            else
            {
                DisplayStatus($"Error reading VertiPaq Analyzer from dataset {selectedDataset.Name}");
            }
        }

        private Dax.ViewModel.VpaModel GetVpaModel(string name, string id)
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
                    var viewVpa = new Dax.ViewVpaExport.Model(CurrentVpaModel.Model);
                    DisplayStatus($"Saving {filename} ...");
                    Dax.Vpax.Tools.VpaxTools.ExportVpax(filename, CurrentVpaModel.Model, viewVpa);
                    DisplayStatus($"VPAX saved to {filename}");
                }
            }
        }

        private async void PbiTreeView_GroupExpanded(object sender, EventArgs e)
        {
            var expandedGroup = (e as PbiMetadataTreeView.GroupExpandedEventArgs)?.Group;
            if (expandedGroup == null) return;
            Log.Information($"Group:{expandedGroup.Name}");

            var _group = expandedGroup.Group;
            if (_group.Datasets == null)
            {
                await Task.Run(() =>
                {
                    DisplayStatus($"Loading datasets for group {_group.Name}...");
                    _group.Datasets = new BindableCollection<Dataset>(
                            from d in PowerBI_Tools.GetDatasets(_group)
                            orderby d.Name
                            select d
                        );

                    // Enforce refresh? Not working...
                    var savedBinding = PbiMetadataTreeBinding.PbiGroups;
                    PbiMetadataTreeBinding.PbiGroups = null;

                    this.Dispatcher.Invoke(new System.Action(() =>
                    {
                        PbiMetadataTreeBinding.PbiGroups = null;
                    }));
                    this.Dispatcher.Invoke(new System.Action(() =>
                    {
                        Task.Delay(2000).Wait();
                        PbiMetadataTreeBinding.PbiGroups = savedBinding;
                    }));


                    DisplayStatus();
                });
            }
        }
    }
}
