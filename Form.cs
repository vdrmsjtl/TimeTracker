using Newtonsoft.Json;
using static System.TimeSpan;

namespace TimeTracker.Ui;

public partial class Form : System.Windows.Forms.Form
{
    private readonly TrackedDay _currentDay;

    private readonly DateTime _lastMonday =
        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MyTrackedTimes.json");

    private readonly DateTime _startTime = DateTime.Now;

    private readonly List<TrackedDay> _trackedDays;

    private DateTime _breakStartTime;
    private bool _isOnBreak;

    public Form()
    {
        InitializeComponent();

        _trackedDays = LoadRecordsFromFileDays();
        _currentDay = _trackedDays.FirstOrDefault(d => d.Date.Date == _startTime.Date);

        if (_currentDay == null)
        {
            _currentDay = new TrackedDay();
            _currentDay.SetStartTime(_startTime);
            _trackedDays.Add(_currentDay);

            UpdateRecords();
        }
        else
        {
            _currentDay.AddSession(_startTime);

            UpdateRecords();
        }

        FormClosing += SysTrayApp_FormClosing;
    }

    private string HoursWorked
    {
        get
        {
            var now = DateTime.Now;

            //this day
            var workedTime = _currentDay.GetWorkedTime(now);
            var breakTime = _currentDay.GetBreakTime();

            var endTime = _currentDay.Date.AddHours(8);
            var remainingTime = endTime - now - workedTime + breakTime;

            var lapsedBreakTime = _isOnBreak ? DateTime.Now - _breakStartTime : Zero;

            //this week
            var workedWeek = _trackedDays
                .Where(pr => pr.Date >= _lastMonday && pr.Date < now)
                .Aggregate(Zero, (current, pr) => current.Add(pr.WorkedHours)) + workedTime;

            var lapsedTimeString = $"Lapsed Time: {workedTime.Hours}H {workedTime.Minutes}m {workedTime.Seconds}s";

            var remainingTimeString =
                $"\nRemaining Time: {remainingTime.Hours}H {remainingTime.Minutes}m {remainingTime.Seconds}s";

            var workedWeekString =
                $"\nWorked this Week: {workedWeek.Hours}H {workedWeek.Minutes}m {workedWeek.Seconds}s";

            var statusString = $"\n{(_isOnBreak ? "Status: On Break" : "Status: Working")}";

            var breakTimeString =
                $"{(_isOnBreak ? $"\n\nBreak Time: {lapsedBreakTime.Hours}H {lapsedBreakTime.Minutes}m {lapsedBreakTime.Seconds}s" : string.Empty)}";

            return
                $"{lapsedTimeString}{remainingTimeString}{workedWeekString}{statusString}{breakTimeString}";
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
            return JsonConvert.DeserializeObject<List<TrackedDay>>(fileContent, settings) ??
                   new List<TrackedDay>();
        }
        catch (Exception)
        {
            return new List<TrackedDay>();
        }
    }

    private void SysTrayApp_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var endTime = DateTime.Now;
        _currentDay.SetEndTime(endTime);
        UpdateRecords();
    }

    private void UpdateRecords()
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        File.WriteAllText(_path, JsonConvert.SerializeObject(_trackedDays, Formatting.Indented, settings));
    }

    private void BreakContinue()
    {
        if (!_isOnBreak)
        {
            _isOnBreak = true;
            _breakStartTime = DateTime.Now;
            _trayIcon.Icon = new Icon(Path.Combine(Application.StartupPath, "pause-48.ico"));
        }
        else
        {
            _isOnBreak = false;
            _currentDay.AddBreak(new Break(_breakStartTime, DateTime.Now));
            _breakStartTime = default;
            _trayIcon.Icon = new Icon(Path.Combine(Application.StartupPath, "timer-48.ico"));
            UpdateRecords();
        }
    }
}