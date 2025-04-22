using System.Text.Json.Serialization;

namespace TimeTracker.Ui;

public class TrackedDay
{
    [JsonConstructor]
    public TrackedDay()
    {
    }

    [JsonInclude]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime Date { get; set; }

    [JsonInclude]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan WorkedHours { get; set; }

    [JsonInclude] private List<Session> Sessions { get; set; } = [];

    [JsonIgnore] private Session CurrentSession => Sessions.LastOrDefault() ?? new Session(Date.TimeOfDay);

    public void EndSession(TimeSpan endTime)
    {
        CurrentSession.EndSession(endTime);
        WorkedHours = GetWorkedTime(endTime);
    }

    public TimeSpan GetBreakTime()
    {
        var totalBreakTime = Sessions
            .Select(s => s.GetBreakDuration())
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalBreakTime;
    }

    public TimeSpan GetWorkedTime(TimeSpan now)
    {
        var totalWorkedTime = Sessions
            .Select(session => session.GetSessionDuration(now))
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalWorkedTime;
    }

    public void AddBreak(Break @break)
    {
        CurrentSession.AddBreak(@break);
    }

    public void CreateSession(TimeSpan startTime)
    {
        Sessions.Add(new Session(startTime));
    }

    public void CleanUpSessions()
    {
        Sessions.RemoveAll(session => session != CurrentSession && session.EndTime == default);
        foreach (var session in Sessions)
        {
            session.Breaks.RemoveAll(b => b.EndTime == default);
        }
    }

    public void DiscardSession()
    {
        if (Sessions.Count != 0) Sessions.RemoveAt(Sessions.Count - 1);
    }
}