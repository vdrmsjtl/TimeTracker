using Newtonsoft.Json;

namespace TimeTracker.Ui;

public class Session(DateTime startTime)
{
    [JsonProperty] public DateTime StartTime { get; set; } = startTime;

    [JsonProperty] public DateTime EndTime { get; set; }

    [JsonProperty] public List<Break> Breaks { get; set; } = new();

    public void AddBreak(Break @break)
    {
        Breaks.Add(@break);
    }

    public TimeSpan GetTotalBreakTime()
    {
        return Breaks.Aggregate(TimeSpan.Zero, (current, @break) => current + @break.BreakDuration);
    }

    public TimeSpan SessionDuration(DateTime now)
    {
        return (EndTime == default ? now : EndTime) - StartTime;
    }

    public void SetEndTime(DateTime endTime)
    {
        EndTime = endTime;
    }
}