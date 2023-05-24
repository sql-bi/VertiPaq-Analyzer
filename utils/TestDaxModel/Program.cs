using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Newtonsoft.Json;
using System.IO.Packaging;
using System.IO;
using Dax.Vpax.Tools;
using TOM = Microsoft.AnalysisServices.Tabular;

// TODO
// - Import from DMV 1100 (check for missing attributes?)
#pragma warning disable IDE0051 // Remove unused private members
namespace TestDaxModel
{
    class Program
    {

        static void Main()
        {
            GenericTest();
            //ConnectionStringTest();
            //TestPbiShared_2022();
            //TestPbiShared();
            //TestLocalVpaModel();
            //TestExport();
            //TestExportStream();
        }

        static void TestExportStream()
        {
            string databaseName = "Microsoft_SQLServer_AnalysisServices";
            const string serverName = @"http://localhost:9000/xmla";
            const string applicationName = "Test";
            const string applicationVersion = "0.0";
            bool includeTomModel = false;

            const string path = @"c:\temp\test5.vpax";

            Console.WriteLine("Exporting...");

            //
            // Get Dax.Model object from the SSAS engine
            //
            Dax.Metadata.Model model = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(serverName, databaseName, applicationName, applicationVersion);

            //
            // Get TOM model from the SSAS engine
            //
            TOM.Database database = includeTomModel ? Dax.Metadata.Extractor.TomExtractor.GetDatabase(serverName, databaseName) : null;

            // 
            // Create VertiPaq Analyzer views
            //
            Dax.ViewVpaExport.Model viewVpa = new Dax.ViewVpaExport.Model(model);

            //
            // Save VPAX file
            // 

            using (var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
            {
                Dax.Vpax.Tools.VpaxTools.ExportVpax(stream, model, viewVpa, database);
            }

            using (var fileStream = File.OpenWrite(path))
            using (var memoryStream = new MemoryStream())
            {
                Dax.Vpax.Tools.VpaxTools.ExportVpax(memoryStream, model, viewVpa, database);
                memoryStream.CopyTo(fileStream);
            }

            Console.WriteLine("Completed");
        }

        static void TestExport()
        {
            string databaseName = "Microsoft_SQLServer_AnalysisServices";
            const string serverName = @"http://localhost:9000/xmla";
            const string applicationName = "Test";
            const string applicationVersion = "0.0";
            bool includeTomModel = false;

            const string path = @"c:\temp\test4.vpax";

            Console.WriteLine("Exporting...");

            //
            // Get Dax.Model object from the SSAS engine
            //
            Dax.Metadata.Model model = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(serverName, databaseName, applicationName, applicationVersion);

            //
            // Get TOM model from the SSAS engine
            //
            TOM.Database database = includeTomModel ? Dax.Metadata.Extractor.TomExtractor.GetDatabase(serverName, databaseName) : null;

            // 
            // Create VertiPaq Analyzer views
            //
            Dax.ViewVpaExport.Model viewVpa = new Dax.ViewVpaExport.Model(model);

            //
            // Save VPAX file
            // 
            // TODO: export of database should be optional
            Dax.Vpax.Tools.VpaxTools.ExportVpax(path, model, viewVpa, database);

            Console.WriteLine("Completed");
        }
        static void TestLocalVpaModel()
        {
            //string databaseName = "Contoso";
            //const string serverName = @".\tab17";
            // string databaseName = "CalculationGroups_Currency";
            // const string serverName = @".\tab19";
            string databaseName = "3ac62bef-b3ba-491b-886e-ad11eaccfede";
            const string serverName = @"localhost:53899";

            var connStr = $"Provider=MSOLAP;Data Source={serverName};Initial Catalog={databaseName};";
            var conn = new System.Data.OleDb.OleDbConnection(connStr);

            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(m, conn, serverName, databaseName, "Test", "0.1");
            Dax.Metadata.Extractor.StatExtractor.UpdateStatisticsModel(m, conn, 10);
            DumpRelationships(m);
        }
        static void TestPbiShared()
        {
            const string dataSource = "pbiazure://api.powerbi.com";
            const string identityProvider = "https://login.microsoftonline.com/common, https://analysis.windows.net/powerbi/api, 929d0ec0-7a41-4b1e-bc7c-b754a28bddcc;";
            const string initialCatalog = "cffc2ad9-6ba9-4597-adec-b78af24e8fee";
            const string databaseName = "sobe_wowvirtualserver-" + initialCatalog;
            //const string integratedSecurity = "ClaimsToken";
            //const string other = "MDX Compatibility= 1; MDX Missing Member Mode= Error; Safety Options= 2; Update Isolation Level= 2; Locale Identifier= 1033;";

            const string serverName = dataSource;

            var connStr = String.Format(
                "Provider=MSOLAP;Identity Provider={0};Data Source={1};Initial Catalog={2};", // ;MDX Compatibility= 1; MDX Missing Member Mode= Error; Safety Options= 2; Update Isolation Level= 2;";
                identityProvider,
                dataSource,
                initialCatalog
                );

            var conn = new System.Data.OleDb.OleDbConnection(connStr);
            //conn.Open();
            //Console.WriteLine("Connection open");

            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(m, conn, serverName, databaseName, "Test", "0.1");
        }
        static void TestPbiShared_2022()
        {
            const string dataSource = "powerbi://api.powerbi.com/v1.0/myorg/Azure";
            const string databaseName = "BookSales Azure";

            const string serverName = dataSource;

            var connStr = $"Data Source={dataSource};Catalog={databaseName};";
            var conn = new Microsoft.AnalysisServices.AdomdClient.AdomdConnection(connStr);
            conn.Open();

            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(m, conn, serverName, databaseName, "Test", "0.1");
        }
        static void GenericTest()
        {
            //
            // Retrieve DAX model from database connection
            //
            // String connection for Power Pivot
            // const string serverName = @"http://localhost:9000/xmla";
            // const string databaseName = "Microsoft_SQLServer_AnalysisServices";

            const string serverName = @"localhost\tab19";
            const string databaseName = "Adventure Works";
            // const string databaseName = "Adventure Works 2012 Tabular";
            // const string databaseName = "EnterpriseBI";
            //const string serverName = "localhost:53406";
            //const string databaseName = "84d819d1-e1b3-4c8a-b9f6-c34ac2d2aba2";

            //const string serverName = @"localhost\ctp22";
            //const string databaseName = "Contoso Base";

            const string pathOutput = @"c:\temp\";

            Console.WriteLine("Getting model {0}:{1}", serverName, databaseName);
            var database = Dax.Metadata.Extractor.TomExtractor.GetDatabase(serverName, databaseName);
            var daxModel = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(serverName, databaseName, "TestDaxModel", "0.2", true, 10, analyzeDirectQuery:true);
            Console.WriteLine(database.CompatibilityMode);
            //DumpReferencedColumns(daxModel);
            //DumpReferencedMeasures(daxModel);
            // DumpRelationships(daxModel);

            //
            // Test serialization of Dax.Model in JSON file
            //
            // ExportModelJSON(pathOutput, m);

            Console.WriteLine("Exporting to VertiPaq Analyzer View");

            // 
            // Create VertiPaq Analyzer views
            //
            Dax.ViewVpaExport.Model viewVpa = new Dax.ViewVpaExport.Model(daxModel);
            
            // Save JSON file
            // ExportJSON(pathOutput, export);
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Column Count: {viewVpa.Columns.Count()}");
            Console.WriteLine($"   Relationships Count: {viewVpa.Relationships.Count()}");
            string filename = pathOutput + databaseName + ".vpax";
            Console.WriteLine("Saving {0}...", filename);

            // Save VPAX file
            // old internal version ExportVPAX(filename, daxModel, export);
            VpaxTools.ExportVpax(filename, daxModel, viewVpa, database);
            Console.WriteLine("File saved.");
            // ImportExport();

            Console.WriteLine("=================");
            Console.WriteLine($"Loading {filename}...");
            
            var content = VpaxTools.ImportVpax(filename);
            // var view2 = new Dax.ViewVpaExport.Model(content.DaxModel);
            viewVpa = new Dax.ViewVpaExport.Model(content.DaxModel);
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Column Count: {viewVpa.Columns.Count()}");
            Console.WriteLine($"   Relationships Count: {viewVpa.Relationships.Count()}");
            
            var vm = new Dax.ViewModel.VpaModel(daxModel);
            foreach (var r in vm.Relationships) {
                Console.WriteLine($"From: {r.FromColumnName}  To: {r.ToColumnName} ({r.RelationshipFromToName})");
            }
        }

        static void ConnectionStringTest()
        {
            //
            // Retrieve DAX model from database connection
            //
            // String connection for Power Pivot
            // const string serverName = @"http://localhost:9000/xmla";
            // const string databaseName = "Microsoft_SQLServer_AnalysisServices";

            const string connectionString = "data source=powerbi://api.powerbi.com/v1.0/myorg/dgosbell%20aas%20migration;initial catalog=Adventure Works";
            

            //const string serverName = @"localhost\ctp22";
            //const string databaseName = "Contoso Base";

            const string pathOutput = @"c:\temp\";

            Console.WriteLine("Getting model for connectionString {0}", connectionString);
            var database = Dax.Metadata.Extractor.TomExtractor.GetDatabase(connectionString);
            var daxModel = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(connectionString, "TestDaxModel", "0.2", true, 10, analyzeDirectQuery: true);
            Console.WriteLine(database.CompatibilityMode);
            //DumpReferencedColumns(daxModel);
            //DumpReferencedMeasures(daxModel);
            // DumpRelationships(daxModel);

            //
            // Test serialization of Dax.Model in JSON file
            //
            // ExportModelJSON(pathOutput, m);

            Console.WriteLine("Exporting to VertiPaq Analyzer View");

            // 
            // Create VertiPaq Analyzer views
            //
            Dax.ViewVpaExport.Model viewVpa = new Dax.ViewVpaExport.Model(daxModel);

            // Save JSON file
            // ExportJSON(pathOutput, export);
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Column Count: {viewVpa.Columns.Count()}");
            Console.WriteLine($"   Relationships Count: {viewVpa.Relationships.Count()}");
            string filename = pathOutput + database.Name + ".vpax";
            Console.WriteLine("Saving {0}...", filename);

            // Save VPAX file
            // old internal version ExportVPAX(filename, daxModel, export);
            VpaxTools.ExportVpax(filename, daxModel, viewVpa, database);
            Console.WriteLine("File saved.");
            // ImportExport();

            Console.WriteLine("=================");
            Console.WriteLine($"Loading {filename}...");

            var content = VpaxTools.ImportVpax(filename);
            // var view2 = new Dax.ViewVpaExport.Model(content.DaxModel);
            viewVpa = new Dax.ViewVpaExport.Model(content.DaxModel);
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Table Count : {viewVpa.Tables.Count()}");
            Console.WriteLine($"   Column Count: {viewVpa.Columns.Count()}");
            Console.WriteLine($"   Relationships Count: {viewVpa.Relationships.Count()}");

        }

        private static void ImportExport()
        {
            string filename = @"c:\temp\AdventureWorks.vpax";
            string fileout = @"c:\temp\export.vpax";
            var content = VpaxTools.ImportVpax(filename);
            VpaxTools.ExportVpax(fileout, content.DaxModel, content.ViewVpa, content.TomDatabase);
        }
        /// <summary>
        /// Export the Dax.Model in JSON format
        /// </summary>
        /// <param name="pathOutput"></param>
        /// <param name="m"></param>
        private static void ExportModelJSON(string pathOutput, Dax.Metadata.Model m)
        {
            var json = JsonConvert.SerializeObject(
                m,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
            System.IO.File.WriteAllText(pathOutput + "model.json", json);
            // Console.WriteLine(json);
        }

        /// <summary>
        /// Export VertiPaq Analyzer JSON format (just for test, the same file is embedded in VPAX)
        /// </summary>
        /// <param name="pathOutput"></param>
        /// <param name="export"></param>
        private static void ExportJSON(string pathOutput, Dax.ViewVpaExport.Model export)
        {
            var json = JsonConvert.SerializeObject(
                export,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Error
                });
            System.IO.File.WriteAllText(pathOutput + "export.json", json);
            // Console.WriteLine(json);
        }
        /*
        /// <summary>
        /// Export to VertiPaq Analyzer (VPAX) file
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="pathOutput"></param>
        /// <param name="export"></param>
        private static void ExportVPAX(string path, Dax.Model.Model model, Dax.ViewVpaExport.Model export)
        {
            Uri uriModel = PackUriHelper.CreatePartUri(new Uri("DaxModel.json", UriKind.Relative));
            Uri uriModelVpa = PackUriHelper.CreatePartUri(new Uri("DaxModelVpa.json", UriKind.Relative));
            using (Package package = Package.Open(path, FileMode.Create))
            {
                using (TextWriter tw = new StreamWriter(package.CreatePart(uriModel, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8))
                {
                    tw.Write(
                        JsonConvert.SerializeObject(
                            model,
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.All,
                                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                            }
                        )
                    );
                    tw.Close();
                }
                using (TextWriter tw = new StreamWriter(package.CreatePart(uriModelVpa, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8))
                {
                    tw.Write(JsonConvert.SerializeObject(export, Formatting.Indented));
                    tw.Close();
                }
                package.Close();
            }
        }
        */
        /// <summary>
        /// Dump internal structure for permissions
        /// </summary>
        /// <param name="m"></param>
        private static void DumpPermissions(Dax.Metadata.Model m)
        {
            Console.WriteLine("------------------------");
            foreach (var t in m.Tables)
            {
                Console.WriteLine("---Table={0}", t.TableName);
                foreach (var tp in t.GetTablePermissions())
                {
                    Console.WriteLine("Role {0} = {1} ", tp.Role.RoleName.Name, tp.FilterExpression?.Expression);
                }
            }
        }

        private static void DumpReferencedColumns(Dax.Metadata.Model model)
        {
            Console.WriteLine("------------------------");
            foreach (var t in model.Tables)
            {
                Console.WriteLine($"---Table={t.TableName} [referenced={t.IsReferenced}]");
                foreach (var c in t.Columns.Where(c=>c.IsReferenced))
                {
                    Console.WriteLine(c.ColumnName);
                }
            }
        }

        private static void DumpReferencedMeasures(Dax.Metadata.Model model)
        {
            Console.WriteLine("------------------------");
            foreach (var t in model.Tables)
            {
                Console.WriteLine($"---Table={t.TableName} [referenced={t.IsReferenced}]");
                foreach (var m in t.Measures.Where(m => m.IsReferenced))
                {
                    Console.WriteLine(m.MeasureName);
                }
            }
        }
        /// <summary>
        /// Dump internal structure for Relationships
        /// </summary>
        /// <param name="m"></param>
        private static void DumpRelationships(Dax.Metadata.Model m)
        {
            Console.WriteLine("------------------------");
            foreach (var t in m.Tables)
            {
                Console.WriteLine("---Table={0}", t.TableName);
                foreach (var r in t.GetRelationshipsTo())
                {
                    Console.WriteLine(
                        "{0}[{1}] ==> {2}[{3}] MissingKeys={4}, InvalidRows={5}, Violations={6}",
                        r.FromColumn.Table.TableName,
                        r.FromColumn.ColumnName,
                        r.ToColumn.Table.TableName,
                        r.ToColumn.ColumnName,
                        r.MissingKeys,
                        r.InvalidRows,
                        string.Join(",",r.SampleReferentialIntegrityViolations)
                    );
                }
                foreach (var r in t.GetRelationshipsFrom())
                {
                    Console.WriteLine(
                        "{0}[{1}] ==> {2}[{3}] MissingKeys={4}, InvalidRows={5}, Violations={6}",
                        r.FromColumn.Table.TableName,
                        r.FromColumn.ColumnName,
                        r.ToColumn.Table.TableName,
                        r.ToColumn.ColumnName,
                        r.MissingKeys,
                        r.InvalidRows,
                        string.Join(",", r.SampleReferentialIntegrityViolations)
                    );
                }
            }
        }

        /// <summary>
        /// Dump internal model structure 
        /// </summary>
        static void SimpleDump()
        {
            Dax.Metadata.Model m = new Dax.Metadata.Model();
            Dax.Metadata.Table tA = new Dax.Metadata.Table(m)
            {
                TableName = new Dax.Metadata.DaxName("A")
            };
            Dax.Metadata.Table tB = new Dax.Metadata.Table(m)
            {
                TableName = new Dax.Metadata.DaxName("B")
            };
            Dax.Metadata.Column ca1 = new Dax.Metadata.Column(tA)
            {
                ColumnName = new Dax.Metadata.DaxName("A_1")
            };
            Dax.Metadata.Column ca2 = new Dax.Metadata.Column(tA)
            {
                ColumnName = new Dax.Metadata.DaxName("A_2")
            };
            tA.Columns.Add(ca1);
            tA.Columns.Add(ca2);
            m.Tables.Add(tA);
            m.Tables.Add(tB);

            // Test serialization on JSON file
            var json = JsonConvert.SerializeObject(m, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            System.IO.File.WriteAllText(@"C:\temp\model.json", json);
        }
    }


}
