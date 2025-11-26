using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    /// <summary>
    /// Allows the user to enter a train number (or select from a drop‑down)
    /// and retrieve the current status of that train.
    /// </summary>
    public class TrainStatusForm : Form
    {
        private Guna2BorderlessForm _borderlessForm;
        private Guna2ShadowForm _shadowForm;
        private System.ComponentModel.IContainer components;

        private readonly List<Train> _trains = new();
        private Guna2ComboBox _cmbTrain;
        private Guna2Button _btnCheck;
        private Guna2Button _btnDetails;
        private Guna2Button _btnBack;
        
        private Guna2Panel _resultPanel;
        private Label _lblTrainName;
        private Label _lblArrival;
        private Label _lblDeparture;
        private Label _lblPlatform;
        private Label _lblStatus;
        
        private Train? _lastTrain;

        public TrainStatusForm()
        {
            components = new System.ComponentModel.Container();
            this.FormBorderStyle = FormBorderStyle.None;
            _borderlessForm = new Guna2BorderlessForm(components);
            _shadowForm = new Guna2ShadowForm(components);
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            this.BackColor = UITheme.BackgroundColor;
            this.Size = new Size(1000, 700);
            
            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            
            InitializeUI();
            LoadTrains();
        }

        private void InitializeUI()
        {
             // Header with Window Controls
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FillColor = UITheme.PrimaryColor,
                Padding = new Padding(10)
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
             
            var lblHeader = new Label
            {
                Text = "TRAIN STATUS",
                Font = UITheme.GetTitleFont(18f),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            headerPanel.Controls.Add(btnClose);
            headerPanel.Controls.Add(btnMax);
            headerPanel.Controls.Add(btnMin);
            headerPanel.Controls.Add(lblHeader);
            this.Controls.Add(headerPanel);

            // Main Layout: Split Screen
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(40)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Input Panel
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Result Panel
            this.Controls.Add(mainLayout);
            
            // headerPanel (Dock=Top) should be Back (processed first)
            // mainLayout (Dock=Fill) should be Front (processed last)
            headerPanel.SendToBack();
            mainLayout.BringToFront();

            // 1. Input Panel (Left)
            var inputPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = UITheme.SurfaceColor,
                BorderRadius = 20,
                Margin = new Padding(0, 0, 20, 0)
            };
            inputPanel.ShadowDecoration.Enabled = true;
            
            var inputLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(30, 40, 0, 0),
                WrapContents = false
            };
            inputPanel.Controls.Add(inputLayout);

            var lblTitle = new Label
            {
                Text = "Check Status",
                Font = UITheme.GetTitleFont(),
                ForeColor = UITheme.PrimaryColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };
            inputLayout.Controls.Add(lblTitle);
            
            var lblSelect = new Label
            {
                Text = "Select Train Number:",
                Font = UITheme.GetBodyFont(),
                ForeColor = UITheme.TextColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            inputLayout.Controls.Add(lblSelect);

            _cmbTrain = new Guna2ComboBox
            {
                Width = 300,
                Height = 45,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BorderRadius = 6,
                BorderColor = Color.Silver,
                Font = UITheme.GetBodyFont(),
                Margin = new Padding(0, 0, 0, 25)
            };
            inputLayout.Controls.Add(_cmbTrain);

            _btnCheck = new Guna2Button { Text = "Check Status", Width = 300, Height = 50, Margin = new Padding(0, 0, 0, 20) };
            UITheme.ApplyButtonTheme(_btnCheck, true);
            _btnCheck.Click += BtnCheck_Click;
            inputLayout.Controls.Add(_btnCheck);

            _btnBack = new Guna2Button { Text = "Back to Home", Width = 300, Height = 50 };
            UITheme.ApplyButtonTheme(_btnBack, false);
            _btnBack.FillColor = Color.Transparent;
            _btnBack.ForeColor = UITheme.TextSecondaryColor;
            _btnBack.BorderColor = UITheme.TextSecondaryColor;
            _btnBack.BorderThickness = 1;
            _btnBack.Click += (_, _) => this.Close();
            inputLayout.Controls.Add(_btnBack);

            mainLayout.Controls.Add(inputPanel, 0, 0);

            // 2. Result Panel (Right)
            _resultPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = UITheme.SurfaceColor,
                BorderRadius = 20,
                Margin = new Padding(20, 0, 0, 0),
                Visible = false // Hidden initially
            };
            _resultPanel.ShadowDecoration.Enabled = true;
            
            var resultLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40, 40, 0, 0),
                WrapContents = false
            };
            _resultPanel.Controls.Add(resultLayout);

            // Details inside Result Panel
            _lblTrainName = new Label
            {
                Text = "Train Name",
                Font = UITheme.GetHeaderFont(22f),
                ForeColor = UITheme.PrimaryColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };
            resultLayout.Controls.Add(_lblTrainName);

            _lblStatus = new Label
            {
                Text = "Status: On Time",
                Font = UITheme.GetHeaderFont(16f),
                ForeColor = UITheme.SuccessColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };
             resultLayout.Controls.Add(_lblStatus);

            _lblArrival = new Label { Font = UITheme.GetBodyFont(14f), ForeColor = UITheme.TextColor, AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
            resultLayout.Controls.Add(_lblArrival);

            _lblDeparture = new Label { Font = UITheme.GetBodyFont(14f), ForeColor = UITheme.TextColor, AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
            resultLayout.Controls.Add(_lblDeparture);

            _lblPlatform = new Label { Font = UITheme.GetBodyFont(14f), ForeColor = UITheme.TextColor, AutoSize = true, Margin = new Padding(0, 0, 0, 30) };
            resultLayout.Controls.Add(_lblPlatform);

            _btnDetails = new Guna2Button { Text = "View Full Details", Width = 250, Height = 45 };
            UITheme.ApplyButtonTheme(_btnDetails, false); // Secondary action
            _btnDetails.Click += BtnDetails_Click;
            resultLayout.Controls.Add(_btnDetails);

            mainLayout.Controls.Add(_resultPanel, 1, 0);
        }

        private void LoadTrains()
        {
             _trains.Clear();
            _trains.AddRange(TrainService.LoadTrains(AppContext.BaseDirectory));
            _cmbTrain.Items.Clear();
            foreach (var t in _trains)
            {
                _cmbTrain.Items.Add(t.TrainNumber);
            }
        }

        private void BtnCheck_Click(object? sender, EventArgs e)
        {
            var number = _cmbTrain.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(number))
            {
                MessageBox.Show("Please select a train number.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var train = _trains.FirstOrDefault(t => string.Equals(t.TrainNumber, number, StringComparison.OrdinalIgnoreCase));
            if (train == null) return;

            _lastTrain = train;
            
            // Update UI
            _lblTrainName.Text = $"Train {train.TrainNumber} – {train.Destination}";
            _lblArrival.Text = $"Scheduled Arrival: {train.DepartureTime.AddMinutes(-30):hh:mm tt}"; // Mock arrival
            _lblDeparture.Text = $"Scheduled Departure: {train.DepartureTime:hh:mm tt}";
            _lblPlatform.Text = "Platform: 04"; // Mock platform
            
            // Status Logic
            _lblStatus.Text = $"Status: {train.Status}";
            switch (train.Status.ToLowerInvariant())
            {
                case "on time":
                    _lblStatus.ForeColor = UITheme.SuccessColor;
                    break;
                case "delayed":
                    _lblStatus.ForeColor = UITheme.SecondaryColor;
                    break;
                case "cancelled":
                    _lblStatus.ForeColor = UITheme.ErrorColor;
                    break;
                default:
                    _lblStatus.ForeColor = UITheme.TextColor;
                    break;
            }

            _resultPanel.Visible = true;
        }

        private void BtnDetails_Click(object? sender, EventArgs e)
        {
            if (_lastTrain != null)
            {
                using var frm = new TrainDetailsForm(_lastTrain);
                frm.ShowDialog(this);
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
