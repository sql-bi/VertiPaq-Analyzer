namespace Dax.Vpax.Tests
{
    using Dax.Metadata.Extractor;
    using Dax.Vpax.Tools;
    using Microsoft.AnalysisServices;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class VpaxToolTests
    {
        public const string ContosoBimFilePath = @".\_data\Contoso.bim";
        public const string ContosoVpaxFilePath = @".\_data\Contoso.vpax";
        //
        public const string VacciniBimFilePath = @".\_data\Vaccini.bim";
        public const string VacciniVpaxFilePath = @".\_data\Vaccini.vpax";

        [Theory]
        [InlineData(ContosoVpaxFilePath)]
        [InlineData(VacciniVpaxFilePath)]
        public void ImportVpax_FromFile_Test(string vpaxPath)
        {
            var vpaxContent = VpaxTools.ImportVpax(vpaxPath);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        [Theory]
        [InlineData(ContosoVpaxFilePath)]
        [InlineData(VacciniVpaxFilePath)]
        public void ImportVpax_FromStream_Test(string vpaxPath)
        {
            using var stream = File.OpenRead(vpaxPath);
            var vpaxContent = VpaxTools.ImportVpax(stream);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        [Theory]
        [InlineData(ContosoVpaxFilePath, ContosoBimFilePath, 3)]
        [InlineData(VacciniVpaxFilePath, VacciniBimFilePath, 4)]
        public void ImportVpax_VpaxDaxModelEqualsBimTomModel_Test(string vpaxPath, string bimPath, int tomVersion)
        {
            var daxModel = VpaxTools.ImportVpax(vpaxPath).DaxModel;
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
        [InlineData(ContosoVpaxFilePath, ContosoBimFilePath)]
        [InlineData(VacciniVpaxFilePath, VacciniBimFilePath)]
        public void ImportVpax_VpaxTomDatabaseEqualsBimTomDatabase_Test(string vpaxPath, string bimPath)
        {
            var mode = CompatibilityMode.Unknown;

            var vpaxTomDatabase = VpaxTools.ImportVpax(vpaxPath).TomDatabase;
            var bimTomDatabase = DeserializeDatabase(bimPath, mode);

            var expectedString = SerializeToComparableString(bimTomDatabase, mode);
            var actualString = SerializeToComparableString(vpaxTomDatabase, mode);

            Assert.Equal(expectedString, actualString);
        }

        [Theory]
        [InlineData(ContosoBimFilePath)]
        [InlineData(VacciniBimFilePath)]
        public void ExportVpax_ToFile_Test(string bimPath)
        {
            var vpaxPath = Path.GetTempFileName();
            var tomDatabase = DeserializeDatabase(bimPath, CompatibilityMode.Unknown);
            var daxModel = GetDaxModel(tomDatabase);

            VpaxTools.ExportVpax(vpaxPath, daxModel, viewVpa: null, tomDatabase);
        }

        [Theory]
        [InlineData(ContosoBimFilePath)]
        [InlineData(VacciniBimFilePath)]
        public void ExportVpax_ToStream_Test(string bimPath)
        {
            using var vpaxStream = new MemoryStream();
            var tomDatabase = DeserializeDatabase(bimPath, CompatibilityMode.Unknown);
            var daxModel = GetDaxModel(tomDatabase);

            VpaxTools.ExportVpax(vpaxStream, daxModel, viewVpa: null, tomDatabase);
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
    }
}