using System.ComponentModel.Composition;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWpfPowerBI.Model;
using TestWpfPowerBI.Events;
using TestWpfPowerBI.Interfaces;
using Serilog;
using Microsoft.PowerBI.Api.Models;

namespace TestWpfPowerBI.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class PbiMetadataViewModel : ToolWindowBase
        , IHandle<DocumentConnectionUpdateEvent>
        , IHandle<UpdateGlobalOptions>
        , IViewAware
    {
        private readonly IEventAggregator _eventAggregator;
        public DocumentViewModel CurrentDocument { get; }

        [ImportingConstructor]
        public PbiMetadataViewModel(IEventAggregator eventAggregator, DocumentViewModel currentDocument)
        {
            Log.Debug("{class} {method} {message}", "PbiMetadataViewModel", "ctor", "start");
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            CurrentDocument = currentDocument;
            Log.Debug("{class} {method} {message}", "PbiMetadataViewModel", "ctor", "end");
        }

        private BindableCollection<Group> _pbiGroups;
        public BindableCollection<Group> PbiGroups {
            get {
                return _pbiGroups;
            }
            set {
                _pbiGroups = value;
                NotifyOfPropertyChange(() => PbiGroups);
                NotifyOfPropertyChange(() => PbiDatasets);
            }
        }

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

        public void Handle(DocumentConnectionUpdateEvent message)
        {
            // TODO connect VPA data
            Log.Information("VertiPaq Analyzer Handle DocumentConnectionUpdateEvent call");
        }

        public void Handle(UpdateGlobalOptions message)
        {
            // NotifyOfPropertyChange(() => ShowTraceColumns);
            Log.Information("VertiPaq Analyzer Handle UpdateGlobalOptions call");
        }

    }

}
