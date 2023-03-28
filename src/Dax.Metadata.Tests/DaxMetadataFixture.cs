namespace Dax.Metadata.Tests
{
    using Dax.Vpax.Tools;

    public class DaxMetadataFixture
    {
        private const string ContosoVpaxFilePath = @".\_data\Contoso.vpax";
        private const string VacciniVpaxFilePath = @".\_data\Vaccini.vpax";

        public DaxMetadataFixture()
        {
            Contoso = VpaxTools.ImportVpax(ContosoVpaxFilePath).DaxModel;
            Vaccini = VpaxTools.ImportVpax(VacciniVpaxFilePath).DaxModel;
        }

        public Metadata.Model Contoso { get; private set; }

        public Metadata.Model Vaccini { get; private set; }
    }
}
