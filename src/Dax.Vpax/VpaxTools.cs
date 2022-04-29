﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
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
                InternalExportVpax(exportVpax, model, viewVpa, database);
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
                InternalExportVpax(exportVpax, model, viewVpa, database);
            }
        }

        internal static void InternalExportVpax(ExportVpax exportVpax, Dax.Metadata.Model model, Dax.ViewVpaExport.Model viewVpa = null, TOM.Database database = null)
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
        public static VpaxContent ImportVpax(string path)
        {
            VpaxContent Content;
            using (ImportVpax importVpax = new ImportVpax(path))
            {
                Content.DaxModel = importVpax.ImportModel();
                Content.ViewVpa = null;
                Content.TomDatabase = importVpax.ImportDatabase();
            }
            return Content;
        }

        /// <summary>
        /// Import from VertiPaq Analyzer (VPAX) file stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static VpaxContent ImportVpax(Stream stream)
        {
            VpaxContent Content;
            using (ImportVpax importVpax = new ImportVpax(stream))
            {
                Content.DaxModel = importVpax.ImportModel();
                Content.ViewVpa = null;
                Content.TomDatabase = importVpax.ImportDatabase();
            }
            return Content;
        }
    }
}
