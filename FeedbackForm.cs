using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    /// <summary>
    /// Presents a simple dialog for users to leave feedback about the
    /// kiosk application.
    /// </summary>
    public class FeedbackForm : Form
    {
        private Guna2BorderlessForm _borderlessForm;
        private Guna2ShadowForm _shadowForm;
        private System.ComponentModel.IContainer components;
        
        private Guna2TextBox _txtFeedback;
        private Guna2Button _btnSubmit;
        private Guna2ComboBox _cmbRating;
        private Guna2Button _btnClose;

        public FeedbackForm()
        {
            components = new System.ComponentModel.Container();
            this.FormBorderStyle = FormBorderStyle.None;
            _borderlessForm = new Guna2BorderlessForm(components);
            _shadowForm = new Guna2ShadowForm(components);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(500, 450);
            this.BackColor = UITheme.SurfaceColor;
            
            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeUI();
        }

        private void InitializeUI()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                Padding = new Padding(30),
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Header
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rating Label
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Rating Combo
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Feedback Text
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Submit Button
            
            this.Controls.Add(mainPanel);

            // 1. Header Panel (Title + Window Controls)
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };
            
            var lblTitle = new Label
            {
                Text = "We Value Your Feedback",
                Font = UITheme.GetHeaderFont(18f),
                ForeColor = UITheme.PrimaryColor,
                AutoSize = true,
                Location = new Point(0, 10)
            };
            headerPanel.Controls.Add(lblTitle);

            // Window Controls
            var btnClose = new Guna2ControlBox
            {
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = UITheme.TextSecondaryColor,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White }
            };
            var btnMin = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = UITheme.TextSecondaryColor
            };
            
            headerPanel.Controls.Add(btnClose);
            headerPanel.Controls.Add(btnMin);
            
            mainPanel.Controls.Add(headerPanel, 0, 0);

            // 2. Rating
            var lblRating = new Label
            {
                Text = "How was your experience?",
                Font = UITheme.GetBodyFont(),
                ForeColor = UITheme.TextColor,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 5)
            };
            mainPanel.Controls.Add(lblRating, 0, 1);

            _cmbRating = new Guna2ComboBox
            {
                Width = 440,
                Height = 40,
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BorderRadius = 6,
                BorderColor = Color.Silver,
                Font = UITheme.GetBodyFont(),
                Margin = new Padding(0, 0, 0, 15)
            };
            _cmbRating.Items.AddRange(new object[] { "⭐⭐⭐⭐⭐ Excellent", "⭐⭐⭐⭐ Very Good", "⭐⭐⭐ Good", "⭐⭐ Fair", "⭐ Poor" });
            _cmbRating.SelectedIndex = 0;
            mainPanel.Controls.Add(_cmbRating, 0, 2);

            // 3. Feedback Text
            _txtFeedback = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                PlaceholderText = "Tell us what you think...",
                Margin = new Padding(0, 0, 0, 15)
            };
            UITheme.ApplyInputTheme(_txtFeedback);
            mainPanel.Controls.Add(_txtFeedback, 0, 3);

            // 4. Submit Button
            _btnSubmit = new Guna2Button
            {
                Text = "Submit Feedback",
                Dock = DockStyle.Fill,
                Height = 45
            };
            UITheme.ApplyButtonTheme(_btnSubmit, true);
            _btnSubmit.Click += BtnSubmit_Click;
            mainPanel.Controls.Add(_btnSubmit, 0, 4);
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            var feedback = _txtFeedback.Text.Trim();
            if (string.IsNullOrEmpty(feedback))
            {
                MessageBox.Show("Please enter your feedback.", "Feedback Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RailwayKiosk");
                var feedbackDir = Path.Combine(baseDir, "Feedback");
                Directory.CreateDirectory(feedbackDir);
                var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(feedbackDir, fileName);
                var rating = _cmbRating.SelectedItem?.ToString() ?? "";
                var content = $"Rating: {rating}\nDate: {DateTime.Now}\n\n{feedback}";
                File.WriteAllText(filePath, content);
                
                // Show Success Message nicely
                MessageBox.Show("Thank you for your feedback!", "Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save feedback:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
