﻿using System;
using System.Text;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
using TOM = Microsoft.AnalysisServices.Tabular;
using Dax.Vpax.Json;

namespace Dax.Vpax
{
    internal class ExportVpax : IDisposable
    {
        public Package Package { get; private set; }

        public ExportVpax(string path) 
        {
            this.Package = Package.Open(path, FileMode.Create);
        }

        public ExportVpax(Stream stream)
        {
            this.Package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
        }

        public void Close()
        {
            this.Package.Close();
        }

        public void ExportDatabase(TOM.Database database)
        {
            Uri uriTom = PackUriHelper.CreatePartUri(new Uri(VpaxFormat.TOMMODEL, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriTom, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8))
            {
                tw.Write(TOM.JsonSerializer.SerializeDatabase(database));
                tw.Close();
            }
            ExportCompatibilityMode(database);
        }

        public void ExportCompatibilityMode(TOM.Database database)
        {
            Uri uriTom = PackUriHelper.CreatePartUri(new Uri(VpaxFormat.COMPATMODE, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriTom, "text/plain", CompressionOption.Maximum).GetStream(), Encoding.UTF8)) {
                tw.Write(database.CompatibilityMode.ToString());
                tw.Close();
            }
        }

        public void ExportViewVpa(ViewVpaExport.Model viewVpa)
        {
            Uri uriModelVpa = PackUriHelper.CreatePartUri(new Uri(VpaxFormat.DAXVPAVIEW, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriModelVpa, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8)) {
                var value = JsonConvert.SerializeObject(viewVpa, Formatting.None, new JsonSerializerSettings
                {
                    ContractResolver = ShouldSerializeContractResolver.Instance,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                });
                tw.Write(value);
                tw.Close();
            }
        }

        public void ExportModel(Metadata.Model model)
        {
            Uri uriModel = PackUriHelper.CreatePartUri(new Uri(VpaxFormat.DAXMODEL, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriModel, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8)) {
                var value = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
                {
                    ContractResolver = ShouldSerializeContractResolver.Instance,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
                tw.Write(value);
                tw.Close();
            }
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
