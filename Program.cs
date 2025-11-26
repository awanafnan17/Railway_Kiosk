using System;
using System.IO;
using System.Windows.Forms;
using Serilog;

namespace RailwayKiosk
{
    /// <summary>
    /// Entry point for the Railway Station Kiosk application.  This program
    /// configures the application to use Windows Forms and launches the
    /// login form.  The login form optionally authenticates an admin user
    /// before presenting the kiosk interface.  Guests can skip login and
    /// access the kiosk directly.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try 
            {
                // Enable high DPI support and modern WinForms styling
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (s, e) =>
                    MessageBox.Show(e.Exception.ToString(), "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                    MessageBox.Show(e.ExceptionObject?.ToString() ?? "Unknown error", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Use local directory for logging to avoid permission issues during debug
                var logsDir = Path.Combine(AppContext.BaseDirectory, "Logs");
                Directory.CreateDirectory(logsDir);
                Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                    .WriteTo.File(Path.Combine(logsDir, "app.log"), rollingInterval: Serilog.RollingInterval.Day)
                    .CreateLogger();
                Serilog.Log.Information("Application starting");

                // Launch the login screen; the login form is responsible for
                // switching to the main form once credentials are validated or
                // the user continues as a guest.  Use using to ensure proper
                // disposal of forms on close.
                using var loginForm = new LoginForm();
                Serilog.Log.Information("LoginForm instantiated");
                Application.Run(loginForm);
                Serilog.Log.Information("Application exiting");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical Startup Error: {ex}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Also try to write to a file in the same dir if possible
                try { File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "startup_error.txt"), ex.ToString()); } catch { }
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }
    }
}
