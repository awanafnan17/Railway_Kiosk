using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    public class RegisterForm : Form
    {
        private readonly IContainer components;
        private readonly Guna2BorderlessForm _borderlessForm;
        private readonly Guna2ShadowForm _shadowForm;

        private Guna2TextBox _txtUser;
        private Guna2TextBox _txtPass;
        private Guna2TextBox _txtConfirmPass;
        private Guna2Button _btnRegister;
        private Guna2Button _btnBack;
        private Guna2ControlBox _btnClose;

        public RegisterForm()
        {
            components = new Container();
            this.FormBorderStyle = FormBorderStyle.None;
            _borderlessForm = new Guna2BorderlessForm(components) { ContainerControl = this };
            _shadowForm = new Guna2ShadowForm(components) { TargetForm = this };

            this.Text = "Register - Railway Kiosk";
            this.Size = new Size(750, 500); // Slightly taller for extra field
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UITheme.SurfaceColor;

            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeUI();
        }

        private void InitializeUI()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.Controls.Add(table);

            // Left Panel (Branding)
            var leftPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = UITheme.PrimaryColor
            };
            
            var lblBrand = new Label
            {
                Text = "Join Us",
                Font = UITheme.GetTitleFont(32f),
                ForeColor = Color.White,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            // Centering logic handled by Resize event or table anchoring
            // Simpler: Use a TableLayout inside the panel or just relative positioning
            lblBrand.Location = new Point(50, 200); // Approximate

            leftPanel.Controls.Add(lblBrand);
            table.Controls.Add(leftPanel, 0, 0);

            // Right Panel (Content)
            var rightPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };
            table.Controls.Add(rightPanel, 1, 0);

            // Window Controls
            _btnClose = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.Gray,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White },
                Left = rightPanel.Width - 50
            };
            var btnMinimize = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.Gray,
                Left = rightPanel.Width - 95
            };
            
            rightPanel.Controls.Add(_btnClose);
            rightPanel.Controls.Add(btnMinimize);
            
             // Adjust control box positions on resize
            rightPanel.Resize += (s, e) => {
                 _btnClose.Left = rightPanel.Width - 50;
                 btnMinimize.Left = rightPanel.Width - 95;
            };

            // Header
            var headerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(40, 40, 0, 0)
            };
            
            var lblHeader = new Label
            {
                Text = "Create Account",
                Font = UITheme.GetHeaderFont(20f),
                ForeColor = UITheme.TextColor,
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);
            rightPanel.Controls.Add(headerPanel);

            // Form Layout
            var formLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40, 20, 0, 0),
                WrapContents = false
            };
            rightPanel.Controls.Add(formLayout);

            // Inputs
            // Username
            formLayout.Controls.Add(CreateLabel("Username"));
            _txtUser = CreateInput();
            formLayout.Controls.Add(_txtUser);

            // Password
            formLayout.Controls.Add(CreateLabel("Password"));
            _txtPass = CreateInput(true);
            formLayout.Controls.Add(_txtPass);

            // Confirm Password
            formLayout.Controls.Add(CreateLabel("Confirm Password"));
            _txtConfirmPass = CreateInput(true);
            formLayout.Controls.Add(_txtConfirmPass);

            // Register Button
            _btnRegister = new Guna2Button
            {
                Text = "REGISTER",
                Width = 350,
                Height = 45,
                Margin = new Padding(0, 30, 0, 0)
            };
            UITheme.ApplyButtonTheme(_btnRegister, true);
            _btnRegister.Click += BtnRegister_Click;
            formLayout.Controls.Add(_btnRegister);

            // Back Button
            _btnBack = new Guna2Button
            {
                Text = "Back to Login",
                Width = 350,
                Height = 45,
                FillColor = Color.Transparent,
                ForeColor = UITheme.TextSecondaryColor,
                Font = UITheme.GetBodyFont(10f),
                Margin = new Padding(0, 10, 0, 0)
            };
            _btnBack.HoverState.FillColor = Color.FromArgb(240, 240, 240);
            _btnBack.Click += (s, e) => {
                this.Hide();
                new LoginForm().Show();
            };
            formLayout.Controls.Add(_btnBack);
            
            // Ensure Window Controls are on top
            _btnClose.BringToFront();
            // btnMinimize needs to be accessible too, but it was local variable.
            // I need to find where btnMinimize is defined.
            // It was defined as var btnMinimize = ... at line 91.
            // I cannot access it here unless I change it to field or find it in Controls.
            // But wait, I can just re-add them? No.
            // I should use rightPanel.Controls to find it or just ensure they are added LAST.
            // Or better, change btnMinimize to field or just move the addition to end.
            
            // Actually, I can just BringToFront the ones I have reference to.
            // _btnClose is a field. btnMinimize is NOT.
            // Let's check if I can get btnMinimize from Controls.
            // Or just make btnMinimize a field.
            
            // Checking RegisterForm.cs lines 83-101.
            // _btnClose is field. btnMinimize is var.
            // I should change btnMinimize to field in the class definition.
            // But that requires two edits.
            
            // Alternative: find it in rightPanel.Controls?
            // foreach(Control c in rightPanel.Controls) if(c is Guna2ControlBox && c != _btnClose) c.BringToFront();
            
            // Simpler: Move the creation and addition of Window Controls to the END of InitializeUI.
            // But that involves moving large block of code.
            
            // Let's just edit the end of InitializeUI to find it.
            foreach (Control c in rightPanel.Controls)
            {
                if (c is Guna2ControlBox)
                {
                    c.BringToFront();
                }
            }
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = UITheme.GetBodyFont(10f),
                ForeColor = UITheme.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(5, 15, 0, 5)
            };
        }

        private Guna2TextBox CreateInput(bool isPassword = false)
        {
            var txt = new Guna2TextBox
            {
                Width = 350,
                Height = 40
            };
            if (isPassword)
            {
                txt.PasswordChar = '●';
                txt.UseSystemPasswordChar = true;
            }
            UITheme.ApplyInputTheme(txt);
            return txt;
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            var user = _txtUser.Text.Trim();
            var pass = _txtPass.Text;
            var confirm = _txtConfirmPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (UserService.RegisterUser(user, pass))
            {
                MessageBox.Show("Registration successful! Please login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                new LoginForm().Show();
            }
            else
            {
                MessageBox.Show("Username already taken.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }
    }
}
