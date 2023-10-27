using System.IO;
using Dax.Tcdx.Metadata;

namespace Dax.Tcdx.Tools
{
    public static class TcdxTools
    {
        /// <summary>
        /// Export to TDCX stream
        /// </summary>
        public static void ExportTcdx(Stream stream, ConsumersCollection consumers)
        {
            using (ExportTcdx exportTcdx = new ExportTcdx(stream))
            {
                ExportTcdxImpl(exportTcdx, consumers);
            }

            stream.Position = 0L;
        }

        /// <summary>
        /// Export to TDCX file
        /// </summary>
        public static void ExportTcdx(string path, ConsumersCollection consumers)
        {
            using (ExportTcdx exportVpax = new ExportTcdx(path))
            {
                ExportTcdxImpl(exportVpax, consumers);
            }
        }

        internal static void ExportTcdxImpl(ExportTcdx exportTcdx, ConsumersCollection consumers)
        {
            if (consumers != null)
            {
                exportTcdx.ExportConsumers(consumers);
            }
        }

        public struct TcdxContent
        {
            public ConsumersCollection Consumers;
        }

        /// <summary>
        /// Import from VertiPaq Analyzer (VPAX) file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TcdxContent ImportTcdx(string path, bool importDatabase = true)
        {
            TcdxContent Content;
            using (ImportTcdx importTcdx = new ImportTcdx(path))
            {
                Content.Consumers = importTcdx.ImportConsumers();
            }
            return Content;
        }

        /// <summary>
        /// Import from VertiPaq Analyzer (VPAX) file stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TcdxContent ImportTcdx(Stream stream, bool importDatabase = true)
        {
            TcdxContent Content;
            using (ImportTcdx importTcdx = new ImportTcdx(stream))
            {
                Content.Consumers = importTcdx.ImportConsumers();
            }
            return Content;
        }
    }
}
