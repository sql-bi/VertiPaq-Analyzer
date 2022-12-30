namespace Dax.Model.Extractor.Tests
{
    using Microsoft.AnalysisServices;
    using System.IO;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class ModelFixture
    {
        private const string ContosoFilePath = @".\_data\bim\Contoso.bim";
        private const string TestModel1FilePath = @".\_data\bim\TestModel1.bim";

        public ModelFixture()
        {
            Contoso = GetModel(ContosoFilePath);
            TestModel1 = GetModel(TestModel1FilePath, CompatibilityMode.PowerBI);
        }

        private TOM.Model GetModel(string path, CompatibilityMode mode = CompatibilityMode.Unknown)
        {
            var json = File.ReadAllText(path);
            var database = TOM.JsonSerializer.DeserializeDatabase(json, mode: mode);
            return database.Model;
        }

        public TOM.Model Contoso { get; private set; }

        public TOM.Model TestModel1 { get; private set; }
    }
}
