using System.IO;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace Dax.Vpax.Tools
{
    public static class VpaxTools
    {
        /// <summary>
        /// Export to VertiPaq Analyzer (VPAX) stream
        /// </summary>
        public static void ExportVpax(Stream stream, Dax.Metadata.Model model, Dax.ViewVpaExport.Model viewVpa = null, TOM.Database database = null)
        {
            using (ExportVpax exportVpax = new ExportVpax(stream))
            {
                ExportVpaxImpl(exportVpax, model, viewVpa, database);
            }

            stream.Position = 0L;
        }

        /// <summary>
        /// Export to VertiPaq Analyzer (VPAX) file
        /// </summary>
        public static void ExportVpax(string path, Dax.Metadata.Model model, Dax.ViewVpaExport.Model viewVpa = null, TOM.Database database = null)
        {
            using (ExportVpax exportVpax = new ExportVpax(path))
            {
                ExportVpaxImpl(exportVpax, model, viewVpa, database);
            }
        }

        internal static void ExportVpaxImpl(ExportVpax exportVpax, Dax.Metadata.Model model, Dax.ViewVpaExport.Model viewVpa = null, TOM.Database database = null)
        {
            if (model != null)
            {
                exportVpax.ExportModel(model);
            }
            if (viewVpa != null)
            {
                exportVpax.ExportViewVpa(viewVpa);
            }
            if (database != null)
            {
                exportVpax.ExportDatabase(database);
            }
            exportVpax.Close();
        }

        public struct VpaxContent
        {
            public Dax.Metadata.Model DaxModel;
            public Dax.ViewVpaExport.Model ViewVpa;
            public TOM.Database TomDatabase;
        }

        /// <summary>
        /// Import from VertiPaq Analyzer (VPAX) file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VpaxContent ImportVpax(string path, bool importDatabase = true)
        {
            VpaxContent Content;
            using (ImportVpax importVpax = new ImportVpax(path))
            {
                Content.DaxModel = importVpax.ImportModel();
                Content.ViewVpa = null;
                Content.TomDatabase = importDatabase ? importVpax.ImportDatabase() : null;
            }
            return Content;
        }

        /// <summary>
        /// Import from VertiPaq Analyzer (VPAX) file stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VpaxContent ImportVpax(Stream stream, bool importDatabase = true)
        {
            VpaxContent Content;
            using (ImportVpax importVpax = new ImportVpax(stream))
            {
                Content.DaxModel = importVpax.ImportModel();
                Content.ViewVpa = null;
                Content.TomDatabase = importDatabase ? importVpax.ImportDatabase() : null;
            }
            return Content;
        }
    }
}
