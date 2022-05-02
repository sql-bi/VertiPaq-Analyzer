using Microsoft.PowerBI.Api.Models;
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

namespace TestWpfPowerBI.Views
{
    /// <summary>
    /// Interaction logic for PbiMetadataView.xaml
    /// </summary>
    public partial class PbiMetadataView : UserControl
    {
        public PbiMetadataView()
        {
            InitializeComponent();
        }

        public Group SelectedGroup {
            get {
                return Groups.SelectedItem as Group;
            }
        }

        public Dataset SelectedDataset {
            get {
                return Datasets.SelectedItem as Dataset;
            }
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Datasets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatasetChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler GroupChanged;
        public event EventHandler DatasetChanged;
    }
}
