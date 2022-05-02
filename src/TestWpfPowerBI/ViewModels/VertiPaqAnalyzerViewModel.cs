﻿using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows.Data;
using System;
using System.ComponentModel;
using Serilog;
//using DaxStudio.UI.Events;
//using DaxStudio.UI.Model;
//using DaxStudio.UI.Interfaces;
//using System.Windows.Input;
//using DaxStudio.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using TestWpfPowerBI.Model;
using TestWpfPowerBI.Events;
using TestWpfPowerBI.Interfaces;
using Dax.ViewModel;

namespace TestWpfPowerBI.ViewModels
{



    public class TableGroupDescription : PropertyGroupDescription, IComparable
    {
        public TableGroupDescription(string propertyName) : base(propertyName) { }

        public override bool NamesMatch(object groupName, object itemName)
        {
            var groupTable = (VpaTableViewModel)groupName;
            var itemTable = (VpaTableViewModel)itemName;
            return base.NamesMatch(groupTable.TableName, itemTable.TableName);
        }
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            VpaRelationshipViewModel rel = item as VpaRelationshipViewModel;
            return item is VpaColumnViewModel col ? col.Table : rel.Table;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public class RelationshipGroupDescription : PropertyGroupDescription
    {
        public RelationshipGroupDescription(string propertyName) : base(propertyName) { }

        public override bool NamesMatch(object groupName, object itemName)
        {
            var groupTable = (VpaTableViewModel)groupName;
            var itemTable = (VpaTableViewModel)itemName;
            return base.NamesMatch(groupTable.TableName, itemTable.TableName);
        }
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            var rel = item as VpaRelationshipViewModel;
            return rel.Table;
        }

    }


    public class PartitionGroupDescription : PropertyGroupDescription
    {
        public PartitionGroupDescription(string propertyName) : base(propertyName) { }

