using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
using Microsoft.AnalysisServices.Tabular;


namespace Dax.Vpax
{
    internal class ImportVpax : IDisposable
    {
        public Package Package { get; private set; }
        public ImportVpax(string path)
        {
            this.Package = Package.Open(path, FileMode.Open);
        }

        public void Close()
        {
            this.Package.Close();
        }

        private string ReadPackageContentAsString(string uriString)
        {
            Uri uriTom = PackUriHelper.CreatePartUri(new Uri(uriString, UriKind.Relative));
            using (TextReader tw = new StreamReader(this.Package.GetPart(uriTom).GetStream(), Encoding.UTF8))
            {
                string content = tw.ReadToEnd();
                tw.Close();
                return content;
            }
        }

        public Model.Model ImportModel()
        {
            string viewVpa = ReadPackageContentAsString(VpaxFormat.DAXMODEL);
            return JsonConvert.DeserializeObject(viewVpa, typeof(Model.Model)) as Model.Model;
        }

        /* ViewVpa cannot be imported - it is designed only to be exported to VertiPaq Analyzer in Excel
        public ViewVpaExport.Model ImportViewVpa()
        {
            string viewVpa = ReadPackageContentAsString(VpaxFormat.DAXVPAVIEW);
            return JsonConvert.DeserializeObject(viewVpa, typeof(ViewVpaExport.Model)) as ViewVpaExport.Model;
        }
        */

        public Microsoft.AnalysisServices.Database ImportDatabase()
        {
            string modelBim = ReadPackageContentAsString(VpaxFormat.TOMMODEL);
            return Microsoft.AnalysisServices.JsonSerializer.DeserializeDatabase(modelBim);
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
