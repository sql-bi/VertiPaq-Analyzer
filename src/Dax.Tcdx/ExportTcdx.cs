using System;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
using Dax.Tcdx.Metadata;

namespace Dax.Tcdx
{
    internal class ExportTcdx : IDisposable
    {
        public Package Package { get; private set; }

        public ExportTcdx(string path) 
        {
            this.Package = Package.Open(path, FileMode.Create);
        }

        public ExportTcdx(Stream stream)
        {
            this.Package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
        }

        public void Close()
        {
            this.Package.Close();
        }
        public void ExportConsumers(ConsumersCollection consumers)
        {
            Uri uriModel = PackUriHelper.CreatePartUri(new Uri(TcdxFormat.CONSUMERS, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriModel, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8))
            {
                tw.Write(
                    JsonConvert.SerializeObject(
                        consumers,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.All,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                        }
                    )
                );
                tw.Close();
            }
        }

        public void ExportQueryGroups(QueryGroupsCollection queryGroups)
        {
            Uri uriModel = PackUriHelper.CreatePartUri(new Uri(TcdxFormat.QUERY_GROUPS, UriKind.Relative));
            using (TextWriter tw = new StreamWriter(this.Package.CreatePart(uriModel, "application/json", CompressionOption.Maximum).GetStream(), Encoding.UTF8)) {
                tw.Write(
                    JsonConvert.SerializeObject(
                        queryGroups,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.All,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                        }
                    )
                );
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
