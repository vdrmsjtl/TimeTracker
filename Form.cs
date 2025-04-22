using System.Text;
using System.Text.Json;
using static System.TimeSpan;

namespace TimeTracker.Ui;

public partial class Form : System.Windows.Forms.Form
{
    private readonly TrackedDay? _currentDay;

    private readonly DateTime _lastMonday =
        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TimeTrackerRecords.json");

    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerOptions.Default)
    {
        WriteIndented = true
    };

    private readonly List<TrackedDay> _trackedDays;
    private TimeSpan _breakStartTime;
    private bool _discardSession;
    private bool _isOnBreak;

    private Icon _pauseIcon;
    private Icon _timerIcon;

    public Form()
    {
        InitializeComponent();

        var now = DateTime.Now;

        _trackedDays = LoadRecordsFromFileDays();
        _currentDay = _trackedDays.FirstOrDefault(d => d.Date == now.Date);

        if (_currentDay == null)
        {
            _currentDay = new TrackedDay { Date = now.Date };
            _currentDay.CreateSession(now.TimeOfDay);
            _trackedDays.Add(_currentDay);
        }
        else
        {
            var openSession = _currentDay.Sessions.LastOrDefault(session => session.EndTime == Zero);
            if (openSession == default)
            {
                _currentDay.CreateSession(now.TimeOfDay);
            }
        }

        UpdateRecords();

        FormClosing += OnFormClosing;
    }

    private string HoursWorked
    {
        get
        {
            var now = DateTime.Now;
            var currentBreakTime = _isOnBreak ? now.TimeOfDay - _breakStartTime : Zero;
            var workedTodayTime = _currentDay.GetWorkedTime(now.TimeOfDay) - currentBreakTime;
            var breaksToday = _currentDay.GetBreakTime() + currentBreakTime;
            var remainingTime = FromHours(8) - workedTodayTime;
            var workedWeek = _trackedDays
                .Where(pr => pr.Date >= _lastMonday && pr.Date < now.Date)
                .Aggregate(Zero, (current, pr) => current.Add(pr.WorkedHours)) + workedTodayTime;

            var sb = new StringBuilder();
            sb.AppendLine($"Today: {FormatTimeSpan(workedTodayTime)}");
            if (breaksToday.TotalSeconds > 0) sb.AppendLine($"Breaks: {FormatTimeSpan(breaksToday)}");
            sb.AppendLine($"{(remainingTime < Zero ? $"Overtime: {FormatTimeSpan(FromTicks(Math.Abs(remainingTime.Ticks)))}" : $"Remaining: {FormatTimeSpan(remainingTime)}")}");
            sb.AppendLine($"This Week: {FormatTimeSpan(workedWeek)}");
            sb.Append($"{(_isOnBreak ? $"\nStatus: On Break ({FormatTimeSpan(currentBreakTime)})" : "\nStatus: Working")}");

            return sb.ToString();
        }
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        var absTimeSpan = FromTicks(Math.Abs(timeSpan.Ticks));
        return $"{absTimeSpan.Days * 24 + absTimeSpan.Hours}h {absTimeSpan.Minutes}m {absTimeSpan.Seconds}s";
    }

    private List<TrackedDay> LoadRecordsFromFileDays()
    {
        try
        {
            var fileContent = File.ReadAllText(_path);
            var data = JsonSerializer.Deserialize<List<TrackedDay>>(fileContent, _serializerOptions) ?? [];

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return [];
        }
    }

    private void UpdateRecords()
    {
#if DEBUG
        return;
#endif
        var serializedString = JsonSerializer.Serialize(_trackedDays, _serializerOptions);
        File.WriteAllText(_path, serializedString);
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_discardSession)
        {
            _currentDay.DiscardSession();
        }
        else
        {
            if (_isOnBreak) StopBreak();
            _currentDay.EndSession(DateTime.Now.TimeOfDay);
        }

        UpdateRecords();
    }

    private void UpdateToolTip()
    {
        _trayIcon.Text = HoursWorked;
    }

    private void BreakContinue()
    {
        if (!_isOnBreak)
            StartBreak();
        else
            StopBreak();
    }

    private void StartBreak()
    {
        _isOnBreak = true;
        _breakStartTime = DateTime.Now.TimeOfDay;
        _trayIcon.Icon = _pauseIcon;
        UpdateToolTip();
    }

    private void StopBreak()
    {
        _isOnBreak = false;
        var now = DateTime.Now.TimeOfDay;
        if (now - _breakStartTime >= FromSeconds(1))
        {
            _currentDay.AddBreak(new Break(_breakStartTime, now));
        }

        _breakStartTime = default;
        _trayIcon.Icon = _timerIcon;
        UpdateToolTip();
    }
}