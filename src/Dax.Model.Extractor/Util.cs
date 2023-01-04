﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dax.Metadata.Extractor
{
    internal class ExtractorInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
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

    }
}
