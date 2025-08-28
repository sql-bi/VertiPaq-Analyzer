using Newtonsoft.Json;

namespace Dax.Metadata
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum TimeUnit
    {
        Unknown = 0,
        Year = 1,
        Semester = 2,
        SemesterOfYear = 3,
        Quarter = 4,
        QuarterOfYear = 5,
        QuarterOfSemester = 6,
        Month = 7,
        MonthOfYear = 8,
        MonthOfSemester = 9,
        MonthOfQuarter = 10,
        Week = 11,
        WeekOfYear = 12,
        WeekOfSemester = 13,
        WeekOfQuarter = 14,
        WeekOfMonth = 15,
        Date = 16,
        DayOfYear = 17,
        DayOfSemester = 18,
        DayOfQuarter = 19,
        DayOfMonth = 20,
        DayOfWeek = 21
    }
}
