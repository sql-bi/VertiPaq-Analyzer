using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
using TOM = Microsoft.AnalysisServices.Tabular;
using Microsoft.AnalysisServices;

namespace Dax.Vpax
{
    internal class ImportVpax : IDisposable
    {
        public Package Package { get; private set; }
        public ImportVpax(string path)
        {
            this.Package = Package.Open(path, FileMode.Open);
        }
        public ImportVpax(Stream stream)
        {
            this.Package = Package.Open(stream);
        }

        public void Close()
        {
            this.Package.Close();
        }

        private string ReadPackageContentAsString(string uriString)
        {
            Uri uriTom = PackUriHelper.CreatePartUri(new Uri(uriString, UriKind.Relative));
            if (!this.Package.PartExists(uriTom)) return null;

            var part = this.Package.GetPart(uriTom);
            using (TextReader tw = new StreamReader(part.GetStream(), Encoding.UTF8))
            {
                string content = tw.ReadToEnd();
                tw.Close();
                return content;
            }
        }

        private T DeserializePackageContent<T>(string uriString)
        {
            var partUri = PackUriHelper.CreatePartUri(new Uri(uriString, UriKind.Relative));

            if (!this.Package.PartExists(partUri)) 
                return default(T);
            
            var packagePart = this.Package.GetPart(partUri);

            using (var stream = packagePart.GetStream())
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public Metadata.Model ImportModel()
        {
            return DeserializePackageContent<Metadata.Model>(VpaxFormat.DAXMODEL);
        }

        /* ViewVpa cannot be imported - it is designed only to be exported to VertiPaq Analyzer in Excel
        public ViewVpaExport.Model ImportViewVpa()
        {
            string viewVpa = ReadPackageContentAsString(VpaxFormat.DAXVPAVIEW);
            return JsonConvert.DeserializeObject(viewVpa, typeof(ViewVpaExport.Model)) as ViewVpaExport.Model;
        }
        */

        public TOM.Database ImportDatabase()
        {
            string strCompatMode = ReadPackageContentAsString(VpaxFormat.COMPATMODE);
            Microsoft.AnalysisServices.CompatibilityMode compatMode = Microsoft.AnalysisServices.CompatibilityMode.Unknown;
            Enum.TryParse(strCompatMode,true , out compatMode );
            string modelBim = ReadPackageContentAsString(VpaxFormat.TOMMODEL);
            if (modelBim == null) return null;
            var tomDb = TryDeserializeDatabase(modelBim, compatMode);
            return tomDb;
        }

        // This method is mainly here for backward compatibility
        // if an existing vpax file has been created that will not deserialize with the default
        // compat mode of 'Unknown' and we get a JsonSerializationException, then we recursively
        // re-try with the other compat modes to see if they work.
        private TOM.Database TryDeserializeDatabase(string modelBim, CompatibilityMode compatMode)
        {
            TOM.Database db = null;
            try {
                db = TOM.JsonSerializer.DeserializeDatabase(modelBim, null, compatMode);
            }
            catch (Microsoft.AnalysisServices.JsonSerializationException) {
                // if we hit an error of any sort try to deserialize using the different compat modes
                // in the following order: PowerBI, AnalysisServices, Excel
                switch (compatMode) {
                    case CompatibilityMode.Unknown:
                        db = TryDeserializeDatabase(modelBim, CompatibilityMode.PowerBI);
                        break;
                    case CompatibilityMode.PowerBI:
                        db = TryDeserializeDatabase(modelBim, CompatibilityMode.AnalysisServices);
                        break;
                    case CompatibilityMode.AnalysisServices:
                        db = TryDeserializeDatabase(modelBim, CompatibilityMode.Excel);
                        break;
                     default:
                        db = null;
                        break;
                }
            }
            return db;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)this.Package)?.Dispose();
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
