using Dax.Metadata;

namespace Dax.Model.Extractor;

public class ExtractorSettings : IExtractorProperties
{
    public bool StatisticsEnabled { get; set; } = true;
    public int ReferentialIntegrityViolationSamples { get; set; } = StatExtractor.DefaultReferentialIntegrityViolationSamples;
    public DirectLakeExtractionMode DirectLakeMode { get; set; } = DirectLakeExtractionMode.ResidentOnly;
    public DirectQueryExtractionMode DirectQueryMode { get; set; } = DirectQueryExtractionMode.Full;
    public int CommandTimeout { get; set; } = 0; // default to 0 (no timeout)
}
