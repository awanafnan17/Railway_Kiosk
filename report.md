# Status Query System for a Railway Station Kiosk

## Understanding the Assignment

The Human–Computer Interaction (HCI) lab exam asks students to **design the front end** of an information system for users who are *not computer literate* and may have *perceptual difficulties*.  You do **not** have to build a complete backend; the focus is on the user interface, interactivity and accessibility.  Several example systems are provided in the brief, one of which is a **status‑query system for a kiosk at a railway station**.  The key points of this example are:

* The kiosk acts as a **self‑service information point** where passengers can quickly access information.
* The interface should be **intuitive and easy to navigate**, enabling searches for trains or filters based on destination, departure time or train type【852756744971947†L80-L87】.
* A **feedback option** should be available so users can comment on the accuracy of information or suggest improvements【852756744971947†L89-L92】.
* Accessibility features such as **large text options**, **voice instructions** and support for users with disabilities must be incorporated【852756744971947†L91-L94】.

The lab manuals supplied in the ZIP archive cover the practical skills required to implement such a system.  They introduce Visual Studio and C# WinForms (Practical 1), basic controls like labels, buttons and text boxes (Practical 2), picture boxes and radio buttons (Practical 3), list and combo boxes (Practical 4), menu strips and dialog boxes (Practical 5), multiple forms (Practical 6), database connectivity (Practical 7), multimedia integration (Lab 8), text‑to‑speech (Lab 9), secure login and password hashing (Lab 10), and encryption (Lab 11).

## Requirements Derived from the Materials

Based on the brief and the lab notes, a railway station kiosk should provide the following functionality:

1. **Search and Filter:** Passengers must be able to search for a train by number and filter results based on destination, train type or departure time.  This requirement comes directly from the example in the brief【852756744971947†L85-L87】.
2. **Accessible Design:** The interface needs to be easy to use for non‑technical and visually impaired users.  At minimum this means large, high‑contrast text, adjustable font sizes and the ability to read information aloud using text‑to‑speech【852756744971947†L91-L94】.  Lab 9 demonstrates how to add text‑to‑speech support using the SAPI library【511076950892768†L19-L35】.  Lab 8 shows how to add audio and video components for richer instructions【634389264135875†L13-L54】.
3. **Feedback Mechanism:** Users should have the option to leave feedback on the kiosk’s usability or the accuracy of information【852756744971947†L89-L92】.
4. **Administrative Access (Optional):** While not explicitly required for passengers, Practical 6 and Lab 10/11 show how to implement multi‑form interfaces and secure login with password hashing【919523187036429†L16-L61】.  An admin login can restrict access to maintenance functions.
5. **Data Source:** Practical 7 demonstrates connecting a form to a database.  For the purpose of this front‑end prototype we can instead load train information from a file; the backend implementation can later be replaced with a live data source.

## How the Provided Prototype Meets the Requirements

The `railway_kiosk` project supplied alongside this report implements a **WinForms** prototype of the railway station kiosk:

* **Modern UI:** The project uses the **Guna2** control library to create a polished interface.  The main window contains search fields (train number, destination, train type and a departure time picker) and a data grid to display matching trains.
* **Data Loading:** Sample train data is stored in `TrainData/trains.json` and is loaded at runtime.  The `MainForm` class uses `System.Text.Json` to deserialize this file and populate the grid.
* **Search & Filter:** Clicking the *Search* button applies the specified filters.  The departure time filter matches trains within ±60 minutes of the selected time.
* **Accessibility Features:**
  * **Font scaling:** `A+` and `A−` buttons adjust the base font size across the entire interface.
  * **High contrast:** Panels and controls use neutral colours with dark text to maximise legibility.
  * **Text‑to‑speech:** A *Speak Selected* button invokes the SAPI voice engine to read the selected train’s details aloud, following the procedure in Lab 9【511076950892768†L19-L35】.
* **Feedback Form:** A *Feedback* button opens a modal dialog where users can type comments.  Submissions are saved to timestamped text files under the `Feedback` folder so they can be reviewed by administrators.
* **Login Form (Optional):** The application starts at a login screen.  Users can log in with the default admin credentials or continue as guests.  Passwords are salted and hashed using PBKDF2, as illustrated in Lab 10【919523187036429†L16-L61】.  This demonstrates how secure authentication can be added when the kiosk needs to protect administrative functions.

## Next Steps

This prototype delivers the key elements requested in the assignment and showcases how the practical lab skills come together to build a usable application.  To turn this into a production system you would:

* Replace the JSON file with a call to a live database or web service (see Practical 7 for database integration).
* Implement error handling and data validation more robustly.
* Localise the interface and text‑to‑speech output for the languages spoken in the target region.
* Conduct user testing with representative passengers to refine the layout and interactions.

By following the HCI principles emphasised in the lab materials—simplicity, discoverability, feedback and accessibility—this kiosk can provide a welcoming and effective self‑service experience for railway passengers.