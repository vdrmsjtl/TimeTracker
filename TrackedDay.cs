using Newtonsoft.Json;

namespace TimeTracker.Ui;

public class TrackedDay
{
    [JsonProperty] public DateTime Date { get; set; }

    [JsonProperty] public TimeSpan WorkedHours { get; set; }

    [JsonProperty] private List<Session> Sessions { get; set; } = new List<Session>();

    private Session CurrentSession => Sessions.LastOrDefault() ?? new Session(Date);

    public void SetStartTime(DateTime startTime)
    {
        Date = startTime;
        AddSession(startTime);
    }
    
    public void SetEndTime(DateTime endTime)
    {
        CurrentSession.SetEndTime(endTime);
        WorkedHours = GetWorkedTime(endTime);
    }

    public TimeSpan GetBreakTime()
    {
        return CurrentSession.GetTotalBreakTime();
    }

    public TimeSpan GetWorkedTime(DateTime now)
    {
        var totalWorkedTime = Sessions
            .Select(session => session.SessionDuration(now))
            .Aggregate(TimeSpan.Zero, (current, sessionTime) => current + sessionTime);

        return totalWorkedTime;
    }

    public void AddBreak(Break @break)
    {
        CurrentSession.AddBreak(@break);
    }

    public void AddSession(DateTime startTime)
    {
        Sessions.Add(new Session(startTime));
    }
}