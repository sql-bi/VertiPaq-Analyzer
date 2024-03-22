using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Tom = Microsoft.AnalysisServices;

namespace Dax.Metadata.Extractor
{
    internal class ExtractorInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public enum DirectLakeExtractionMode
    {

        [Description("Only does a detailed scan of columns that are already in memory")]
        ResidentOnly,
        [Description("Only does a detailed scan of columns referenced by measures or relationships")]
        Referenced,
        [Description("Does a detailed scan of all columns forcing them to be paged into memory")]
        Full
    }

    internal static class Util
    {
        public static ExtractorInfo GetExtractorInfo(object extractorInstance)
        {
            // AssemblyVersion - contains MAJOR version number only. MINOR,REVISION,BUILDNUMBER numbers are always zero.
            // e.g.
            // - CI and RELEASE build: 1.0.0.0

            // AssemblyInformationalVersion - <semanticVersion>-<previewVersion>+<commitHash>
            // e.g.
            // - CI and RELEASE build:
            //          1.2.6+946cc83a3405704663a3f2dbfcbd2a5ae3431088
            //      OR  1.2.6-preview1+946cc83a3405704663a3f2dbfcbd2a5ae3431088

            var assembly = extractorInstance.GetType().Assembly;
            var assemblyName = assembly.GetName();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return new ExtractorInfo
            {
                Name = assemblyName.Name,
                Version = fileVersionInfo.ProductVersion
            };
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize = 50)
        {
            for (int i = 0; i < locations.Count; i += nSize) {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public static string GetPowerBIPaaSConnectionType(Tom.ConnectionInfo connectionInfo)
        {
            if (connectionInfo.ConnectionType != Tom.ConnectionType.Http)
                return null;

            try {
                var uri = new Uri(connectionInfo.Server, UriKind.Absolute);
                var scheme = uri.Scheme.ToLowerInvariant();

                if (scheme == "asazure" || scheme == "pbidedicated" || scheme == "powerbi" || scheme == "pbiazure")
                    return scheme;

                return "other";
            }
            catch {
                return "unknown";
            }
        }
    }
}
