using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api.Models;
using System.ComponentModel.Composition;
using TestWpfPowerBI.Model;
using Caliburn.Micro;
using Serilog;

namespace TestWpfPowerBI.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class PbiMetadataTreeViewModel :
        ToolWindowBase // ToolPaneBaseViewModel
        // , IHandle<UpdateGlobalOptions>
        //, IDragSource
        // , IMetadataPane
    {
        private readonly IEventAggregator _eventAggregator;
        public DocumentViewModel CurrentDocument { get; }

        [ImportingConstructor]
        public PbiMetadataTreeViewModel(IEventAggregator eventAggregator, DocumentViewModel currentDocument)
        {
            Log.Debug("{class} {method} {message}", "PbiMetadataViewModel", "ctor", "start");
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            CurrentDocument = currentDocument;
            Log.Debug("{class} {method} {message}", "PbiMetadataViewModel", "ctor", "end");
        }


        private BindableCollection<TreeViewPbiGroup> _pbiGroups;
        public BindableCollection<TreeViewPbiGroup> PbiGroups {
            get {
                return _pbiGroups;
            }
            set {
                _pbiGroups = value;
                NotifyOfPropertyChange(() => PbiGroups);
                // NotifyOfPropertyChange(() => PbiDatasets);
            }
        }

        /*
        private BindableCollection<Dataset> _pbiDatasets;
        public BindableCollection<Dataset> PbiDatasets {
            get {
                return _pbiDatasets;
            }
            set {
                _pbiDatasets = value;
                NotifyOfPropertyChange(() => PbiDatasets);
            }
        }
        */
    }
}
