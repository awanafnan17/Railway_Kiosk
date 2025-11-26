using System;
using System.Drawing;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    /// <summary>
    /// The modern welcome screen presents high-level navigation options via
    /// a dashboard-style interface.
    /// </summary>
    public class HomeForm : Form
    {
        private System.ComponentModel.IContainer components;
        private readonly Label _lblClock;
        private readonly Timer _timer;
        private Guna2AnimateWindow _animateWindow;

        public HomeForm(string username = "Guest")
        {
            components = new System.ComponentModel.Container();
            _animateWindow = new Guna2AnimateWindow(components)
            {
                AnimationType = Guna.UI2.WinForms.Guna2AnimateWindow.AnimateWindowType.AW_BLEND,
                Interval = 500,
                TargetForm = this
            };

            // Initialize Timer for Clock
            _timer = new Timer(components);
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => {
                if (_lblClock != null && !_lblClock.IsDisposed)
                    _lblClock.Text = DateTime.Now.ToString("f");
            };
            _timer.Start();

            this.Text = "Railway Station Information Kiosk";
            this.StartPosition = FormStartPosition.CenterScreen;
            // Removed Maximized state
            this.Size = new Size(1280, 800);
            this.BackColor = UITheme.BackgroundColor;
            
            // Enable Double Buffering to reduce flicker and layout artifacts
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            // Header
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                FillColor = UITheme.PrimaryColor
            };

            // Window Controls
            var btnClose = new Guna2ControlBox
            {
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White }
            };
            var btnMax = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White
            };
            var btnMin = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White
            };

            // Window Controls Container
            var controlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 10, 0)
            };
            controlsPanel.Controls.Add(btnClose);
            controlsPanel.Controls.Add(btnMax);
            controlsPanel.Controls.Add(btnMin);
            headerPanel.Controls.Add(controlsPanel);

            var lblTitle = new Label
            {
                Text = "RAILWAY KIOSK",
                Font = UITheme.GetTitleFont(24f),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 25),
                BackColor = Color.Transparent
            };

            _lblClock = new Label
            {
                Text = DateTime.Now.ToString("f"),
                Font = UITheme.GetHeaderFont(12f),
                ForeColor = Color.LightGray,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            _lblClock.Location = new Point(30, 70);

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(_lblClock);
            this.Controls.Add(headerPanel);

            // Dashboard Grid
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(50),
                BackColor = UITheme.BackgroundColor
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            table.Controls.Add(CreateTile("SEARCH TRAINS", "Find your train by number or destination", Color.FromArgb(52, 152, 219), () => OpenForm(new SearchTrainForm())), 0, 0);
            table.Controls.Add(CreateTile("CHECK STATUS", "View real-time arrivals and departures", Color.FromArgb(46, 204, 113), () => OpenForm(new TrainStatusForm())), 1, 0);
            table.Controls.Add(CreateTile("HELP & ACCESSIBILITY", "Get assistance or change settings", Color.FromArgb(155, 89, 182), () => OpenForm(new HelpForm())), 0, 1);
            table.Controls.Add(CreateTile("FEEDBACK", "Rate your experience with us", Color.FromArgb(230, 126, 34), () => OpenForm(new FeedbackForm())), 1, 1);

            // Admin Logic
            if (UserService.IsAdmin(username))
            {
                table.RowCount = 3;
                table.RowStyles.Clear();
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 33f));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 33f));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 33f));
                
                // Add Admin Tile spanning 2 columns
                var adminTile = CreateTile("ADMIN DASHBOARD", "Manage users, trains, and system settings", Color.Crimson, () => OpenForm(new AdminDashboardForm()));
                table.Controls.Add(adminTile, 0, 2);
                table.SetColumnSpan(adminTile, 2);
            }

            this.Controls.Add(table);
            // headerPanel should be at the Back (processed first for Dock=Top)
            // table should be at the Front (processed last for Dock=Fill)
            headerPanel.SendToBack();
            table.BringToFront();

            // Voice FAB (Floating Action Button)
            var btnVoice = new Guna2CircleButton
            {
                Size = new Size(60, 60),
                FillColor = UITheme.SecondaryColor,
                Text = "🎤",
                Font = new Font("Segoe UI Emoji", 20f),
                ForeColor = Color.White,
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle, Depth = 10, Enabled = true }
            };
            // Position relative to form
            this.Controls.Add(btnVoice);
            btnVoice.BringToFront();
            
            // Handle resizing for FAB
            this.Resize += (s, e) => {
                 btnVoice.Location = new Point(this.ClientSize.Width - 80, this.ClientSize.Height - 80);
            };
        }

        private Control CreateTile(string title, string subtitle, Color color, Action onClick)
        {
            var panel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(20),
                FillColor = Color.White,
                BorderRadius = 15,
                Cursor = Cursors.Hand
            };
            panel.ShadowDecoration.Enabled = true;
            panel.ShadowDecoration.Depth = 5;
            panel.ShadowDecoration.Color = Color.Gray;

            var lblTitle = new Label
            {
                Text = title,
                Font = UITheme.GetHeaderFont(18f),
                ForeColor = color,
                AutoSize = true,
                Location = new Point(30, 30),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = UITheme.GetBodyFont(12f),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(30, 70),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            // Event Handlers for Interaction
            void OnClick(object s, EventArgs e) => onClick();

            // Hover Logic with Bounds Checking to prevent flicker
            EventHandler enter = (s, e) => panel.FillColor = Color.FromArgb(20, color);
            
            EventHandler leave = (s, e) => {
                // Check if mouse is still within the panel's bounds
                Point p = panel.PointToClient(Cursor.Position);
                if (!panel.ClientRectangle.Contains(p))
                {
                    panel.FillColor = Color.White;
                }
            };

            // Attach handlers
            panel.Click += OnClick;
            lblTitle.Click += OnClick;
            lblSub.Click += OnClick;

            panel.MouseEnter += enter;
            lblTitle.MouseEnter += enter;
            lblSub.MouseEnter += enter;

            panel.MouseLeave += leave;
            lblTitle.MouseLeave += leave;
            lblSub.MouseLeave += leave;

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblSub);
            
            return panel;
        }

        private void OpenForm(Form form)
        {
            using (form)
            {
                form.ShowDialog(this);
            }
        }

        private void BtnVoice_Click(object? sender, EventArgs e)
        {
            try
            {
                using var synthesizer = new SpeechSynthesizer();
                synthesizer.Speak("Welcome to the Railway Station Kiosk. Please select an option from the dashboard.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
