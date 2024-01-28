using System.IO;
using Dax.Tcdx.Metadata;

namespace Dax.Tcdx.Tools
{
    public static class TcdxTools
    {
        /// <summary>
        /// Export to TDCX stream
        /// </summary>
        public static void ExportTcdx(Stream stream, VersionInfo versionInfo, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            using (ExportTcdx exportTcdx = new ExportTcdx(stream))
            {
                ExportTcdxImpl(exportTcdx, versionInfo, consumers, queryGroups);
            }

            stream.Position = 0L;
        }

        /// <summary>
        /// Export to TDCX file
        /// </summary>
        public static void ExportTcdx(string path, VersionInfo versionInfo, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            using (ExportTcdx exportTcdx = new ExportTcdx(path)) 
            {
                ExportTcdxImpl(exportTcdx, versionInfo, consumers, queryGroups);
            }
        }
        internal static void ExportTcdxImpl(ExportTcdx exportTcdx, VersionInfo versionInfo, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            if (versionInfo != null) {
                exportTcdx.ExportVersionInfo(versionInfo);
            }
            if (consumers != null)
            {
                exportTcdx.ExportConsumers(consumers);
            }
            if (queryGroups != null) 
            {
                exportTcdx.ExportQueryGroups(queryGroups);
            }
        }

        public struct TcdxContent
        {
            public VersionInfo VersionInfo;
            public ConsumersCollection Consumers;
            public QueryGroupsCollection QueryGroups;
        }

        public static TcdxContent ImportTcdx(string path)
        {
            TcdxContent Content;
            using (ImportTcdx importTcdx = new ImportTcdx(path))
            {
                Content.VersionInfo = importTcdx.ImportVersionInfo();
                Content.Consumers = importTcdx.ImportConsumers();
                Content.QueryGroups = importTcdx.ImportQueryGroups();
            }
            return Content;
        }

        public static TcdxContent ImportTcdx(Stream stream)
        {
            TcdxContent Content;
            using (ImportTcdx importTcdx = new ImportTcdx(stream))
            {
                Content.VersionInfo = importTcdx.ImportVersionInfo();
                Content.Consumers = importTcdx.ImportConsumers();
                Content.QueryGroups = importTcdx.ImportQueryGroups();
            }
            return Content;
        }
    }
}
