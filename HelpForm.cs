using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Speech.Synthesis;

namespace RailwayKiosk
{
    /// <summary>
    /// Provides guidance on how to use the kiosk and exposes simple
    /// accessibility options such as adjusting the font size and toggling
    /// high‑contrast mode.
    /// </summary>
    public class HelpForm : Form
    {
        private Guna2BorderlessForm _borderlessForm;
        private Guna2ShadowForm _shadowForm;
        private System.ComponentModel.IContainer components;
        
        private float _fontSize = 14f;
        private Label _lblInstructions;
        private Guna2Button _btnIncrease;
        private Guna2Button _btnDecrease;
        private Guna2ToggleSwitch _toggleContrast;
        private Guna2Button _btnBack;
        private Guna2Button _btnPlay;
        private Guna2Panel _contentPanel;
        private Label _lblTitle;
        private Label _lblContrast;
        
        // Use class-level synthesizer to avoid disposal before speech completes
        private SpeechSynthesizer _synthesizer;

        public HelpForm()
        {
            components = new System.ComponentModel.Container();
            _synthesizer = new SpeechSynthesizer();
            
            this.FormBorderStyle = FormBorderStyle.None;
            _borderlessForm = new Guna2BorderlessForm(components);
            _shadowForm = new Guna2ShadowForm(components);
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            this.BackColor = UITheme.BackgroundColor;
            this.Size = new Size(1000, 700);
            this.Padding = new Padding(30);

            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Header with Window Controls
             var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FillColor = Color.Transparent
            };
            
