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
using Dax.Metadata;

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
            List<Item> items = new List<Item>();
            Item pivot = CreateMockupPivot();
            Item powerBiVisual = CreateMockupPowerBiVisual();
            items.Add(pivot);
            items.Add(powerBiVisual);
            ConsumersCollection c = BuildMockupConsumersCollection(items);
            QueryGroupsCollection g = BuildMockupQueryGroupsCollection(items);
            SerializeAllTcdx(filePath, c, g);
            TcdxTools.TcdxContent tcdx = TcdxTools.ImportTcdx(filePath);
            ConsumersCollection c2 = tcdx.Consumers;
            QueryGroupsCollection g2 = tcdx.QueryGroups;
            SerializeAllTcdx(filePath1, c2, g2);
        }

        private static ConsumersCollection BuildMockupConsumersCollection(List<Item> items)
        {
            ConsumersCollection consumers = new  ConsumersCollection();
            consumers.ConsumersCollectionProperties.Add("TestProperty1", new TcdxName("TestValue1"));
            consumers.ConsumersCollectionProperties.Add("TestProperty2", new TcdxName("TestValue2"));
            if (items.Count == 0) {
                return consumers;
            }
            Item item0 = items[0];
            Consumer consumer = new Consumer();
            consumer.ConsumerType = EnumConsumerType.Excel;
            consumer.HostName = new TcdxName("MyComputer");
            consumer.Container = new TcdxName("C:\\Path");
            consumer.FileName = new TcdxName("ExcelClient.xlsx");
            consumer.Uri = null; // no uri for Excel client located on my pc
            consumer.UtcAcquisition = new DateTime(2023, 1, 4, 12, 30, 0, DateTimeKind.Utc);
            consumer.UtcModification = new DateTime(2023, 1, 4, 13, 0, 0, DateTimeKind.Utc);
            consumer.ConsumerProperties.Add("Conumer1Property1", new TcdxName("Consumer1Value1"));
            consumer.ConsumerProperties.Add("Conumer1Property2", new TcdxName("Consumer1Value2"));
            consumer.Items.Add(item0);
            consumers.Consumers.Add(consumer);
            if (items.Count < 2) {
                return consumers;
            }
            Item item1 = items[1];
            consumer = new Consumer();
            consumer.ConsumerType = EnumConsumerType.PowerBIDesktop;
            consumer.HostName = new TcdxName("MyComputer");
            consumer.Container = new TcdxName("C:\\BI\\PowerBI");
            consumer.FileName = new TcdxName("MyModel.pbix");
            consumer.Uri = null; // no uri for Excel client located on my pc
            consumer.UtcAcquisition = new DateTime(2023, 1, 12, 13, 20, 0, DateTimeKind.Utc);
            consumer.UtcModification = new DateTime(2023, 1, 12, 14, 0, 0, DateTimeKind.Utc);
            consumer.ConsumerProperties.Add("Conumer2Property1", new TcdxName("Consumer2Value1"));
            consumer.Items.Add(item1);
            consumers.Consumers.Add(consumer);
            return consumers;
        }

        private static QueryGroupsCollection BuildMockupQueryGroupsCollection(List<Item> items)
        {
            QueryGroupsCollection queryGroups = new QueryGroupsCollection();
            queryGroups.QueryGroupsCollectionProperties.Add("QueryGroupCollectionTestProperty1", new TcdxName("QueryGroupCollectionTestValue1"));
            QueryGroup queryGroup1 = new QueryGroup();
            queryGroup1.CorrelationId = "correlationId";
            queryGroup1.QueryGroupType = EnumQueryGroupType.ExtendedEvents;
            queryGroup1.QueryGroupName = new TcdxName("Captured queries on 2023 01 28");
            queryGroup1.QueryGroupProperties.Add("QueryGroup1Property1", new TcdxName("QueryGroup1Value1"));
            queryGroup1.QueryGroupProperties.Add("QueryGroup1Property2", new TcdxName("QueryGroup1Value2"));

            queryGroup1.TableQueries.Add("Customers", 10);
            queryGroup1.TableQueries.Add("Sales", 20);
            queryGroup1.TableQueries.Add("Budget", 2);
            queryGroup1.TableQueries.Add("Products", 23);
            queryGroup1.ColumnQueries.Add("Customers[CountryRegion]", 11);
            queryGroup1.ColumnQueries.Add("Customers[Continent]", 12);
            queryGroup1.MeasureQueries.Add("[Sales Amount]", 13);
            queryGroup1.MeasureQueries.Add("[Budget Amount]", 2);
            queryGroup1.TokenQueries.Add("Unknow if Column or Measure", 1);

            queryGroup1.TotalExecTimeMilliseconds = 12345678;
            queryGroup1.NumberOfQueries = 2000;
            queryGroup1.MaxExcTimeMilliseconds = 10000;
            queryGroup1.MinExcTimeMilliseconds = 10;
            queryGroup1.AverageExcTimeMilliseconds = 100;
            queryGroup1.StandardDeviationExcTimeMilliseconds = 0.1;
            queryGroup1.UtcStart = new DateTime(2023, 1, 4, 12, 30, 0, DateTimeKind.Utc);
            queryGroup1.UtcEnd = new DateTime(2023, 1, 4, 13, 0, 0, DateTimeKind.Utc);
            queryGroups.QueryGroups.Add(queryGroup1);
            if (items.Count == 0) {
                return queryGroups;
            }
            Item item0 = items[0];
            queryGroup1.Item = item0;
            return queryGroups;
        }

        private static Item CreateMockupPivot()
        {
            Item pivot = new Item();
            TableDependency customersTable = new TableDependency();
            customersTable.TableName = new TcdxName("Customers");
            pivot.TableDependencies.Add(customersTable);
            TableDependency salesTable = new TableDependency();
            salesTable.TableName = new TcdxName("Sales");
            pivot.TableDependencies.Add(salesTable);
            ColumnDependency column = new ColumnDependency();
            column.TableName = new TcdxName("Customers");
            column.ColumnName = new TcdxName("CountryRegion");
            pivot.ColumnDependencies.Add(column);
            MeasureDependency measure = new MeasureDependency();
            measure.MeasureName = new TcdxName("Sales Amount");
            measure.Table = salesTable;
            pivot.MeasureDependencies.Add(measure);
            return pivot;
        }

        private static Item CreateMockupPowerBiVisual()
        {
            Item visual = new Item();
            TableDependency customerTable = new TableDependency();
            customerTable.TableName = new TcdxName("Customers");    
            visual.TableDependencies.Add(customerTable);
            TableDependency customersTable = new TableDependency();
            customersTable.TableName = new TcdxName("Products");
            visual.TableDependencies.Add(customersTable);
            TableDependency budgetTable = new TableDependency();
            budgetTable.TableName = new TcdxName("Budget");
            visual.TableDependencies.Add(budgetTable);
            ColumnDependency continentColumn = new ColumnDependency();
            continentColumn.TableName = new TcdxName("Customers");
            continentColumn.ColumnName = new TcdxName("Continent");
            ColumnDependency columnProductBrand = new ColumnDependency();
            columnProductBrand.TableName = new TcdxName("Products");
            columnProductBrand.ColumnName = new TcdxName("Product Brand");
            visual.ColumnDependencies.Add(columnProductBrand);
            MeasureDependency measure = new MeasureDependency();
            measure.MeasureName = new TcdxName("Budget Amount");
            measure.Table = budgetTable;
            visual.MeasureDependencies.Add(measure);
            return visual;
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
