namespace Dax.Vpax.Tests
{
    using Dax.Vpax.Tools;
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class VpaxToolTests
    {
        private const string ContosoVpaxPath = @".\_data\vpax\Contoso.vpax";
        private const string VacciniVpaxPath = @".\_data\vpax\Vaccini.vpax";
        private const string ContosoBimPath = @".\_data\bim\Contoso.bim";
        private const string VacciniBimPath = @".\_data\bim\Vaccini.bim";

        private readonly TOM.SerializeOptions ComparableObjectSerializationOptions = new TOM.SerializeOptions
        {
            IgnoreInferredObjects = true,
            IgnoreInferredProperties = true,
            IgnoreTimestamps = true,
            IncludeRestrictedInformation = true,
            SplitMultilineStrings = true,
        };

        [Theory]
        [InlineData(ContosoVpaxPath)]
        [InlineData(VacciniVpaxPath)]
        public void ImportVpax_FileTest(string vpaxPath)
        {
            var vpaxContent = VpaxTools.ImportVpax(vpaxPath);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        [Theory]
        [InlineData(ContosoVpaxPath, ContosoBimPath)]
        [InlineData(VacciniVpaxPath, VacciniBimPath)]
        public void ImportVpax_DaxModelNotChangedTest(string vpaxPath, string bimPath)
        {
            var vpaxContent = VpaxTools.ImportVpax(vpaxPath);
            var daxModel = vpaxContent.DaxModel;
            var tomDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(bimPath));

            // TODO: implement DaxModel comparer

            Assert.True(daxModel.Version > 0);
            Assert.Equal(tomDatabase.CompatibilityLevel, daxModel.CompatibilityLevel);
            Assert.Equal(tomDatabase.Model.Tables.Count, daxModel.Tables.Count);
            Assert.Equal(tomDatabase.Model.Relationships.Count, daxModel.Relationships.Count);
            Assert.Equal(tomDatabase.Model.Roles.Count, daxModel.Roles.Count);

            var tomTableNames = tomDatabase.Model.Tables.Cast<TOM.Table>().Select((t) => t.Name).OrderBy((n) => n);
            var daxTableNames = daxModel.Tables.Select((t) => t.TableName.Name).OrderBy((n) => n);

            Assert.True(tomTableNames.SequenceEqual(daxTableNames));

            //foreach (var daxTable in daxModel.Tables)
            //{
            //    var tomTable = tomDatabase.Model.Tables[daxTable.TableName.Name];

            //    Assert.Equal(tomTable.Description, daxTable.Description.Note);

            //    var tomColumnNames = tomTable.Columns.Cast<TOM.Column>().Select((c) => c.Name).OrderBy((n) => n);
            //    var daxColumnNames = daxTable.Columns.Select((c) => c.ColumnName.Name).OrderBy((n) => n);

            //    Assert.True(tomColumnNames.SequenceEqual(daxColumnNames), daxTable.TableName.Name);

            //    foreach (var daxColumn in daxTable.Columns)
            //    {
            //        var tomColumn = tomTable.Columns[daxColumn.ColumnName.Name];

            //        Assert.Equal(tomColumn.DisplayFolder, daxColumn.DisplayFolder.Note);
            //        Assert.Equal(tomColumn.Description, daxColumn.Description.Note);
            //        Assert.Equal(tomColumn.SortByColumn?.Name, daxColumn.SortByColumnName.Name);

            //        if (tomColumn is TOM.CalculatedColumn tomCalculatedColumn)
            //        {
            //            Assert.Equal(tomCalculatedColumn.Expression, daxColumn.ColumnExpression.Expression);
            //        }

            //        // TODO: ...
            //    }
            //}

            // TODO: ...
        }

        [Theory]
        [InlineData(ContosoVpaxPath, ContosoBimPath)]
        [InlineData(VacciniVpaxPath, VacciniBimPath)]
        public void ImportVpax_TomDatabaseNotChangedTest(string vpaxPath, string bimPath)
        {
            var vpaxContent = VpaxTools.ImportVpax(vpaxPath);
            var vpaxTomDatabase = vpaxContent.TomDatabase;
            var bimTomDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(bimPath));

            var expectedString = SerializeToComparableString(bimTomDatabase);
            var actualString = SerializeToComparableString(vpaxTomDatabase);

            Assert.Equal(expectedString, actualString);
        }

        [Theory]
        [InlineData(ContosoVpaxPath)]
        [InlineData(VacciniVpaxPath)]
        public void ImportVpax_StreamTest(string vpaxPath)
        {
            using var stream = File.OpenRead(vpaxPath);
            var vpaxContent = VpaxTools.ImportVpax(stream);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        [Theory]
        [InlineData(ContosoBimPath)]
        [InlineData(VacciniBimPath)]
        public void ExportVpax_FileTest(string bimPath)
        {
            var vpaxPath = Path.GetTempFileName();
            var vpaxModel = new Metadata.Model();
            var tomDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(bimPath));

            VpaxTools.ExportVpax(vpaxPath, vpaxModel, viewVpa: null, tomDatabase);
            var vpaxContent = VpaxTools.ImportVpax(vpaxPath);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        [Theory]
        [InlineData(ContosoBimPath)]
        [InlineData(VacciniBimPath)]
        public void ExportVpax_StreamTest(string bimPath)
        {
            using var vpaxStream = new MemoryStream();
            var vpaxModel = new Metadata.Model();
            var tomDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(bimPath));

            VpaxTools.ExportVpax(vpaxStream, vpaxModel, viewVpa: null, tomDatabase);
            var vpaxContent = VpaxTools.ImportVpax(vpaxStream);

            Assert.Null(vpaxContent.ViewVpa);
            Assert.NotNull(vpaxContent.DaxModel);
            Assert.NotNull(vpaxContent.TomDatabase);
        }

        private string SerializeToComparableString(TOM.Database database)
        {
            database.Name = "DefaultDatabaseName";
            database.ID = Guid.Empty.ToString();

            if (database.Model.Annotations.ContainsName("PBIDesktopVersion"))
                database.Model.Annotations.Remove("PBIDesktopVersion");
            if (database.Model.Annotations.ContainsName("TabularEditor_SerializeOptions"))
                database.Model.Annotations.Remove("TabularEditor_SerializeOptions");
            if (database.Model.Annotations.ContainsName("__TEdtr"))
                database.Model.Annotations.Remove("__TEdtr");

            var serializedDatabase = TOM.JsonSerializer.SerializeDatabase(database, ComparableObjectSerializationOptions);
            return serializedDatabase;
        }
    }
}