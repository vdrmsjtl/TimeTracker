using Newtonsoft.Json;

namespace TimeTracker.Ui;

public class TrackedDay
{
    [JsonProperty] public DateTime Date { get; set; }

    [JsonProperty] public TimeSpan WorkedHours { get; set; }

    [JsonProperty] private List<Session> Sessions { get; set; } = new();

    private Session CurrentSession => Sessions.LastOrDefault() ?? new Session(Date);

    public void SetStartTime(DateTime startTime)
    {
        Date = startTime;
        CreateSession(startTime);
    }

    public void EndSession(DateTime endTime)
    {
        CurrentSession.EndSession(endTime);
        WorkedHours = GetWorkedTime(endTime);
    }

    public TimeSpan GetBreakTime()
    {
        var totalBreakTime = Sessions
            .Select(s => s.BreakDuration())
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalBreakTime;
    }

    public TimeSpan GetWorkedTime(DateTime now)
    {
        var totalWorkedTime = Sessions
            .Select(session => session.SessionDuration(now))
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalWorkedTime;
    }

    public void AddBreak(Break @break)
    {
        CurrentSession.AddBreak(@break);
    }

    public void CreateSession(DateTime startTime)
    {
        Sessions.Add(new Session(startTime));
    }
}