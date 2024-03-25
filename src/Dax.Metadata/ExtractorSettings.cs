namespace Dax.Metadata;

using Newtonsoft.Json;
using System.ComponentModel;

public sealed class ExtractorSettings
{
    /// <summary>
    /// Specifies whether to enable statistics collection from the data instead of relying on the the DMVs. The result is more accurate statistics, but it can be slower.
    /// </summary>
    public bool StatisticsEnabled { get; set; }

    /// <remarks>
    /// This settings only applies when <see cref="StatisticsEnabled"/> is <see langword="true"/>.
    /// </remarks>
    [JsonIgnore] // Ignored in JSON serialization because RI violation values are not serialized
    public int ReferentialIntegrityViolationSamples { get; set; }

    /// <remarks>
    /// This settings only applies when <see cref="StatisticsEnabled"/> is <see langword="true"/>.
    /// </remarks>
    public DirectLakeExtractionMode DirectLakeMode { get; set; }

    /// <remarks>
    /// This settings only applies when <see cref="StatisticsEnabled"/> is <see langword="true"/>.
    /// </remarks>
    public DirectQueryExtractionMode DirectQueryMode { get; set; }
}

public enum DirectLakeExtractionMode
{
    /// <summary>
    /// Only does a detailed scan of columns that are already in memory
    /// </summary>
    [Description("Only does a detailed scan of columns that are already in memory")]
    ResidentOnly = 0,

    /// <summary>
    /// Only does a detailed scan of columns referenced by measures or relationships
    /// </summary>
    [Description("Only does a detailed scan of columns referenced by measures or relationships")]
    Referenced = 1,

    /// <summary>
    /// Does a detailed scan of all columns forcing them to be paged into memory
    /// </summary>
    [Description("Does a detailed scan of all columns forcing them to be paged into memory")]
    Full = 2
}

public enum DirectQueryExtractionMode
{
    /// <summary>
    /// Excludes all DirectQuery tables from statistics collection
    /// </summary>
    None = 0,

    /// <summary>
    /// Includes all DirectQuery tables in statistics collection
    /// </summary>
    Full = 1
}