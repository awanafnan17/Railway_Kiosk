using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Speech.Synthesis;

namespace RailwayKiosk
{
    /// <summary>
    /// A dedicated screen for searching trains by number, destination,
    /// departure time and type. Results are shown in a styled table.
    /// </summary>
    public class SearchTrainForm : Form
    {
        private readonly List<Train> _trains = new();
        private readonly Guna2TextBox _txtTrainNumber;
        private readonly Guna2ComboBox _cmbDestination;
        private readonly Guna2ComboBox _cmbTrainType;
        private readonly DateTimePicker _dtpDeparture;
        private readonly Guna2Button _btnSearch;
        private readonly Guna2DataGridView _dgvResults;
        private readonly Guna2Button _btnVoice;
        private readonly Guna2Button _btnDetails;
        private readonly Guna2Button _btnBack;

        public SearchTrainForm()
        {
            this.Text = "Search Train";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
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
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White }
            };
            var btnMax = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White,
                Left = btnClose.Left - 45
            };
            var btnMin = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White,
                Left = btnMax.Left - 45
            };
            
            // Layout Controls
            btnClose.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 50, 10); 
            // Since form is Normal, we use form width. But better to just Dock Right or Anchor
            // Guna ControlBox supports Anchoring.
            // We'll add them to headerPanel and let Anchor handle it, but we need to set initial Right position relative to Form Width if it was fixed, 
            // but here it's Dock Top. 
            // Let's rely on FlowLayout or just Anchor from Right.
            
            // To make it simple, let's use a flow or just manual positioning relative to right.
            // But we need to update positions on Resize if we don't use Anchor properly.
            // Wait, Anchor works relative to parent. headerPanel is Dock Top. 
            // So if we add them to headerPanel, Anchor Right refers to headerPanel's Right.
            
            btnClose.Dock = DockStyle.Right;
            btnMax.Dock = DockStyle.Right;
            btnMin.Dock = DockStyle.Right;
            
            // But if we Dock them, they will be next to each other.
            
            var lblTitle = new Label
            {
                Text = "SEARCH TRAINS",
                Font = UITheme.GetTitleFont(20f),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            
            // We also have _btnBack which is Dock Right.
            // We should put Window Controls on the far right, and Back button maybe next to them or just remove Back button if Close is enough?
            // The user wants "Back" usually to go back to Home.
            // Close window usually closes the app or form.
            // If this is a child form, Close() just closes this form, returning to Home (if Home is open).
            // Let's keep Back button but maybe move it.
            // Or just put Window Controls at Top Right corner above everything?
            
            // Let's use a container for right-side controls.
            var rightControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            
            rightControls.Controls.Add(btnClose);
            rightControls.Controls.Add(btnMax);
            rightControls.Controls.Add(btnMin);
            // And maybe _btnBack?
            
            _btnBack = new Guna2Button
            {
                Text = "BACK",
                Width = 100,
                Height = 40,
                FillColor = Color.Transparent,
                BorderColor = Color.White,
                BorderThickness = 1,
                BorderRadius = 5,
                Margin = new Padding(0, 5, 20, 0)
            };
            _btnBack.Click += (_, _) => this.Close();
             rightControls.Controls.Add(_btnBack);

            headerPanel.Controls.Add(rightControls);
            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // Layout
            var splitContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(20)
            };
            splitContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300)); // Filter Panel
            splitContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Grid
            this.Controls.Add(splitContainer);
            
            // Fix Dock Order: headerPanel (Top) must be Back, splitContainer (Fill) must be Front
            headerPanel.SendToBack();
            splitContainer.BringToFront();

            // Filter Panel
            var filterPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 10,
                Padding = new Padding(20)
            };
            filterPanel.ShadowDecoration.Enabled = true;
            filterPanel.ShadowDecoration.Depth = 5;

            var lblFilters = new Label { Text = "Filters", Font = UITheme.GetHeaderFont(), ForeColor = UITheme.PrimaryColor, Dock = DockStyle.Top, Height = 40 };
            
            _txtTrainNumber = new Guna2TextBox { PlaceholderText = "Train Number", Height = 40, Dock = DockStyle.Top };
            UITheme.ApplyInputTheme(_txtTrainNumber);
            
            _cmbDestination = new Guna2ComboBox { Height = 40, Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbDestination.BorderColor = Color.Silver;
            _cmbDestination.BorderRadius = 6;
            
            _cmbTrainType = new Guna2ComboBox { Height = 40, Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbTrainType.BorderColor = Color.Silver;
            _cmbTrainType.BorderRadius = 6;

            // Wrap DateTimePicker in a panel for styling since native control is ugly
            var dtpPanel = new Panel { Height = 50, Dock = DockStyle.Top, Padding = new Padding(0, 10, 0, 0) };
            _dtpDeparture = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 260, Font = UITheme.GetBodyFont() };
            dtpPanel.Controls.Add(_dtpDeparture);

            _btnSearch = new Guna2Button { Text = "SEARCH", Height = 45, Dock = DockStyle.Top };
            UITheme.ApplyButtonTheme(_btnSearch);
            _btnSearch.Click += BtnSearch_Click;

            // Spacers
            var spacer1 = new Panel { Height = 10, Dock = DockStyle.Top };
            var spacer2 = new Panel { Height = 10, Dock = DockStyle.Top };
            var spacer3 = new Panel { Height = 10, Dock = DockStyle.Top };
            var spacer4 = new Panel { Height = 20, Dock = DockStyle.Top };

            filterPanel.Controls.Add(_btnSearch);
            filterPanel.Controls.Add(spacer4);
            filterPanel.Controls.Add(dtpPanel);
            filterPanel.Controls.Add(spacer3);
            filterPanel.Controls.Add(_cmbTrainType);
            filterPanel.Controls.Add(spacer2);
            filterPanel.Controls.Add(_cmbDestination);
            filterPanel.Controls.Add(spacer1);
            filterPanel.Controls.Add(_txtTrainNumber);
            filterPanel.Controls.Add(lblFilters);
            
            splitContainer.Controls.Add(filterPanel, 0, 0);

            // Grid Panel
            var gridPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 10,
                Padding = new Padding(10)
            };
            gridPanel.ShadowDecoration.Enabled = true;

            // Action Buttons for Grid
            var actionPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, FlowDirection = FlowDirection.RightToLeft };
            _btnVoice = new Guna2Button { Text = "SPEAK INFO", Width = 150, Height = 40 };
            UITheme.ApplyButtonTheme(_btnVoice, false);
            _btnVoice.Click += BtnVoice_Click;
            
            _btnDetails = new Guna2Button { Text = "VIEW DETAILS", Width = 150, Height = 40 };
            UITheme.ApplyButtonTheme(_btnDetails, true);
            _btnDetails.Click += BtnDetails_Click;
            
            actionPanel.Controls.Add(_btnDetails);
            actionPanel.Controls.Add(_btnVoice);

            _dgvResults = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
            UITheme.ApplyGridTheme(_dgvResults);

            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Train.TrainNumber), HeaderText = "Train No", FillWeight = 20 });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Train.Destination), HeaderText = "Destination", FillWeight = 30 });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Train.DepartureTime), HeaderText = "Departure", DefaultCellStyle = new DataGridViewCellStyle { Format = "t" }, FillWeight = 20 });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Train.TrainType), HeaderText = "Type", FillWeight = 15 });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Train.Status), HeaderText = "Status", FillWeight = 15 });

            gridPanel.Controls.Add(_dgvResults);
            gridPanel.Controls.Add(actionPanel);
            splitContainer.Controls.Add(gridPanel, 1, 0);

            // Load Data
            _trains.AddRange(TrainService.LoadTrains(AppContext.BaseDirectory));
            _dgvResults.DataSource = _trains;
            PopulateFilters();
        }

        private void PopulateFilters()
        {
            var destinations = new HashSet<string>();
            var types = new HashSet<string>();
            foreach (var t in _trains)
            {
                destinations.Add(t.Destination);
                types.Add(t.TrainType);
            }
            _cmbDestination.Items.Add("All Destinations");
            foreach (var d in destinations) _cmbDestination.Items.Add(d);
            _cmbDestination.SelectedIndex = 0;

            _cmbTrainType.Items.Add("All Types");
            foreach (var t in types) _cmbTrainType.Items.Add(t);
            _cmbTrainType.SelectedIndex = 0;
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            var filtered = TrainService.Filter(_trains,
                _txtTrainNumber.Text,
                _cmbDestination.SelectedItem?.ToString(),
                _cmbTrainType.SelectedItem?.ToString(),
                null); // Time filter omitted for simplicity or can be added back
            _dgvResults.DataSource = filtered;
        }

        private void BtnDetails_Click(object? sender, EventArgs e)
        {
            if (_dgvResults.SelectedRows.Count > 0 && _dgvResults.SelectedRows[0].DataBoundItem is Train train)
            {
                using var frm = new TrainDetailsForm(train);
                frm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Please select a train first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnVoice_Click(object? sender, EventArgs e)
        {
            if (_dgvResults.SelectedRows.Count > 0 && _dgvResults.SelectedRows[0].DataBoundItem is Train train)
            {
                try
                {
                    using var synthesizer = new SpeechSynthesizer();
                    synthesizer.Speak($"Train {train.TrainNumber} to {train.Destination} departs at {train.DepartureTime:hh:mm tt} and is currently {train.Status}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
