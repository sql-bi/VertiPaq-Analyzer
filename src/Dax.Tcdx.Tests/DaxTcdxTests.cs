namespace Dax.Tcdx.Tests
{
    using Dax.Tcdx.Tools;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class TcdxToolsTests
    {
        public const string TcdxConsumerFilePath = @".\_data\SampleConsumer.tcdx";
        public const string TcdxQueryFilePath = @".\_data\SampleQuery.tcdx";

        [Theory]
        [InlineData(TcdxConsumerFilePath)]
        [InlineData(TcdxQueryFilePath)]
        public void ImportTcdx_FromFile_Test(string tcdxPath)
        {
            // fail the test on purpose
            Assert.True(false);

//            var tcdxContent = TcdxTools.ImportTcdx(tcdxPath);

//            Assert.Null(tcdxContent.ViewVpa);
//            Assert.NotNull(tcdxContent.Consumers);
            //Assert.NotNull(tcdxContent.QueryGroups);
        }

        [Theory]
        [InlineData(TcdxConsumerFilePath)]
        [InlineData(TcdxQueryFilePath)]
        public void ImportTcdx_FromStream_Test(string tcdxPath)
        {
            using var stream = File.OpenRead(tcdxPath);
            var tcdxContent = TcdxTools.ImportTcdx(stream);

            //Assert.Null(tcdxContent.ViewVpa);
            //Assert.NotNull(tcdxContent.DaxModel);
            //Assert.NotNull(tcdxContent.TomDatabase);
        }

/*
        [Theory]
        [InlineData(TcdxConsumerFilePath, ContosoBimFilePath, 3)]
        [InlineData(TcdxQueryFilePath, VacciniBimFilePath, 4)]
        public void ImportTcdx_TcdxDaxModelEqualsBimTomModel_Test(string tcdxPath, string bimPath, int tomVersion)
        {
            var daxModel = TcdxTools.ImportTcdx(tcdxPath).DaxModel;
            var tomDatabase = DeserializeDatabase(bimPath, CompatibilityMode.Unknown);

            Assert.Equal(tomVersion, daxModel.Version);
            Assert.Equal(tomDatabase.CompatibilityLevel, daxModel.CompatibilityLevel);
            Assert.Equal(tomDatabase.Model.Tables.Count, daxModel.Tables.Count);
            Assert.Equal(tomDatabase.Model.Relationships.Count, daxModel.Relationships.Count);
            Assert.Equal(tomDatabase.Model.Roles.Count, daxModel.Roles.Count);

            var tomTableNames = tomDatabase.Model.Tables.Cast<TOM.Table>().Select((t) => t.Name).OrderBy((n) => n);
            var daxTableNames = daxModel.Tables.Select((t) => t.TableName.Name).OrderBy((n) => n);

            Assert.True(tomTableNames.SequenceEqual(daxTableNames));

            var tomRelationshipNames = tomDatabase.Model.Relationships.Cast<TOM.Relationship>().Select((r) => r.Name).OrderBy((n) => n);
            var daxRelationshipNames = daxModel.Relationships.Select((r) => r.Name).OrderBy((n) => n);

            Assert.True(tomRelationshipNames.SequenceEqual(daxRelationshipNames));

            var tomRoleNames = tomDatabase.Model.Roles.Cast<TOM.ModelRole>().Select((r) => r.Name).OrderBy((n) => n);
            var daxRoleNames = daxModel.Roles.Select((r) => r.RoleName.Name).OrderBy((n) => n);

            Assert.True(tomRoleNames.SequenceEqual(daxRoleNames));
        }

        [Theory]
        [InlineData(TcdxConsumerFilePath, ContosoBimFilePath)]
        [InlineData(TcdxQueryFilePath, VacciniBimFilePath)]
        public void ImportTcdx_TcdxTomDatabaseEqualsBimTomDatabase_Test(string tcdxPath, string bimPath)
        {
            var mode = CompatibilityMode.Unknown;

            var vpaxTomDatabase = TcdxTools.ImportTcdx(tcdxPath).TomDatabase;
            var bimTomDatabase = DeserializeDatabase(bimPath, mode);

            var expectedString = SerializeToComparableString(bimTomDatabase, mode);
            var actualString = SerializeToComparableString(vpaxTomDatabase, mode);

            Assert.Equal(expectedString, actualString);
        }

        [Theory]
        [InlineData(ContosoBimFilePath)]
        [InlineData(VacciniBimFilePath)]
        public void ExportTcdx_ToFile_Test(string bimPath)
        {
            var tcdxPath = Path.GetTempFileName();
            var tomDatabase = DeserializeDatabase(bimPath, CompatibilityMode.Unknown);
            var daxModel = GetDaxModel(tomDatabase);

            TcdxTools.ExportTcdx(tcdxPath, daxModel, viewVpa: null, tomDatabase);
        }

        [Theory]
        [InlineData(ContosoBimFilePath)]
        [InlineData(VacciniBimFilePath)]
        public void ExportTcdx_ToStream_Test(string bimPath)
        {
            using var vpaxStream = new MemoryStream();
            var tomDatabase = DeserializeDatabase(bimPath, CompatibilityMode.Unknown);
            var daxModel = GetDaxModel(tomDatabase);

            TcdxTools.ExportTcdx(vpaxStream, daxModel, viewVpa: null, tomDatabase);
        }

        private string SerializeToComparableString(TOM.Database database, CompatibilityMode mode)
        {
            database.Name = "DefaultDatabaseName";
            database.ID = Guid.Empty.ToString();

            if (database.Model.Annotations.ContainsName("PBIDesktopVersion"))
                database.Model.Annotations.Remove("PBIDesktopVersion");
            if (database.Model.Annotations.ContainsName("TabularEditor_SerializeOptions"))
                database.Model.Annotations.Remove("TabularEditor_SerializeOptions");
            if (database.Model.Annotations.ContainsName("__TEdtr"))
                database.Model.Annotations.Remove("__TEdtr");

            if (database.CompatibilityMode == CompatibilityMode.Unknown)
                database.CompatibilityMode = mode;

            var serializedDatabase = TOM.JsonSerializer.SerializeDatabase(database, new TOM.SerializeOptions
            {
                IgnoreInferredObjects = true,
                IgnoreInferredProperties = true,
                IgnoreTimestamps = true,
                IncludeRestrictedInformation = true,
                SplitMultilineStrings = true,
            });
            return serializedDatabase;
        }

        private TOM.Database DeserializeDatabase(string path, CompatibilityMode mode)
        {
            var json = File.ReadAllText(path);
            var database = TOM.JsonSerializer.DeserializeDatabase(json, mode: mode);

            if (database.CompatibilityMode == CompatibilityMode.Unknown)
                database.CompatibilityMode = mode;

            return database;
        }

        private Metadata.Model GetDaxModel(TOM.Database database)
        {
            var daxModel = TomExtractor.GetDaxModel(database.Model, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");
            return daxModel;
        }
*/
    }
}