namespace TimeTracker.Ui;

public static class StartupManager
{
    private static readonly string ExePath = Application.ExecutablePath;
    private static readonly string StartupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    private static readonly string ShortcutPath = Path.Combine(StartupFolderPath, $"{Path.GetFileNameWithoutExtension(ExePath)}.lnk");
    private static readonly string? CurrentDirectory = Path.GetDirectoryName(ExePath);

    private static dynamic? CreateShellObject()
    {
        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        return shellType != null ? Activator.CreateInstance(shellType) : null;
    }
    private static bool CompareShortcut(dynamic shortcut)
    {
        return string.Equals(shortcut.WorkingDirectory, CurrentDirectory, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(shortcut.TargetPath, ExePath, StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool ShortcutExists()
    {
        try
        {
            if (!File.Exists(ShortcutPath)) return false;

            var shell = CreateShellObject();
            if (shell == null) return false;

            var existingShortcut = shell.CreateShortcut(ShortcutPath);
            return CompareShortcut(existingShortcut);
        }
        catch (Exception)
        {
            // ignore
        }

        return false;
    }

    public static string GetMenuItemText(bool shortcutExists)
    {
        return shortcutExists ? "Launch at system startup ✓" : "Launch at system startup";
    }

    public static void ToggleStartup(ToolStripMenuItem menuItem)
    {
        if (!ShortcutExists()) CreateOrUpdateShortcut();
        else RemoveShortcut();

        menuItem.Text = GetMenuItemText(File.Exists(ShortcutPath));
    }

    private static void CreateOrUpdateShortcut()
    {
        try
        {
            var shell = CreateShellObject();
            if (shell == null) return;

            if (File.Exists(ShortcutPath))
            {
                var existingShortcut = shell.CreateShortcut(ShortcutPath);
                if (CompareShortcut(existingShortcut)) return;
            }

            var shortcut = shell.CreateShortcut(ShortcutPath);
            shortcut.TargetPath = ExePath;
            shortcut.WorkingDirectory = CurrentDirectory;
            shortcut.Save();
        }
        catch (Exception)
        {
            // ignore
        }
    }

    private static void RemoveShortcut()
    {
        try
        {
            if (File.Exists(ShortcutPath)) File.Delete(ShortcutPath);
        }
        catch (Exception)
        {
            // ignore
        }
    }
}