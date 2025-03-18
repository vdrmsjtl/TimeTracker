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
    private DateTime _sessionStartTime;
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

            UpdateRecords();
        }
        else
        {
            _currentDay.CreateSession(now);

            UpdateRecords();
        }

        _sessionStartTime = now;

        FormClosing += SysTrayApp_FormClosing;
    }

    private string HoursWorked
    {
        get
        {
            var now = DateTime.Now;
            var currentBreakTime = _isOnBreak ? now - _breakStartTime : Zero;
            var workedTodayTime = _currentDay.GetWorkedTime(now) - currentBreakTime - (now - _sessionStartTime);
            var remainingTime = FromHours(8) + _currentDay.GetBreakTime() - workedTodayTime;
            var workedWeek = _trackedDays
                .Where(pr => pr.Date >= _lastMonday && pr.Date < now.Date)
                .Aggregate(Zero, (current, pr) => current.Add(pr.WorkedHours)) + workedTodayTime;

            var workedTodayString = $"Worked Today: {workedTodayTime.Hours}h {workedTodayTime.Minutes}m {workedTodayTime.Seconds}s";

            var remainingTimeString = remainingTime.TotalSeconds < 0
                ? $"\nOvertime: {Math.Abs(remainingTime.Hours)}h {Math.Abs(remainingTime.Minutes)}m {Math.Abs(remainingTime.Seconds)}s"
                : $"\nRemaining Time: {remainingTime.Hours}h {remainingTime.Minutes}m {remainingTime.Seconds}s";

            var workedWeekString = $"\nWorked this Week: {workedWeek.Hours}h {workedWeek.Minutes}m {workedWeek.Seconds}s";

            var statusString = $"\n{(_isOnBreak ? "Status: On Break" : "Status: Working")}";

            var breakTimeString = $"{(_isOnBreak ? $"\n\nBreak Time: {currentBreakTime.Hours}h {currentBreakTime.Minutes}m {currentBreakTime.Seconds}s" : string.Empty)}";

            return $"{workedTodayString}{remainingTimeString}{workedWeekString}{statusString}{breakTimeString}";
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
    }

    private void StopBreak()
    {
        _isOnBreak = false;
        _currentDay.AddBreak(new Break(_breakStartTime, DateTime.Now));
        _breakStartTime = default;
        _trayIcon.Icon = _timerIcon;
        UpdateRecords();
    }
}