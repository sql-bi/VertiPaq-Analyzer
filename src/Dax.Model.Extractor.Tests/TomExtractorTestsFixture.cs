namespace Dax.Model.Extractor.Tests
{
    using Microsoft.AnalysisServices;
    using System.IO;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class TomExtractorTestsFixture
    {
        private const string ContosoBimFilePath = @".\_data\Contoso.bim";
        private const string VacciniBimFilePath = @".\_data\Vaccini.bim";

        public TomExtractorTestsFixture()
        {
            Contoso = GetTomModel(ContosoBimFilePath);
            Vaccini = GetTomModel(VacciniBimFilePath);
        }

        private TOM.Model GetTomModel(string path, CompatibilityMode mode = CompatibilityMode.Unknown)
        {
            var json = File.ReadAllText(path);
            var database = TOM.JsonSerializer.DeserializeDatabase(json, mode: mode);
            return database.Model;
        }

        public TOM.Model Contoso { get; private set; }

        public TOM.Model Vaccini { get; private set; }
    }
}
