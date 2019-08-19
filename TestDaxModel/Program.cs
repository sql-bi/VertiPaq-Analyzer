using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Newtonsoft.Json;
using System.IO.Packaging;
using System.IO;

// TODO
// - Import from DMV 1100 (check for missing attributes?)
namespace TestDaxModel
{
    class Program
    {
        static void Main(string[] args)
        { 
            //
            // Retrieve DAX model from database connection
            //
            const string serverName = @"localhost\tab17";
            const string databaseName = "AdventureWorks";
            const string pathOutput = @"c:\temp\";

            Console.WriteLine("Getting model {0}:{1}", serverName, databaseName);

            var model = Dax.Model.Extractor.TomExtractor.GetDaxModel(serverName, databaseName, "TestDaxModel", "0.1");

            //
            // Test serialization of Dax.Model in JSON file
            //
            // ExportModelJSON(pathOutput, m);

            Console.WriteLine("Exporting to VertiPaq Analyzer View");

            // 
            // Create VertiPaq Analyzer views
            //
            Dax.ViewVpaExport.Model export = new Dax.ViewVpaExport.Model(model);

            // Save JSON file
            // ExportJSON(pathOutput, export);

            string filename = pathOutput + databaseName + ".vpax";
            Console.WriteLine("Saving {0}...", filename);

            // Save VPAX file
            ExportVPAX(filename, model, export);

            Console.WriteLine("File saved.");
        }

        /// <summary>
        /// Export the Dax.Model in JSON format
        /// </summary>
        /// <param name="pathOutput"></param>
        /// <param name="m"></param>
        private static void ExportModelJSON(string pathOutput, Dax.Model.Model m)
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

        /// <summary>
        /// Dump internal structure for permissions
        /// </summary>
        /// <param name="m"></param>
        private static void DumpPermissions(Dax.Model.Model m)
        {
            Console.WriteLine("------------------------");
            foreach (var t in m.Tables)
            {
                Console.WriteLine("---Table={0}", t.TableName);
                foreach (var tp in t.GetTablePermissions())
                {
                    Console.WriteLine("Role {0} = {1} ", tp.Role.RoleName.Name, tp.FilterExpression.Expression);
                }
            }
        }

        /// <summary>
        /// Dump internal structure for Relationships
        /// </summary>
        /// <param name="m"></param>
        private static void DumpRelationships(Dax.Model.Model m)
        {
            Console.WriteLine("------------------------");
            foreach (var t in m.Tables)
            {
                Console.WriteLine("---Table={0}", t.TableName);
                foreach (var r in t.GetRelationshipsTo())
                {
                    Console.WriteLine(
                        "{0}[{1}] ==> {2}[{3}]",
                        r.FromColumn.Table.TableName,
                        r.FromColumn.ColumnName,
                        r.ToColumn.Table.TableName,
                        r.ToColumn.ColumnName
                    );
                }
                foreach (var r in t.GetRelationshipsFrom())
                {
                    Console.WriteLine(
                        "{0}[{1}] ==> {2}[{3}]",
                        r.FromColumn.Table.TableName,
                        r.FromColumn.ColumnName,
                        r.ToColumn.Table.TableName,
                        r.ToColumn.ColumnName
                    );
                }
            }
        }

        /// <summary>
        /// Dump internal model structure 
        /// </summary>
        static void SimpleDump()
        {
            Dax.Model.Model m = new Dax.Model.Model();
            Dax.Model.Table tA = new Dax.Model.Table(m)
            {
                TableName = new Dax.Model.DaxName("A")
            };
            Dax.Model.Table tB = new Dax.Model.Table(m)
            {
                TableName = new Dax.Model.DaxName("B")
            };
            Dax.Model.Column ca1 = new Dax.Model.Column(tA)
            {
                ColumnName = new Dax.Model.DaxName("A_1")
            };
            Dax.Model.Column ca2 = new Dax.Model.Column(tA)
            {
                ColumnName = new Dax.Model.DaxName("A_2")
            };
            tA.Columns.Add(ca1);
            tA.Columns.Add(ca2);
            m.Tables.Add(tA);

            // Test serialization on JSON file
            var json = JsonConvert.SerializeObject(m, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
            System.IO.File.WriteAllText(@"C:\temp\model.json", json);
        }
    }


}
