# TimeTracker  [![.NET](https://github.com/vdrmsjtl/TimeTracker/actions/workflows/dotnet.yml/badge.svg)](https://github.com/vdrmsjtl/TimeTracker/actions/workflows/dotnet.yml) [Download v1.0.10 (TimeTracker.zip)](https://github.com/user-attachments/files/19855873/TimeTracker.zip) 🗂️

Time Tracker is a .NET app aimed at tracking your working time. It conveniently calculates hours worked, break durations and provides total worked times for the day and week.
When added to Windows Autostart, it tracks all your working times automatically.

---

## 📌 **Features**
- ✅ Automatically track hours worked
- ✅ Calculate break times (including current break time)
- ✅ Determine remaining time to finish an 8hr workday 
- ✅ Weekly worked hours summary
- ✅ Informative tray icon status updates

![image](https://github.com/user-attachments/assets/f1dce4c4-7558-427c-8a31-0e403162339d)
![image](https://github.com/user-attachments/assets/384454eb-e967-4e9a-8cdf-40e6d740bf14)
![image](https://github.com/user-attachments/assets/93113f2f-b28e-4b15-b4f9-334f7a4845da)


## **Usage**

Once executed, Time Tracker, represented by a Task Tray icon, immediately starts tracking your work hours. Below are the Time Tracker's operations:

- Hover over the task tray icon to see the current status including worked hours, remaining time, status, and break time.
- Double-click on the icon to take a break or continue working.
- Right-click the icon and select "Take a Break / Continue" to start or stop a break or select "Exit" to close the application.
- On closing the app, the current session ends and worked times (including breaks) are saved.

**Tip**: **Add a shortcut to Windows Autostart!**

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
