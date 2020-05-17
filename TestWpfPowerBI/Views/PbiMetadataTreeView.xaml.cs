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
using System.Windows.Shapes;
using Microsoft.PowerBI.Api.Models;
using TestWpfPowerBI.Model;
using Serilog;

namespace TestWpfPowerBI.Views
{
    /// <summary>
    /// Interaction logic for PbiMetadataTreeView.xaml
    /// </summary>
    public partial class PbiMetadataTreeView : UserControl
    {
        public PbiMetadataTreeView()
        {
            InitializeComponent();
        }

        private void PbiTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Group)
            {
                SelectedGroup = e.NewValue as Group;
                SelectedDataset = null;
                GroupChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (e.NewValue is Dataset)
            {
                SelectedGroup = null;
                SelectedDataset = e.NewValue as Dataset;
                DatasetChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler GroupChanged;
        public event EventHandler DatasetChanged;
        public event EventHandler GroupExpanded;

        public Group SelectedGroup { get; private set; }

        public Dataset SelectedDataset { get; private set; }

        public class GroupExpandedEventArgs : EventArgs
        {
            public GroupExpandedEventArgs(TreeViewPbiGroup pbiGroup)
            {
                this.Group = pbiGroup;
            }

            public TreeViewPbiGroup Group { get;  }
        }
        private void PbiTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            var group = (e.OriginalSource as TreeViewItem)?.DataContext as TreeViewPbiGroup;
            if (group == null)
            {
                Log.Information("Unknown expanded event");
                return;
            }
            if (group?.Group.Datasets == null)
            {
                Log.Information($"Loading {group.Name} group");
                GroupExpanded?.Invoke(this, new GroupExpandedEventArgs(group));
            }
        }
    }
}
