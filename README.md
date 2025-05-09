# TimeTracker  [![.NET](https://github.com/vdrmsjtl/TimeTracker/actions/workflows/dotnet.yml/badge.svg)](https://github.com/vdrmsjtl/TimeTracker/actions/workflows/dotnet.yml) [Download v1.1.1 (TimeTracker.zip)](https://github.com/user-attachments/files/20122604/TimeTracker.zip)

Time Tracker is a .NET app aimed at tracking your working time. It conveniently calculates hours worked, break durations and provides total worked times for the day and week.
When added to Windows Autostart, it tracks all your working times automatically.

---

## 📌 **Features**
- ✅ Automatically track hours worked
- ✅ Calculate break times (including current break time)
- ✅ Weekly worked hours summary
- ✅ Informative tray icon status updates

![image](https://github.com/user-attachments/assets/cf000a26-bcba-4954-8ed8-a065cdb7a365)
![image](https://github.com/user-attachments/assets/88a8ccfe-677c-4cf8-a83a-336b85f461a8)
![image](https://github.com/user-attachments/assets/6b4272c3-7400-496e-80a1-9407b267eb64)

## **Usage**

Once executed, Time Tracker, represented by a Task Tray icon, immediately starts tracking your work hours. Below are the Time Tracker's operations:

- Hover over the task tray icon to see the current status including worked hours, remaining time, status, and break time.
- Double-click on the icon to take a break or continue working.
- Right-click the icon and select "Launch at system startup" to automatically run the application on Windows startup
- Right-click the icon and select "Take a Break / Continue" to start or stop a break.
- Right-click the icon and select "Discard session & Exit" to discard the current session and close the application without saving.
- Right-click the icon and select "Save & Exit" to end the session and close the application.
- On closing the app, the current session ends and worked times (including breaks) are saved.

**Recommended**: **Add a shortcut to Windows Autostart to automatically run the application on Windows startup!**

---

## **Resources**

🗃️ File with all records can be found in
   ```sh
  %APPDATA%\Roaming\TimeTrackerRecords.json
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
