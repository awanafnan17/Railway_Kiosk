using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Speech.Synthesis;

namespace RailwayKiosk
{
    public class TrainDetailsForm : Form
    {
        private readonly Train _train;
        private readonly Guna2Button _btnBack;
        private readonly Guna2Button _btnSpeak;

        public TrainDetailsForm(Train train)
        {
            _train = train;
            this.Text = "Train Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            this.Size = new Size(1000, 700);
            this.BackColor = UITheme.BackgroundColor;
            
            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            // Header
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FillColor = UITheme.PrimaryColor,
                Padding = new Padding(20)
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
            
            // Layout Header
            // We use a container for right-side controls (Back + Window Controls)
            var rightPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            
            rightPanel.Controls.Add(btnClose);
            rightPanel.Controls.Add(btnMax);
            rightPanel.Controls.Add(btnMin);
            
            var lblTitle = new Label
            {
                Text = "TRAIN DETAILS",
                Font = UITheme.GetTitleFont(20f),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            
            _btnBack = new Guna2Button
            {
                Text = "CLOSE",
                Width = 100,
                Height = 40,
                FillColor = Color.Transparent,
                BorderColor = Color.White,
                BorderThickness = 1,
                BorderRadius = 5,
                Margin = new Padding(0, 5, 20, 0)
            };
            _btnBack.Click += (_, _) => this.Close();
             rightPanel.Controls.Add(_btnBack);

            headerPanel.Controls.Add(rightPanel);
            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // Card Panel Layout
            // Use TableLayoutPanel to center the card robustly
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                BackColor = Color.Transparent
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            this.Controls.Add(mainLayout);
            // headerPanel (Dock=Top) should be Back (processed first)
            // mainLayout (Dock=Fill) should be Front (processed last)
            headerPanel.SendToBack();
            mainLayout.BringToFront();

            // Card Panel
            var cardPanel = new Guna2Panel
            {
                Width = 600,
                Height = 500,
                FillColor = Color.White,
                BorderRadius = 20
            };
            cardPanel.ShadowDecoration.Enabled = true;
            cardPanel.ShadowDecoration.Depth = 10;
            
            // Add card to center cell
            mainLayout.Controls.Add(cardPanel, 1, 1);
            
            // Content inside Card
            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40),
                WrapContents = false
            };
            cardPanel.Controls.Add(layout);

            layout.Controls.Add(CreateDetailLabel("TRAIN NUMBER", _train.TrainNumber));
            layout.Controls.Add(CreateDetailLabel("DESTINATION", _train.Destination));
            layout.Controls.Add(CreateDetailLabel("DEPARTURE TIME", _train.DepartureTime.ToString("hh:mm tt")));
            layout.Controls.Add(CreateDetailLabel("TRAIN TYPE", _train.TrainType));
            
            // Status with color
            var lblStatusHeader = new Label { Text = "STATUS", Font = UITheme.GetBodyFont(10f), ForeColor = Color.Gray, AutoSize = true, Margin = new Padding(0, 10, 0, 0) };
            var lblStatusValue = new Label { Text = _train.Status, Font = UITheme.GetHeaderFont(24f), AutoSize = true };
            lblStatusValue.ForeColor = _train.Status.ToLowerInvariant() switch
            {
                "on time" => UITheme.SuccessColor,
                "delayed" => UITheme.SecondaryColor,
                "cancelled" => UITheme.ErrorColor,
                _ => UITheme.TextColor
            };
            layout.Controls.Add(lblStatusHeader);
            layout.Controls.Add(lblStatusValue);

            // Speak Button
            _btnSpeak = new Guna2Button
            {
                Text = "🔊 READ ALOUD",
                Height = 50,
                Width = 200,
                Margin = new Padding(0, 40, 0, 0)
            };
            UITheme.ApplyButtonTheme(_btnSpeak);
            _btnSpeak.Click += (_, _) => SpeakDetails();
            layout.Controls.Add(_btnSpeak);
        }

        private Control CreateDetailLabel(string label, string value)
        {
            var panel = new Panel { AutoSize = true, Margin = new Padding(0, 0, 0, 15) };
            var l = new Label { Text = label, Font = UITheme.GetBodyFont(10f), ForeColor = Color.Gray, Dock = DockStyle.Top, AutoSize = true };
            var v = new Label { Text = value, Font = UITheme.GetHeaderFont(16f), ForeColor = UITheme.TextColor, Dock = DockStyle.Top, AutoSize = true };
            panel.Controls.Add(v);
            panel.Controls.Add(l);
            return panel;
        }

        private void SpeakDetails()
        {
            try
            {
                using var synthesizer = new SpeechSynthesizer();
                synthesizer.Speak($"Train {_train.TrainNumber} to {_train.Destination}. Departs at {_train.DepartureTime:hh:mm tt}. Status: {_train.Status}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
