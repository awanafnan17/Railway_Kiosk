using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    public class TrainEditForm : Form
    {
        public Train Train { get; private set; }
        
        private Guna2TextBox _txtNumber;
        private Guna2TextBox _txtDestination;
        private Guna2DateTimePicker _dtpTime;
        private Guna2ComboBox _cmbType;
        private Guna2ComboBox _cmbStatus;
        private Guna2Button _btnSave;
        private Guna2Button _btnCancel;

        public TrainEditForm(Train train = null)
        {
            this.Text = train == null ? "Add Train" : "Edit Train";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = UITheme.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            if (train == null)
            {
                Train = new Train { DepartureTime = DateTime.Now };
            }
            else
            {
                Train = train;
            }

            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                WrapContents = false
            };
            this.Controls.Add(panel);

            panel.Controls.Add(CreateLabel("Train Number"));
            _txtNumber = CreateTextBox();
            panel.Controls.Add(_txtNumber);

            panel.Controls.Add(CreateLabel("Destination"));
            _txtDestination = CreateTextBox();
            panel.Controls.Add(_txtDestination);

            panel.Controls.Add(CreateLabel("Departure Time"));
            _dtpTime = new Guna2DateTimePicker
            {
                Width = 340,
                Height = 36,
                CustomFormat = "yyyy-MM-dd HH:mm",
                Format = DateTimePickerFormat.Custom
            };
            panel.Controls.Add(_dtpTime);

            panel.Controls.Add(CreateLabel("Train Type"));
            _cmbType = new Guna2ComboBox { Width = 340, Height = 36 };
            _cmbType.Items.AddRange(new object[] { "High Speed", "InterCity", "Express", "Local" });
            panel.Controls.Add(_cmbType);

            panel.Controls.Add(CreateLabel("Status"));
            _cmbStatus = new Guna2ComboBox { Width = 340, Height = 36 };
            _cmbStatus.Items.AddRange(new object[] { "On Time", "Delayed", "Cancelled", "Boarding" });
            panel.Controls.Add(_cmbStatus);

            var btnPanel = new FlowLayoutPanel
            {
                Width = 340,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0, 20, 0, 0)
            };

            _btnSave = new Guna2Button { Text = "Save", FillColor = UITheme.SuccessColor, AutoSize = true, Margin = new Padding(10, 0, 0, 0) };
            _btnSave.Click += BtnSave_Click;

            _btnCancel = new Guna2Button { Text = "Cancel", FillColor = Color.Gray, AutoSize = true };
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            btnPanel.Controls.Add(_btnSave);
            btnPanel.Controls.Add(_btnCancel);
            panel.Controls.Add(btnPanel);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = UITheme.GetBodyFont(10f),
                ForeColor = UITheme.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            };
        }

        private Guna2TextBox CreateTextBox()
        {
            var txt = new Guna2TextBox { Width = 340, Height = 36 };
            UITheme.ApplyInputTheme(txt);
            return txt;
        }

        private void LoadData()
        {
            _txtNumber.Text = Train.TrainNumber;
            _txtDestination.Text = Train.Destination;
            _dtpTime.Value = Train.DepartureTime;
            _cmbType.SelectedItem = Train.TrainType;
            _cmbStatus.SelectedItem = Train.Status;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtNumber.Text) || string.IsNullOrWhiteSpace(_txtDestination.Text))
            {
                MessageBox.Show("Number and Destination are required.");
                return;
            }

            Train.TrainNumber = _txtNumber.Text;
            Train.Destination = _txtDestination.Text;
            Train.DepartureTime = _dtpTime.Value;
            Train.TrainType = _cmbType.SelectedItem?.ToString() ?? "InterCity";
            Train.Status = _cmbStatus.SelectedItem?.ToString() ?? "On Time";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
