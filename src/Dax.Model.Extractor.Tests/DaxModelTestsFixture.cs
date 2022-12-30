namespace Dax.Model.Extractor.Tests
{
    using Dax.Metadata.Extractor;
    using Microsoft.AnalysisServices;
    using System.IO;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class DaxModelTestsFixture
    {
        private const string ContosoBimFilePath = @".\_data\Contoso.bim";

        public DaxModelTestsFixture()
        {
            Contoso = GetDaxModel(ContosoBimFilePath);
        }

        private Metadata.Model GetDaxModel(string path, CompatibilityMode mode = CompatibilityMode.Unknown)
        {
            var json = File.ReadAllText(path);
            var tomDatabase = TOM.JsonSerializer.DeserializeDatabase(json, mode: mode);
            var daxModel = TomExtractor.GetDaxModel(tomDatabase.Model, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            return daxModel;
        }

        public Metadata.Model Contoso { get; private set; }
    }
}
