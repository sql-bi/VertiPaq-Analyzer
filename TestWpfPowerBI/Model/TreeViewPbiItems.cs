using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.PowerBI.Api.Models;

namespace TestWpfPowerBI.Model
{
    public delegate IEnumerable<TreeViewPbiItem> GetChildrenDelegate(IEventAggregator eventAggregator);

    public abstract class TreeViewPbiItem : PropertyChangedBase
    {
        protected GetChildrenDelegate _getChildren;
        protected IEventAggregator _eventAggregator;

        public TreeViewPbiItem(GetChildrenDelegate getChildren, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _getChildren = getChildren;
        }

        public abstract string Name { get; }

        // private IEnumerable<TreeViewPbiItem> _children;
        public IEnumerable<TreeViewPbiItem> Children {
            get {
                IEnumerable<TreeViewPbiItem> _children = null;
                if (_children == null && _getChildren != null)
                { _children = _getChildren.Invoke(_eventAggregator); }
                return _children;
            }
        }
    }

    public class TreeViewPbiGroup : TreeViewPbiItem
    {
        public TreeViewPbiGroup(Group group, GetChildrenDelegate getChildren, IEventAggregator eventAggregator)
            :base(getChildren, eventAggregator)
        {
            this.Group = group;
        }

        public override string Name { get { return Group.Name; } }

        public Group Group { get; set; }

        // public IList<TreeViewPbiItem> Datasets { get { GetChildrenDelegate()}
    }

    public class TreeViewPbiDataset : TreeViewPbiItem
    {
        public TreeViewPbiDataset(Dataset dataset, GetChildrenDelegate getChildren, IEventAggregator eventAggregator)
            : base(getChildren, eventAggregator)
        {
            this.Dataset = dataset;
        }

        public override string Name { get { return Dataset.Name; } }

        public Dataset Dataset { get; set; }
    }

    public class TreeViewPbiDummy : TreeViewPbiItem
    {
        public TreeViewPbiDummy(string name, GetChildrenDelegate getChildren, IEventAggregator eventAggregator)
            : base(getChildren, eventAggregator)
        {
            this.Name = name;
        }

        public override string Name { get; }

        public Dataset Dataset { get; set; }
    }
}
