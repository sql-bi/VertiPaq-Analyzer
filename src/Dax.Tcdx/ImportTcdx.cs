﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using Newtonsoft.Json;
using TOM = Microsoft.AnalysisServices.Tabular;
using Microsoft.AnalysisServices;

namespace Dax.Tcdx
{
    internal class ImportTcdx : IDisposable
    {
        public Package Package { get; private set; }
        public ImportTcdx(string path)
        {
            this.Package = Package.Open(path, FileMode.Open, FileAccess.Read);
        }
        public ImportTcdx(Stream stream)
        {
            this.Package = Package.Open(stream);
        }

        public void Close()
        {
            this.Package.Close();
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

        public Dax.Consumer.ConsumersCollection ImportConsumers()
        {
            return DeserializePackageContent<Dax.Consumer.ConsumersCollection>(TcdxFormat.CONSUMERS);
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