            // Window Controls
            var btnClose = new Guna2ControlBox
            {
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = UITheme.TextColor,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White }
            };
            var btnMax = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = UITheme.TextColor
            };
            var btnMin = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = UITheme.TextColor
            };
            
            headerPanel.Controls.Add(btnClose);
            headerPanel.Controls.Add(btnMax);
            headerPanel.Controls.Add(btnMin);
            
            // Title and Back Button in Header
            _lblTitle = new Label
            {
                Text = "Help & Accessibility",
                Font = UITheme.GetTitleFont(),
                ForeColor = UITheme.PrimaryColor,
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(10, 15, 0, 0)
            };
            headerPanel.Controls.Add(_lblTitle);
            
            // Back Button (next to window controls)
            _btnBack = new Guna2Button 
            { 
                Text = "Back", 
                Width = 80, 
                Height = 30,
                Dock = DockStyle.Right,
                Margin = new Padding(0, 15, 10, 0) 
            };
            UITheme.ApplyButtonTheme(_btnBack, false);
            _btnBack.FillColor = Color.Transparent;
            _btnBack.BorderColor = UITheme.PrimaryColor;
            _btnBack.BorderThickness = 1;
            _btnBack.ForeColor = UITheme.PrimaryColor;
            _btnBack.Click += (_, _) => this.Close();
            
            // Add _btnBack to headerPanel. Since it's Dock=Right and added AFTER window controls,
            // it will appear to the LEFT of the window controls.
            headerPanel.Controls.Add(_btnBack);

            this.Controls.Add(headerPanel);

            // Main Layout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            // Row 0: Content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); 
            // Row 1: Controls (Bottom)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); 
            this.Controls.Add(mainLayout);
            
            // Fix Dock Order
            headerPanel.SendToBack();
            mainLayout.BringToFront();

            // 2. Instructions Content (Card Style)
            _contentPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = UITheme.SurfaceColor,
                BorderRadius = 15,
                Margin = new Padding(0, 10, 0, 10),
                AutoScroll = true // Enable scrolling for large text
            };
            _contentPanel.ShadowDecoration.Enabled = true;
            _contentPanel.ShadowDecoration.Depth = 10;

            _lblInstructions = new Label
            {
                Dock = DockStyle.Top, // Changed from Fill to Top to allow scrolling
                AutoSize = true,      // Allow label to grow with text
                Padding = new Padding(40),
                Font = new Font("Segoe UI", _fontSize),
                ForeColor = UITheme.TextColor,
                Text = GetInstructionsText(),
                TextAlign = ContentAlignment.TopLeft,
                MaximumSize = new Size(this.Width - 100, 0) // Constrain width so text wraps
            };
            _contentPanel.Controls.Add(_lblInstructions);
            mainLayout.Controls.Add(_contentPanel, 0, 0);

            // 3. Accessibility Controls
            var controlsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            _btnIncrease = new Guna2Button { Text = "A+", Width = 60, Height = 45, Margin = new Padding(10) };
            UITheme.ApplyButtonTheme(_btnIncrease);
            _btnIncrease.Click += (_, _) => ChangeFontSize(2);

            _btnDecrease = new Guna2Button { Text = "A-", Width = 60, Height = 45, Margin = new Padding(10) };
            UITheme.ApplyButtonTheme(_btnDecrease);
            _btnDecrease.Click += (_, _) => ChangeFontSize(-2);

            _btnPlay = new Guna2Button { Text = "Read Aloud", Width = 140, Height = 45, Margin = new Padding(10) };
            UITheme.ApplyButtonTheme(_btnPlay);
            _btnPlay.Click += ReadInstructions;

            // Contrast Toggle
            var contrastPanel = new Panel { AutoSize = true, Margin = new Padding(20, 10, 10, 10) };
            _toggleContrast = new Guna2ToggleSwitch { Checked = false };
            _toggleContrast.CheckedState.FillColor = UITheme.SecondaryColor;
            
            _lblContrast = new Label 
            { 
                Text = "High Contrast", 
                Font = UITheme.GetBodyFont(), 
                ForeColor = UITheme.TextColor,
                AutoSize = true,
                Location = new Point(50, 2)
            };
            _toggleContrast.CheckedChanged += (_, _) => ToggleContrast(_toggleContrast.Checked);
            
            contrastPanel.Controls.Add(_toggleContrast);
            contrastPanel.Controls.Add(_lblContrast);

            controlsPanel.Controls.Add(_btnDecrease);
            controlsPanel.Controls.Add(_btnIncrease);
            controlsPanel.Controls.Add(_btnPlay);
            controlsPanel.Controls.Add(contrastPanel);

            mainLayout.Controls.Add(controlsPanel, 0, 2);
            
            // Layout Logic
            this.Resize += (s, e) => {
                 _btnBack.Left = headerPanel.Width - 100;
            };
        }

        private string GetInstructionsText()
        {
            return "Welcome to Railway Kiosk Help\n\n" +
                   "• Search Train: Tap 'Search Train' to find schedules by station or train number.\n\n" +
                   "• Train Status: Tap 'Train Status' to check real-time arrival and departure info.\n\n" +
                   "• Accessibility: Use the controls below to adjust text size or enable high contrast.\n\n" +
                   "• Audio Guide: Tap 'Read Aloud' to hear these instructions.\n\n" +
                   "• Feedback: We value your input. Use the Feedback form to share your thoughts.";
        }

        private void ChangeFontSize(int delta)
        {
            _fontSize = Math.Max(10f, Math.Min(32f, _fontSize + delta));
            _lblInstructions.Font = new Font("Segoe UI", _fontSize);
        }

        private void ReadInstructions(object? sender, EventArgs e)
        {
             try
            {
                if (_synthesizer.State == SynthesizerState.Speaking)
                {
                    _synthesizer.SpeakAsyncCancelAll();
                }
                else
                {
                    _synthesizer.SpeakAsync(_lblInstructions.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message);
            }
        }

        private void ToggleContrast(bool highContrast)
        {
            if (highContrast)
            {
                // High Contrast Mode (Yellow on Black)
                _contentPanel.FillColor = Color.Black;
                _lblInstructions.ForeColor = Color.Yellow;
                this.BackColor = Color.FromArgb(20, 20, 20);
                
                // Update text colors
                _lblTitle.ForeColor = Color.Yellow;
                _lblContrast.ForeColor = Color.Yellow;
            }
            else
            {
                // Normal Mode
                _contentPanel.FillColor = UITheme.SurfaceColor;
                _lblInstructions.ForeColor = UITheme.TextColor;
                this.BackColor = UITheme.BackgroundColor;
                
                // Reset text colors
                _lblTitle.ForeColor = UITheme.PrimaryColor;
                _lblContrast.ForeColor = UITheme.TextColor;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
