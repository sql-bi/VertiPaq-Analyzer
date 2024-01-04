using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO.Packaging;
using System.IO;
using Dax.Tcdx.Tools;
using Dax.Tcdx.Metadata;

// TODO
// - Import from DMV 1100 (check for missing attributes?)
#pragma warning disable IDE0051 // Remove unused private members
namespace TestTcdx
{
    class Program
    {
        const string filePath = @"C:\temp\consumerscollectionandquerygroups.tcdx";
        const string filePath1 = @"C:\temp\consumerscollectionandquerygroups1.tcdx";

        static void Main()
        {
            // for examples of tests see the project TestDaxModel
            ConsumersCollection c = BuildMockupConsumersCollection();
            QueryGroupsCollection g = BuildMockupQueryGroupsCollection();
            SerializeAllTcdx(filePath, c, g);
            TcdxTools.TcdxContent tcdx = TcdxTools.ImportTcdx(filePath);
            ConsumersCollection c2 = tcdx.Consumers;
            QueryGroupsCollection g2 = tcdx.QueryGroups;
            SerializeAllTcdx(filePath1, c2, g2);
        }

        private static ConsumersCollection BuildMockupConsumersCollection()
        {
            ConsumersCollection consumers = new  ConsumersCollection();
            Consumer consumer = new Consumer();
            consumer.ConsumerType = EnumConsumerType.Excel;
            consumer.HostName = new TcdxName("MyComputer");
            consumer.Container = new TcdxName("C:\\Path");
            consumer.FileName = new TcdxName("ExcelClient.xlsx");
            consumer.Uri = null; // no uri for Excel client located on my pc
            consumer.UtcAcquisition = new DateTime(2023, 1, 4, 12, 30, 0, DateTimeKind.Utc);
            consumer.UtcModification = new DateTime(2023, 1, 4, 13, 0, 0, DateTimeKind.Utc);
            consumers.Consumers.Add(consumer);
            return consumers;
        }

        private static QueryGroupsCollection BuildMockupQueryGroupsCollection()
        {
            QueryGroupsCollection queryGroups = new QueryGroupsCollection();
            QueryGroup queryGroup = new QueryGroup();
            queryGroup.CorrelationId = "correlationId";
            queryGroup.QueryGroupType = EnumQueryGroupType.ExtendedEvents;
            // TODO add missing members
            queryGroup.UtcStart = new DateTime(2023, 1, 4, 12, 30, 0, DateTimeKind.Utc);
            queryGroup.UtcEnd = new DateTime(2023, 1, 4, 13, 0, 0, DateTimeKind.Utc);
            queryGroups.QueryGroups.Add(queryGroup);
            return queryGroups;
        }

        private static void SerializeAllTcdx(string filePath, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite)) 
            {
                TcdxTools.ExportTcdx(stream, consumers, queryGroups);
            }
        }
    }
}
