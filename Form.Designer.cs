namespace TimeTracker.Ui;

partial class Form
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private NotifyIcon _trayIcon;
    private ContextMenuStrip _trayMenu;
    
    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "TimeTracker";

        _pauseIcon = new Icon(Path.Combine(Application.StartupPath, "pause-48.ico"));
        _timerIcon = new Icon(Path.Combine(Application.StartupPath, "timer-48.ico"));

        _trayMenu = new ContextMenuStrip();
        var exitMenuItem = new ToolStripMenuItem {Text = "Exit"};
        exitMenuItem.Click += OnExit;
        var breakContinueMenuItem = new ToolStripMenuItem {Text = "Take a Break / Continue"};
        breakContinueMenuItem.Click += (sender, args) => this.BreakContinue();
        _trayMenu.Items.Add(breakContinueMenuItem);
        _trayMenu.Items.Add(exitMenuItem);
        
        _trayIcon = new NotifyIcon();
        _trayIcon.Text = String.Empty;
        _trayIcon.Icon = _timerIcon;
 
        _trayIcon.ContextMenuStrip = _trayMenu;
        _trayIcon.Visible = true;
        _trayIcon.MouseMove += TrayIconOnMouseMove;
        _trayIcon.MouseDoubleClick += (sender, args) => this.BreakContinue();
    }
    
    private DateTime _lastRun = DateTime.MinValue;
    
    void TrayIconOnMouseMove(object sender, MouseEventArgs e)
    {
        var now = DateTime.Now;
        if (now.Subtract(_lastRun).TotalSeconds < 1)
            return;
        _trayIcon.Text = HoursWorked;
        _lastRun = now;
    }
    
    protected override void OnLoad(EventArgs e)
    {
        Visible = false;
        ShowInTaskbar = false;

        _trayIcon.Text = string.Empty;

        base.OnLoad(e);
    }
    
    private void OnExit(object sender, EventArgs e)
    {
        Application.Exit();
    }

    #endregion
}