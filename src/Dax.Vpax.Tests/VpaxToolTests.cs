namespace Dax.Vpax.Tests
{
    using Dax.Vpax.Tools;
    using System.IO;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class VpaxToolTests
    {
        private const string ContosoVpaxPath = @".\_data\vpax\Contoso.vpax";
        private const string ContosoBimPath = @".\_data\bim\Contoso.bim";

        [Fact]
        public void ImportVpax_FileTest()
        {
            var content = VpaxTools.ImportVpax(ContosoVpaxPath);

            Assert.Null(content.ViewVpa);
            Assert.NotNull(content.DaxModel);
            Assert.NotNull(content.TomDatabase);
        }

        [Fact]
        public void ImportVpax_StreamTest()
        {
            using var stream = File.OpenRead(ContosoVpaxPath);
            var content = VpaxTools.ImportVpax(stream);

            Assert.Null(content.ViewVpa);
            Assert.NotNull(content.DaxModel);
            Assert.NotNull(content.TomDatabase);
        }

        [Fact]
        public void ExportVpax_FileTest()
        {
            var vpaxPath = Path.GetTempFileName();
            var vpaxModel = new Metadata.Model
            {
                ServerName = new Metadata.DaxName("server"),
                ModelName = new Metadata.DaxName("model")
            };
            var expectedDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(ContosoBimPath));

            VpaxTools.ExportVpax(vpaxPath, vpaxModel, viewVpa: null, expectedDatabase);

            var content = VpaxTools.ImportVpax(vpaxPath);
            var actualDatabase = content.TomDatabase;

            Assert.Equal(content.TomDatabase, actualDatabase);
        }

        [Fact]
        public void ExportVpax_StreamTest()
        {
            using var vpaxStream = new MemoryStream();
            var vpaxModel = new Metadata.Model
            {
                ServerName = new Metadata.DaxName("server"),
                ModelName = new Metadata.DaxName("model")
            };
            var expectedDatabase = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(ContosoBimPath));

            VpaxTools.ExportVpax(vpaxStream, vpaxModel, viewVpa: null, expectedDatabase);

            var content = VpaxTools.ImportVpax(vpaxStream);
            var actualDatabase = content.TomDatabase;

            Assert.Equal(content.TomDatabase, actualDatabase);
        }
    }
}