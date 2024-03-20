using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Caliburn.Micro;
using TestWpfPowerBI.Model;
using System.Configuration;

namespace TestWpfPowerBI.PowerBI
{
    class MetadataTools
    {
        private static string ApiUrl = "https://api.powerbi.com";
        public PowerBIClient Client { get; private set; }

        public MetadataTools( string token )
        {
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            Client = new PowerBIClient(new Uri(ApiUrl), tokenCredentials);
        }
        public IList<Group> GetGroups()
        {
            return Client.Groups.GetGroups().Value;
        }
        public IList<Dataset> GetDatasets(Guid groupId)
        {
            return Client.Datasets.GetDatasetsInGroup(groupId).Value;
        }
        public IList<Dataset> GetDatasets()
        {
            return Client.Datasets.GetDatasets().Value;
        }

        internal class DynamicDatasets
        {
            private MetadataTools _parent;
            public DynamicDatasets(MetadataTools parent)
            {
                _parent = parent;
            }

            public IEnumerable<TreeViewPbiItem> GetChildren(TreeViewPbiItem item, IEventAggregator eventAggregator)
            {
                

                //if (item.Children != null)
                //{
                    return from d in _parent.GetDatasets(item.Id) select new TreeViewPbiDataset(d,null,null)
                           ;
                //}
                //else
                //{
                //    var pbiDummyDatasets = new List<TreeViewPbiItem>();
                //    pbiDummyDatasets.Add(new TreeViewPbiDummy("Loading...", null, null));
                //    return pbiDummyDatasets;
                //}
            }
        }
        

        public IList<TreeViewPbiGroup> GetPbiGroups(IEventAggregator eventAggregator)
        {
            var pbiGroups =
                from g in Client.Groups.GetGroups().Value
                select new TreeViewPbiGroup(g, (new DynamicDatasets(this)).GetChildren, eventAggregator);
            return pbiGroups.ToList();
        }

        public IList<TreeViewPbiDataset> GetPbiDatasets(Group _group, IEventAggregator eventAggregator)
        {
            var pbiDatasets =
                from d in Client.Datasets.GetDatasetsInGroup(_group.Id).Value
                select new TreeViewPbiDataset(d, null, eventAggregator);
            return pbiDatasets.ToList();
        }
    }
}
