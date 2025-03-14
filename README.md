# TimeTracker

Time Tracker is a .NET app aimed at tracking your working time. It conveniently calculates hours worked, break durations and provides total worked times for the day and week.
When added to Windows Autostart, it tracks all your working times automatically.

---

## 📌 **Features**
- ✅ Automatically track hours worked
- ✅ Calculate break times (including current break time)
- ✅ Determine remaining time to finish an 8hr workday 
- ✅ Weekly worked hours summary
- ✅ Informative tray icon status updates

![image](https://github.com/user-attachments/assets/6c40e85a-8fd4-4626-87a5-786aa870a27e)
![image](https://github.com/user-attachments/assets/9afd8fda-0f39-468d-9f15-b4b8ec64c920)

## **Usage**

Once executed, Time Tracker, represented by a Task Tray icon, immediately starts tracking your work hours. Below are the Time Tracker's operations:

- Hover over the task tray icon to see the current status including worked hours, remaining time, status, and break time.
- Double-click on the icon to take a break or continue working.
- Right-click the icon and select "Take a Break / Continue" to start or stop a break or select "Exit" to close the application.
- On closing the app, the current session ends and worked times (including breaks) are saved.

**Tip**: Add a shortcut to Windows Autostart

🗃️ File with all records can be found in
   ```sh
  %APPDATA%\Roaming
   ```
   ```sh
  %APPDATA%\Roaming\TimeTrackerTimes.json
   ```

---

## **🛠️ How to Build**
1. **Clone the repository**:
   ```sh
   git clone https://github.com/vdrmsjtl/TimeTracker.git
   cd TimeTracker
   ```
2. **Build the solution**:
   ```sh
   dotnet build -c Release
   ```

---

- ## ⚡Dependencies
- .NET 8.0
- Newtonsoft.Json
