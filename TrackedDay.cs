using Newtonsoft.Json;

namespace TimeTracker.Ui;

public class TrackedDay
{
    [JsonProperty] public DateTime Date { get; set; }

    [JsonProperty] public TimeSpan WorkedHours { get; set; }

    [JsonProperty] private List<Session> Sessions { get; set; } = new();

    [JsonIgnore] private Session CurrentSession => Sessions.LastOrDefault() ?? new Session(Date);

    public void CleanUpSessions()
    {
        var sessionsToRemove = new List<Session>();

        foreach (var session in Sessions)
        {
            session.Breaks.RemoveAll(b => b.EndTime == default);

            if (session != CurrentSession && session.EndTime == default)
                sessionsToRemove.Add(session);
        }

        foreach (var session in sessionsToRemove)
        {
            Sessions.Remove(session);
        }
    }

    public TrackedDay(DateTime startTime)
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
            .Select(s => s.GetBreakDuration())
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalBreakTime;
    }

    public TimeSpan GetWorkedTime(DateTime now)
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

    public void CreateSession(DateTime startTime)
    {
        Sessions.Add(new Session(startTime));
    }
}