        public override bool NamesMatch(object groupName, object itemName)
        {
            var groupTable = (VpaTableViewModel)groupName;
            var itemTable = (VpaTableViewModel)itemName;
            return base.NamesMatch(groupTable.TableName, itemTable.TableName);
        }
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            var partition = item as VpaPartitionViewModel;
            return partition.Table;
        }

    }


    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class VertiPaqAnalyzerViewModel : ToolWindowBase
        , IHandle<DocumentConnectionUpdateEvent>
        , IHandle<UpdateGlobalOptions>
        , IViewAware
    {

        private readonly IEventAggregator _eventAggregator;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IGlobalOptions _globalOptions;
#pragma warning restore IDE0052 // Remove unread private members

        [ImportingConstructor]
        public VertiPaqAnalyzerViewModel(Dax.ViewModel.VpaModel viewModel, IEventAggregator eventAggregator, DocumentViewModel currentDocument, IGlobalOptions options)
        {
            Log.Debug("{class} {method} {message}", "VertiPaqAnalyzerViewModel", "ctor", "start");
            this.ViewModel = viewModel;
            _globalOptions = options;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            CurrentDocument = currentDocument;
            Log.Debug("{class} {method} {message}", "VertiPaqAnalyzerViewModel", "ctor", "end");
        }

        public override void TryClose(bool? dialogResult = null)
        {
            // unsubscribe from event aggregator
            _eventAggregator.Unsubscribe(this);
            base.TryClose(dialogResult);
        }

        private VpaModel _viewModel;
        public Dax.ViewModel.VpaModel ViewModel {
            get { return _viewModel; }
            set {
                _viewModel = value;
                _groupedColumns = null;
                _sortedColumns = null;
                _groupedRelationships = null;
                _groupedPartitions = null;
                SummaryViewModel = new VpaSummaryViewModel(this);
                NotifyOfPropertyChange(() => ViewModel);
                NotifyOfPropertyChange(() => GroupedColumns);
                NotifyOfPropertyChange(() => SortedColumns);
                NotifyOfPropertyChange(() => GroupedRelationships);
                NotifyOfPropertyChange(() => TreeviewColumns);
                NotifyOfPropertyChange(() => TreeviewRelationships);
                NotifyOfPropertyChange(() => GroupedPartitions);
                NotifyOfPropertyChange(() => SummaryViewModel);
            }
        }

        private ICollectionView _groupedColumns;
        public ICollectionView GroupedColumns {
            get {
                if (_groupedColumns == null)
                {
                    var cols = ViewModel.Tables.Select(t => new VpaTableViewModel(t, this)).SelectMany(t => t.Columns);
                    _groupedColumns = CollectionViewSource.GetDefaultView(cols);
                    _groupedColumns.GroupDescriptions.Add(new TableGroupDescription("Table"));
                    _groupedColumns.SortDescriptions.Add(new SortDescription("TotalSize", ListSortDirection.Descending));
                }
                return _groupedColumns;
            }
        }

        private ICollectionView _groupedRelationships;

        public VpaSummaryViewModel SummaryViewModel { get; private set; }

        public ICollectionView GroupedRelationships {
            get {
                if (_groupedRelationships == null)
                {
                    var rels = ViewModel.TablesWithFromRelationships.Select(t => new VpaTableViewModel(t, this)).SelectMany(t => t.RelationshipsFrom);
                    _groupedRelationships = CollectionViewSource.GetDefaultView(rels);
                    _groupedRelationships.GroupDescriptions.Add(new RelationshipGroupDescription("Table"));
                    _groupedRelationships.SortDescriptions.Add(new SortDescription("UsedSize", ListSortDirection.Descending));
                }
                return _groupedRelationships;
            }
        }

        private ICollectionView _groupedPartitions;
        public ICollectionView GroupedPartitions {
            get {
                if (_groupedPartitions == null)
                {
                    var partitions = from t in ViewModel.Tables
                                     from p in t.Partitions
                                     select new VpaPartitionViewModel(p, new VpaTableViewModel(t, this), this);

                    _groupedPartitions = CollectionViewSource.GetDefaultView(partitions);
                    _groupedPartitions.GroupDescriptions.Add(new PartitionGroupDescription("Table"));
                    _groupedPartitions.SortDescriptions.Add(new SortDescription("RowsCount", ListSortDirection.Descending));
                }
                return _groupedPartitions;
            }
        }

        private ICollectionView _sortedColumns;
        public ICollectionView SortedColumns {
            get {
                if (_sortedColumns == null)
                {
                    long maxSize = ViewModel.Columns.Max(c => c.TotalSize);
                    var cols = ViewModel.Columns.Select(c => new VpaColumnViewModel(c) { MaxColumnTotalSize = maxSize });

                    _sortedColumns = CollectionViewSource.GetDefaultView(cols);
                    _sortedColumns.SortDescriptions.Add(new SortDescription(nameof(VpaColumn.TotalSize), ListSortDirection.Descending));
                }
                return _sortedColumns;

            }
        }

        public IEnumerable<VpaTable> TreeviewTables { get { return ViewModel.Tables; } }
        public IEnumerable<VpaColumn> TreeviewColumns { get { return ViewModel.Columns; } }
        public IEnumerable<VpaTable> TreeviewRelationships { get { return ViewModel.TablesWithFromRelationships; } }

        // TODO: we might add the database name here
        public override string Title {
            get { return "VertiPaq Analyzer Preview"; }
        }

        public void Handle(DocumentConnectionUpdateEvent message)
        {

            // TODO connect VPA data
            Log.Information("VertiPaq Analyzer Handle DocumentConnectionUpdateEvent call");
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void MouseDoubleClick(object sender )//, MouseButtonEventArgs e)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            System.Diagnostics.Debug.WriteLine("clicked!");
        }

        public void Handle(UpdateGlobalOptions message)
        {
            // NotifyOfPropertyChange(() => ShowTraceColumns);
            Log.Information("VertiPaq Analyzer Handle UpdateGlobalOptions call");
        }

        public void SortTableColumn(System.Windows.Controls.DataGridSortingEventArgs e)
        {
            if (SortColumn == e.Column.SortMemberPath) { SortDirection *= -1; }
            else { SortDirection = 1; }
            SortColumn = e.Column.SortMemberPath;
        }

        public string SortColumn { get; set; }
        public int SortDirection { get; set; }
        public DocumentViewModel CurrentDocument { get; }
    }

    public class VpaColumnViewModel
    {
        readonly VpaColumn _col;

        public VpaColumnViewModel(VpaColumn col)
        {
            _col = col;
        }
        public VpaColumnViewModel(VpaColumn col, VpaTableViewModel table)
        {
            _col = col;
            Table = table;
        }
        public VpaTableViewModel Table { get; }
        public string TableColumnName => _col.TableColumnName;
        public long TableRowsCount => _col.TableRowsCount;
        public long TableSize => Table.TableSize;
        public string ColumnName => _col.ColumnName;
        public string TypeName => _col.TypeName;
        public long ColumnCardinality => _col.ColumnCardinality;
        public string Encoding => _col.Encoding;
        public long DataSize => _col.DataSize;
        public long DictionarySize => _col.DictionarySize;
        public long HierarchiesSize => _col.HierarchiesSize;
        public long TotalSize => _col.TotalSize;
        public double PercentageDatabase => _col.PercentageDatabase;
        public double PercentageTable => _col.PercentageTable;
        public long SegmentsNumber => _col.SegmentsNumber;
        public long PartitionsNumber => _col.PartitionsNumber;
        public int? SegmentsPageable => _col.SegmentsPageable;
        public int? SegmentsResident => _col.SegmentsResident;
        public double? SegmentsAverageTemperature => _col.SegmentsAverageTemperature * 1000;
        public DateTime? SegmentsLastAccessed => _col.SegmentsLastAccessed;
        public long ColumnsNumber => 1;
        //public long MaxColumnCardinality { get; set; }
        public long MaxColumnTotalSize { get; set; }
        public double PercentOfMaxTotalSize => Table == null ? 0 : TotalSize / (double)Table.ColumnMaxTotalSize;
        public double PercentOfMaxCardinality => Table == null ? 0 : ColumnCardinality / (double)Table.ColumnsMaxCardinality;
        public double PercentOfMaxTotalDBSize => MaxColumnTotalSize == 0 ? 0 : TotalSize / (double)MaxColumnTotalSize;
    }

    public class VpaRelationshipViewModel
    {
        readonly VpaRelationship _rel;
        public VpaRelationshipViewModel(VpaRelationship rel, VpaTableViewModel table)
        {
            Table = table;
            _rel = rel;
        }
        public VpaTableViewModel Table { get; }
        public string RelationshipFromToName => _rel.RelationshipFromToName;
        public long UsedSize => _rel.UsedSize;
        public long FromColumnCardinality => _rel.FromColumnCardinality;
        public long ToColumnCardinality => _rel.ToColumnCardinality;
        public long MissingKeys => _rel.MissingKeys;
        public long InvalidRows => _rel.InvalidRows;
        public string SampleReferentialIntegrityViolations => _rel.SampleReferentialIntegrityViolations;
        public double OneToManyRatio => _rel.OneToManyRatio;
    }

    public class VpaTableViewModel : IComparable
    {
        private readonly VpaTable _table;
        private readonly VertiPaqAnalyzerViewModel _parentViewModel;
        public VpaTableViewModel(VpaTable table, VertiPaqAnalyzerViewModel parentViewModel)
        {
            _table = table;
            _parentViewModel = parentViewModel;
            Columns = _table.Columns.Select(c => new VpaColumnViewModel(c, this));
            ColumnMaxTotalSize = Columns.Max(c => c.TotalSize);
            ColumnsMaxCardinality = Columns.Max(c => c.ColumnCardinality);
            RelationshipsFrom = _table.RelationshipsFrom.Select(r => new VpaRelationshipViewModel(r, this));
            if (RelationshipsFrom.Count() > 0)
            {
                RelationshipMaxFromCardinality = RelationshipsFrom.Max(r => r.FromColumnCardinality);
                RelationshipMaxToCardinality = RelationshipsFrom.Max(r => r.ToColumnCardinality);
                RelationshipMaxOneToManyRatio = RelationshipsFrom.Max(r => r.OneToManyRatio);
            }
        }

        public string TableName => _table.TableName;
        public long TableSize => _table.TableSize;
        public string ColumnsEncoding => _table.ColumnsEncoding;
        public string ColumnsTypeName => "-";
        public double PercentageDatabase => _table.PercentageDatabase;
        public long ColumnsTotalSize => _table.ColumnsTotalSize;
        public long ColumnsDataSize => _table.ColumnsDataSize;
        public long ColumnsDictionarySize => _table.ColumnsDictionarySize;
        public long ColumnsHierarchySize => _table.ColumnsHierarchiesSize;
        public long RelationshipSize => _table.RelationshipsSize;
        public long UserHierarchiesSize => _table.UserHierarchiesSize;
        public long ColumnsNumber => _table.ColumnsNumber;
        public long RowsCount => _table.RowsCount;
        public long SegmentsNumber => _table.SegmentsNumber;
        public long PartitionsNumber => _table.PartitionsNumber;
        public long SegmentsTotalNumber => _table.SegmentsTotalNumber;
        public int? SegmentsPageable => _table.SegmentsPageable;
        public int? SegmentsResident => _table.SegmentsResident;
        public double? SegmentsAverageTemperature => _table.SegmentsAverageTemperature * 1000;
        public DateTime? SegmentsLastAccessed => _table.SegmentsLastAccessed;

        public long ReferentialIntegrityViolationCount => _table.ReferentialIntegrityViolationCount;

        public IEnumerable<VpaColumnViewModel> Columns { get; }
        public IEnumerable<VpaRelationshipViewModel> RelationshipsFrom { get; }
        public long RelationshipMaxFromCardinality { get; }
        public double RelationshipMaxOneToManyRatio { get; }
        public long ColumnMaxTotalSize { get; }
        public long ColumnsMaxCardinality { get; }

        public int CompareTo(object obj)
        {
            var objTable = (VpaTableViewModel)obj;
            switch (_parentViewModel.SortColumn)
            {
                case "ColumnCardinality":
                    return RowsCount.CompareTo(objTable.RowsCount) * _parentViewModel.SortDirection;
                case "TotalSize":
                    return ColumnsTotalSize.CompareTo(objTable.ColumnsTotalSize) * _parentViewModel.SortDirection;
                case "DictionarySize":
                    return ColumnsDictionarySize.CompareTo(objTable.ColumnsDictionarySize) * _parentViewModel.SortDirection;
                case "DataSize":
                    return ColumnsDataSize.CompareTo(objTable.ColumnsDataSize) * _parentViewModel.SortDirection;
                case "HierarchiesSize":
                    return ColumnsHierarchySize.CompareTo(objTable.ColumnsHierarchySize) * _parentViewModel.SortDirection;
                default:
                    return TableName.CompareTo(objTable.TableName) * _parentViewModel.SortDirection;
            }

        }

        public bool IsExpanded { get; set; }
        public long RelationshipMaxToCardinality { get; }

    }

    public class VpaPartitionViewModel : IComparable
    {
        private readonly VpaPartition _partition;
        private readonly VertiPaqAnalyzerViewModel _parentViewModel;
        public VpaPartitionViewModel(VpaPartition partition, VpaTableViewModel table, VertiPaqAnalyzerViewModel parentViewModel)
        {
            _partition = partition;
            _parentViewModel = parentViewModel;
            Table = table;
        }

        public VpaTableViewModel Table { get; }
        public string PartitionName => _partition.PartitionName;

        public long RowsCount => _partition.RowsCount;
        public long DataSize => _partition.DataSize;
        public long PartitionsNumber => 1;
        public long SegmentsNumber => _partition.SegmentsNumber;
        public long SegmentsTotalNumber => _partition.SegmentsTotalNumber;
        public int? SegmentsPageable => _partition.SegmentsPageable;
        public int? SegmentsResident => _partition.SegmentsResident;
        public double? SegmentsAverageTemperature => _partition.SegmentsAverageTemperature * 1000;
        public DateTime? SegmentsLastAccessed => _partition.SegmentsLastAccessed;

        public int CompareTo(object obj)
        {
            var objPartition = (VpaPartitionViewModel)obj;
            switch (_parentViewModel.SortColumn)
            {
                case "ColumnCardinality":
                    return RowsCount.CompareTo(objPartition.RowsCount) * _parentViewModel.SortDirection;
                case "DataSize":
                    return DataSize.CompareTo(objPartition.DataSize) * _parentViewModel.SortDirection;
                default:
                    return PartitionName.CompareTo(objPartition.PartitionName) * _parentViewModel.SortDirection;
            }
        }

        public bool IsExpanded { get; set; }

        public double PercentOfTableRows => (Table == null ? 0 : RowsCount / (double)Table.RowsCount);
        public double PercentOfTableSize => Table == null ? 0 : DataSize / (double)Table.ColumnsDataSize;
    }

    public class VpaSummaryViewModel
    {
        public VpaSummaryViewModel(VertiPaqAnalyzerViewModel parent)
        {
            ParentViewModel = parent;
            TableCount = parent.ViewModel.Tables.Count();
            ColumnCount = parent.ViewModel.Columns.Count();
            CompatibilityLevel = parent.ViewModel.Model.CompatibilityLevel;
            CompatibilityMode = parent.ViewModel.Model.CompatibilityMode;
            TotalSize = parent.ViewModel.Tables.Sum(t => t.TableSize);
            DataSource = parent.ViewModel.Model.ServerName?.Name ?? "<Unknown>";
            ModelName = parent.ViewModel.Model.ModelName.Name;
            LastDataRefresh = parent.ViewModel.Model.LastDataRefresh;
            ExtractionDate = parent.ViewModel.Model.ExtractionDate;
        }

        public VertiPaqAnalyzerViewModel ParentViewModel { get; }
        public int TableCount { get; }
        public int ColumnCount { get; }
        public int CompatibilityLevel { get; }
        public string CompatibilityMode { get; }
        public long TotalSize { get; }
        public string FormattedTotalSize {
            get {
                switch (TotalSize)
                {
                    case long size when size < 1024: return TotalSize.ToString("N0") + " b";
                    case long size when size < (Math.Pow(1024, 2)): return (TotalSize / (double)(1024)).ToString("N2") + " Kb";
                    case long size when size < (Math.Pow(1024, 3)): return (TotalSize / (double)(Math.Pow(1024, 2))).ToString("N2") + " Mb";
                    default: return (TotalSize / (double)(Math.Pow(1024, 3))).ToString("N2") + " Gb";
                }
            }
        }
        public string DataSource { get; }
        public string ModelName { get; }
        public DateTime LastDataRefresh { get; }
        public DateTime ExtractionDate { get; }
    }
}
