using System.Text.Json.Serialization;

namespace TimeTracker.Ui;

public class Break(TimeSpan startTime, TimeSpan endTime)
{
    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan StartTime { get; set; } = startTime;
    
    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan EndTime { get; set; } = endTime;

    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan BreakDuration => EndTime - StartTime;
}