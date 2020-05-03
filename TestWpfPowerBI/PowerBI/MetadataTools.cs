using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;

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
        public IList<Dataset> GetDatasets(Group group)
        {
            return Client.Datasets.GetDatasetsInGroup(group.Id).Value;
        }
        public IList<Dataset> GetDatasets()
        {
            return Client.Datasets.GetDatasets().Value;
        }
    }
}
