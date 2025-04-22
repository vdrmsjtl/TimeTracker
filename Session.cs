using System.Text.Json.Serialization;

namespace TimeTracker.Ui;

public class Session(TimeSpan startTime)
{
    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan StartTime { get; set; } = startTime;

    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan EndTime { get; set; }

    [JsonInclude] public List<Break> Breaks { get; set; } = [];

    public void AddBreak(Break @break)
    {
        Breaks.Add(@break);
    }

    public TimeSpan GetBreakDuration()
    {
        return Breaks.Aggregate(TimeSpan.Zero, (current, @break) => current + @break.BreakDuration);
    }

    public TimeSpan GetSessionDuration(TimeSpan now)
    {
        return (EndTime == default ? now : EndTime) - StartTime;
    }

    public void EndSession(TimeSpan endTime)
    {
        EndTime = endTime;
    }
}