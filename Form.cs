using System.Text;
using Newtonsoft.Json;
using static System.TimeSpan;

namespace TimeTracker.Ui;

public partial class Form : System.Windows.Forms.Form
{
    private readonly TrackedDay? _currentDay;

    private readonly DateTime _lastMonday =
        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TimeTrackerTimes.json");
    
    private readonly List<TrackedDay> _trackedDays;

    private DateTime _breakStartTime;
    private readonly DateTime _sessionStartTime;
    private bool _isOnBreak;
    private Icon _pauseIcon;
    private Icon _timerIcon;

    public Form()
    {
        InitializeComponent();
        
        var now = DateTime.Now;
        
        _trackedDays = LoadRecordsFromFileDays();
        _currentDay = _trackedDays.FirstOrDefault(d => d.Date.Date == now.Date);

        if (_currentDay == null)
        {
            _currentDay = new TrackedDay(now);
            _trackedDays.Add(_currentDay);
        }
        else
        {
            _currentDay.CreateSession(now);
        }

        _trackedDays.ForEach(day => day.CleanUpSessions());
        UpdateRecords();

        _sessionStartTime = now;

        FormClosing += SysTrayApp_FormClosing;
    }

    private string HoursWorked
    {
        get
        {
            var now = DateTime.Now;
            var currentBreakTime = _isOnBreak ? now - _breakStartTime : Zero;

            var workedTime = _currentDay.GetWorkedTime(now);
            var breakTime = _currentDay.GetBreakTime();

            var workedTodayTime = workedTime - currentBreakTime;
            var breaksToday = breakTime + currentBreakTime;

            var remainingTime = FromHours(8) + breakTime - workedTodayTime;

            var workedWeek = _trackedDays
                .Where(pr => pr.Date >= _lastMonday && pr.Date < now.Date)
                .Aggregate(Zero, (current, pr) => current.Add(pr.WorkedHours)) + workedTodayTime;

            var sb = new StringBuilder();
            sb.AppendLine($"Today: {workedTodayTime.Hours}h {workedTodayTime.Minutes}m {workedTodayTime.Seconds}s");

            if (breaksToday.TotalSeconds > 0)
            {
                sb.AppendLine($"Breaks: {breaksToday.Hours}h {breaksToday.Minutes}m {breaksToday.Seconds}s");
            }

            if (remainingTime.TotalSeconds < 0)
            {
                sb.AppendLine($"Overtime: {Math.Abs(remainingTime.Hours)}h {Math.Abs(remainingTime.Minutes)}m {Math.Abs(remainingTime.Seconds)}s");
            }
            else
            {
                sb.AppendLine($"Remaining: {remainingTime.Hours}h {remainingTime.Minutes}m {remainingTime.Seconds}s");
            }

            sb.AppendLine($"This Week: {workedWeek.Hours}h {workedWeek.Minutes}m {workedWeek.Seconds}s");

            if (_isOnBreak)
            {
                sb.AppendLine($"{(_isOnBreak ? $"Break Time: {currentBreakTime.Hours}h {currentBreakTime.Minutes}m {currentBreakTime.Seconds}s" : string.Empty)}");
            }

            sb.Append($"{(_isOnBreak ? "\nStatus: On Break" : "\nStatus: Working")}");

            return sb.ToString();
        }
    }

    private List<TrackedDay> LoadRecordsFromFileDays()
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            var fileContent = File.ReadAllText(_path);
            var data = JsonConvert.DeserializeObject<List<TrackedDay>>(fileContent, settings) ?? [];
            return data.TakeLast(6).ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    private void SysTrayApp_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isOnBreak)
        {
            StopBreak();
        }

        _currentDay.EndSession(DateTime.Now);
        UpdateRecords();
    }

    private void UpdateRecords()
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

#if DEBUG
        return;
#endif    

        File.WriteAllText(_path, JsonConvert.SerializeObject(_trackedDays, Formatting.Indented, settings));
    }

    private void UpdateToolTip()
    {
        _trayIcon.Text = HoursWorked;
    }

    private void BreakContinue()
    {
        if (!_isOnBreak)
        {
            StartBreak();
        }
        else
        {
            StopBreak();
        }
    }

    private void StartBreak()
    {
        _isOnBreak = true;
        _breakStartTime = DateTime.Now;
        _trayIcon.Icon = _pauseIcon;
        UpdateToolTip();
    }

    private void StopBreak()
    {
        _isOnBreak = false;
        _currentDay.AddBreak(new Break(_breakStartTime, DateTime.Now));
        _breakStartTime = default;
        _trayIcon.Icon = _timerIcon;
        UpdateToolTip();
        UpdateRecords();
    }
}