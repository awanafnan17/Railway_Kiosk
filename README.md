# Railway Station Kiosk – WinForms Prototype

This repository contains a **Windows Forms** prototype of a status query kiosk for a railway station.  The project follows the guidelines of the Human–Computer Interaction (HCI) lab assignments provided in the uploaded course materials and implements a front‑end for travellers to query train schedules and statuses.  It is written in **C#** and uses the **Guna2** UI library to deliver a clean, modern interface.  The interface has been expanded to include the screens described in the mock‑ups: a welcome/home menu, separate search and status screens, a help & accessibility page and a feedback form.

## Features and Screens

The application is organised into several screens to simplify navigation:

1. **Welcome/Home** – The starting point.  A large title and clearly labelled buttons allow the user to navigate to the search screen, train status screen, help & accessibility screen or feedback form.  A (placeholder) language selector hints at localisation support.

2. **Search Train** – Users can search by train number and filter by destination, train type and approximate departure time.  Matching trains are displayed in a table.  Selecting a row and clicking “Speak” reads the train’s details aloud via SAPI.

3. **Train Status** – Enter a train number or select one from a drop‑down and view its current status.  Status messages are colour coded (green for on time, yellow for delayed, red for cancelled).  This screen also shows scheduled arrival/departure times.

4. **Help & Accessibility** – Provides instructions on how to use the kiosk and offers controls to enlarge or shrink text and toggle a high‑contrast colour scheme.  This ensures the kiosk remains usable for people with visual impairments.

5. **Feedback** – A simple form where users can leave a rating (future work) and a free‑form comment.  Submissions are saved to timestamped files under the `Feedback` folder.

6. **Admin Login (optional)** – Before accessing the system, an admin can log in using credentials stored as a salted PBKDF2 hash.  Guests may bypass this step and proceed directly to the home screen.

## Additional Details

## Dependencies

To build and run the prototype you will need:

* [.NET 6 SDK for Windows](https://dotnet.microsoft.com/en-us/download) – required to compile WinForms projects.
* **Guna.UI2.WinForms** NuGet package – provides modern WinForms controls.  Restore NuGet packages in Visual Studio or via `dotnet restore`.
* **System.Speech** NuGet package – enables the text‑to‑speech functionality.  This package wraps the Windows Speech API (SAPI).

## Building and Running

1. On Windows, run `scripts/setup.ps1` to install .NET Desktop Runtime 6 (and SDK if needed).
2. Run `scripts/build.ps1` to restore, build and execute tests.
3. Run `scripts/run.ps1` to start the app from the build output, or use `scripts/publish.ps1` to produce a self‑contained folder.
4. At startup the login form appears. Use `admin`/`admin123` or continue as guest.

## Notes

* Train data is read from `TrainData\trains.json`.
* Logs are written under `%LOCALAPPDATA%\RailwayKiosk\Logs`.
* Feedback is stored under `%LOCALAPPDATA%\RailwayKiosk\Feedback`.
* If the window does not appear when launching from a non‑interactive session, run the exe directly from Explorer.
* Feedback files are written to the `Feedback` subdirectory in the application output folder.  They are not transmitted anywhere; the goal is simply to demonstrate how a feedback system could persist user comments.
* In a real deployment the train schedule would come from a database or web service.  The course labs (`Practical 7` and `Lab 10/11`) describe how to connect a WinForms application to a database and implement secure storage.

## HCI Considerations

The design of this prototype intentionally follows the guidelines provided in the HCI lab materials.  The kiosk is intended for **non‑technical users** and users with **perceptual difficulties**.  The interface is uncluttered and uses a simple linear flow, minimising the cognitive load required to operate it.  Font sizes can be adjusted at runtime, and a text‑to‑speech facility reads selected train details aloud, aligning with the accessibility recommendations for a railway kiosk.  A feedback form encourages user input and helps identify usability issues, fulfilling the requirement for a feedback mechanism.
