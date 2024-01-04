using System.IO;
using Dax.Tcdx.Metadata;

namespace Dax.Tcdx.Tools
{
    public static class TcdxTools
    {
        /// <summary>
        /// Export to TDCX stream
        /// </summary>
        public static void ExportTcdx(Stream stream, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            using (ExportTcdx exportTcdx = new ExportTcdx(stream))
            {
                ExportTcdxImpl(exportTcdx, consumers, queryGroups);
            }

            stream.Position = 0L;
        }

        /// <summary>
        /// Export to TDCX file
        /// </summary>
        public static void ExportTcdx(string path, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
            using (ExportTcdx exportVpax = new ExportTcdx(path)) 
            {
                ExportTcdxImpl(exportVpax, consumers, queryGroups);
            }
        }
        internal static void ExportTcdxImpl(ExportTcdx exportTcdx, ConsumersCollection consumers, QueryGroupsCollection queryGroups)
        {
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
            public ConsumersCollection Consumers;
            public QueryGroupsCollection QueryGroups;
        }

        public static TcdxContent ImportTcdx(string path)
        {
            TcdxContent Content;
            using (ImportTcdx importTcdx = new ImportTcdx(path))
            {
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
                Content.Consumers = importTcdx.ImportConsumers();
                Content.QueryGroups = importTcdx.ImportQueryGroups();
            }
            return Content;
        }
    }
}
