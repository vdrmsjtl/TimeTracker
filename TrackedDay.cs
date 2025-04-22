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

    [JsonInclude] public List<Session> Sessions { get; set; } = [];

    [JsonIgnore] private Session CurrentSession => Sessions.LastOrDefault() ?? new Session(Date.TimeOfDay);

    public void EndSession(TimeSpan endTime)
    {
        CurrentSession.EndSession(endTime);
        WorkedHours = GetWorkedTime(endTime);
    }

    public TimeSpan GetBreakTime()
    {
        var totalBreakTime = Sessions
            .Select(session => session.GetBreakDuration())
            .Aggregate(TimeSpan.Zero, (current, t) => current + t);

        return totalBreakTime;
    }

    public TimeSpan GetWorkedTime(TimeSpan now)
    {
        var totalWorkedTime = Sessions
            .Select(session => session.GetSessionDuration(now) - session.GetBreakDuration())
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

    public void DiscardSession()
    {
        if (Sessions.Count != 0) Sessions.RemoveAt(Sessions.Count - 1);
    }
}