using Newtonsoft.Json;
using static System.TimeSpan;

namespace TimeTracker.Ui;

public partial class Form : System.Windows.Forms.Form
{
    private readonly Dictionary<string, object> _currentDateRecord;

    private readonly DateTime _lastMonday =
        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MyTrackedTimes.json");

    private readonly List<Dictionary<string, object>> _records;

    private readonly DateTime _startTime = DateTime.Now;

    private DateTime _breakStartTime;
    private bool _isOnBreak;

    public Form()
    {
        InitializeComponent();
        _records = LoadRecordsFromFile(_path);
        _currentDateRecord = _records.FirstOrDefault(r => DateTime.Parse(r["Date"].ToString()) == _startTime.Date);

        if (_currentDateRecord == null)
        {
            _currentDateRecord = new Dictionary<string, object>
            {
                { "Date", _startTime.ToShortDateString() },
                { "Start", _startTime.ToShortTimeString() },
                { "End", "" },
                { "Break", "" },
                { "Work", "" }
            };

            _records.Add(_currentDateRecord);

            File.WriteAllText(_path, JsonConvert.SerializeObject(_records));
        }

        FormClosing += SysTrayApp_FormClosing;
    }

    private string HoursWorked
    {
        get
        {
            var now = DateTime.Now;

            //this day
            var startTime = DateTime.ParseExact(_currentDateRecord["Start"].ToString(), "HH:mm", null);
            var endTime = startTime.AddHours(8);
            if (!TryParse(_currentDateRecord["Break"].ToString(), out var breaks)) breaks = default;

            var duration = now - startTime - breaks;
            var lapsedBreakTime = _isOnBreak ? DateTime.Now - _breakStartTime : Zero;
            var remainingTime = endTime - now + breaks + lapsedBreakTime;

            //this week
            var parsedRecords = _records
                .TakeLast(5)
                .Select(r =>
                {
                    TryParse(r["Work"].ToString(), out var work);

                    return new
                    {
                        Date = DateTime.Parse(r["Date"].ToString()),
                        Work = work
                    };
                })
                .ToList();

            var workedWeek = parsedRecords
                .Where(pr => pr.Date >= _lastMonday && pr.Date < now)
                .Aggregate(Zero, (current, pr) => current.Add(pr.Work)) + (duration - breaks);

            return
                $"Lapsed Time: {duration.Hours}H {duration.Minutes}m {duration.Seconds}s\nRemaining Time: {remainingTime.Hours}H {remainingTime.Minutes}m {duration.Seconds}s\nWorked this Week: {workedWeek.Hours}H {workedWeek.Minutes}m {workedWeek.Seconds}s\n{(_isOnBreak ? "Status: On Break" : "Status: Working")}{(_isOnBreak ? $"\n\nBreak Time: {lapsedBreakTime.Hours}H {lapsedBreakTime.Minutes}m {lapsedBreakTime.Seconds}s" : string.Empty)}";
        }
    }

    private List<Dictionary<string, object>> LoadRecordsFromFile(string path)
    {
        try
        {
            var fileContent = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fileContent) ??
                   new List<Dictionary<string, object>>();
        }
        catch (Exception)
        {
            return new List<Dictionary<string, object>>();
        }
    }

    private void SysTrayApp_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var endTime = DateTime.Now;
        _currentDateRecord["End"] = endTime.ToShortTimeString();

        var startTime = DateTime.Parse(_currentDateRecord["Start"].ToString());
        if (!TryParse(_currentDateRecord["Break"].ToString(), out var breakTime)) breakTime = default;

        var workTime = endTime - startTime - breakTime;
        _currentDateRecord["Work"] = workTime.ToString();

        UpdateRecords();
    }

    private void UpdateRecords()
    {
        var existingRecordIndex = _records.FindIndex(r => DateTime.Parse(r["Date"].ToString()) == _startTime.Date);
        if (existingRecordIndex >= 0)
            _records[existingRecordIndex] = _currentDateRecord;
        else
            _records.Add(_currentDateRecord);

        File.WriteAllText(_path, JsonConvert.SerializeObject(_records, Formatting.Indented));
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
            var lapsedBreakTime = DateTime.Now - _breakStartTime;
            if (!TryParse(_currentDateRecord["Break"].ToString(), out var breakTime)) breakTime = default;

            _currentDateRecord["Break"] = breakTime + lapsedBreakTime;
            _breakStartTime = default;
            _trayIcon.Icon = new Icon(Path.Combine(Application.StartupPath, "timer-48.ico"));
            UpdateRecords();
        }
    }
}