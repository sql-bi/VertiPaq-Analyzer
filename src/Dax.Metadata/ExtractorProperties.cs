namespace Dax.Metadata;

using System.ComponentModel;

public interface IExtractorProperties
{
    /// <summary>
    /// Specifies whether to enable statistics collection from the data instead of relying on the the DMVs. The result is more accurate statistics, but it can be slower.
    /// </summary>
    bool StatisticsEnabled { get; set; }

    /// <remarks>
    /// This settings only applies when <see cref="StatisticsEnabled"/> is <see langword="true"/>.
    /// </remarks>
    DirectLakeExtractionMode DirectLakeMode { get; set; }

    /// <remarks>
    /// This settings only applies when <see cref="StatisticsEnabled"/> is <see langword="true"/>.
    /// </remarks>
    DirectQueryExtractionMode DirectQueryMode { get; set; }
}

public sealed class ExtractorProperties : IExtractorProperties
{
    public bool StatisticsEnabled { get; set; }
    public DirectLakeExtractionMode DirectLakeMode { get; set; }
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
