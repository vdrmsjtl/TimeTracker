using Newtonsoft.Json;

namespace TimeTracker.Ui;

public class Break(DateTime startTime, DateTime endTime)
{
    [JsonProperty] public DateTime StartTime { get; set; } = startTime;

    [JsonProperty] public DateTime EndTime { get; set; } = endTime;

    public TimeSpan BreakDuration => EndTime - StartTime;
}