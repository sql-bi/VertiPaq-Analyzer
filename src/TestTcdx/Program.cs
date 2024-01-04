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

        static void Main()
        {
            // for examples of tests see the project TestDaxModel
            ConsumersCollection c = BuildMockupConsumersCollection();
            SerializeConsumersCollection(c);
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

        private static void SerializeConsumersCollection(ConsumersCollection consumers)
        {
            string path = @"C:\temp\consumerscollection.tcdx";
            using (var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite)) {
                TcdxTools.ExportTcdx(stream, consumers);
            }

        }

    }


}
