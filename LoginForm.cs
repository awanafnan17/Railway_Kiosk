using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    /// <summary>
    /// A simple login form to authenticate an admin user. Guests can
    /// bypass login and access the kiosk directly.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly IContainer components;
        private readonly Guna2BorderlessForm _borderlessForm;
        private readonly Guna2ShadowForm _shadowForm;
        
        private Guna2TextBox _txtUser;
        private Guna2TextBox _txtPass;
        private Guna2Button _btnLogin;
        private Guna2Button _btnGuest;
        private Guna2Button _btnSignup; // Added Sign Up button
        private Guna2ControlBox _btnClose;
        private Guna2ControlBox _btnMinimize;

        public LoginForm()
        {
            Serilog.Log.Information("LoginForm constructor entering");
            
            components = new Container();
            // Modern Borderless Look
            this.FormBorderStyle = FormBorderStyle.None;
            _borderlessForm = new Guna2BorderlessForm(components);
            _borderlessForm.ContainerControl = this;
            _borderlessForm.DockIndicatorTransparencyValue = 0.6;
            _borderlessForm.TransparentWhileDrag = true;
            
            // Shadow for depth
            _shadowForm = new Guna2ShadowForm(components);
            _shadowForm.TargetForm = this;

            this.Text = "Login - Railway Kiosk";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(800, 500); // Slightly increased size
            this.BackColor = UITheme.SurfaceColor;
            this.Icon = SystemIcons.Application;

            // Enable Double Buffering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            new Guna2AnimateWindow(components)
            {
                AnimationType = Guna.UI2.WinForms.Guna2AnimateWindow.AnimateWindowType.AW_BLEND,
                Interval = 500,
                TargetForm = this
            };

            InitializeUI();
            
            this.Load += (s, e) => {
                _shadowForm.SetShadowForm(this);
                Serilog.Log.Information("LoginForm Loaded");
            };
            
            Serilog.Log.Information("LoginForm constructor exiting");
        }

        private void InitializeUI()
        {
            // Use TableLayoutPanel to split Left and Right sections cleanly
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F)); // Fixed Left Width
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Remaining for Right
            this.Controls.Add(layout);

            // --- Left Panel (Branding) ---
            var leftPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = UITheme.PrimaryColor
            };
            
            var lblBrand = new Label
            {
                Text = "Railway\nKiosk",
                Font = UITheme.GetTitleFont(32f),
                ForeColor = Color.White,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            // Center roughly in the panel using relative positioning
            lblBrand.Location = new Point(50, 180); 
            
            var lblSub = new Label
            {
                Text = "Admin Access",
                Font = UITheme.GetBodyFont(12f),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Top = lblBrand.Bottom + 10,
                Left = 90,
                BackColor = Color.Transparent
            };

            leftPanel.Controls.Add(lblBrand);
            leftPanel.Controls.Add(lblSub);
            layout.Controls.Add(leftPanel, 0, 0);

            // --- Right Panel (Content) ---
            var rightPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };
            layout.Controls.Add(rightPanel, 1, 0);

            // Window Controls
            _btnClose = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.Gray,
                HoverState = { FillColor = UITheme.ErrorColor, IconColor = Color.White },
                Left = rightPanel.Width - 50
            };
            _btnMinimize = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.Transparent,
                IconColor = Color.Gray,
                Left = rightPanel.Width - 95
            };
            
            // Login Header
            var headerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(40, 40, 0, 0)
            };
            
            var lblLogin = new Label
            {
                Text = "Welcome Back",
                Font = UITheme.GetHeaderFont(20f),
                ForeColor = UITheme.TextColor,
                AutoSize = true
            };
            headerPanel.Controls.Add(lblLogin);
            rightPanel.Controls.Add(headerPanel);

            // Form Layout
            var formLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40, 10, 0, 0),
                WrapContents = false
            };
            rightPanel.Controls.Add(formLayout);

            // Inputs
            // Username
            var lblUser = new Label
            {
                Text = "Username",
                Font = UITheme.GetBodyFont(10f),
                ForeColor = UITheme.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(5, 10, 0, 5)
            };
            formLayout.Controls.Add(lblUser);

            _txtUser = new Guna2TextBox
            {
                PlaceholderText = "",
                Width = 350,
                Height = 40
            };
            UITheme.ApplyInputTheme(_txtUser);
            formLayout.Controls.Add(_txtUser);

            // Password
            var lblPass = new Label
            {
                Text = "Password",
                Font = UITheme.GetBodyFont(10f),
                ForeColor = UITheme.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(5, 15, 0, 5)
            };
            formLayout.Controls.Add(lblPass);

            _txtPass = new Guna2TextBox
            {
                PlaceholderText = "",
                Width = 350,
                Height = 40,
                PasswordChar = '●',
                UseSystemPasswordChar = true
            };
            UITheme.ApplyInputTheme(_txtPass);
            formLayout.Controls.Add(_txtPass);

            // Buttons
            _btnLogin = new Guna2Button
            {
                Text = "LOGIN",
                Width = 350,
                Height = 45,
                Margin = new Padding(0, 30, 0, 0)
            };
            UITheme.ApplyButtonTheme(_btnLogin, true);
            _btnLogin.Click += BtnLogin_Click;
            formLayout.Controls.Add(_btnLogin);

            _btnGuest = new Guna2Button
            {
                Text = "Continue as Guest",
                Width = 350,
                Height = 45,
                FillColor = Color.Transparent,
                ForeColor = UITheme.TextSecondaryColor,
                Font = UITheme.GetBodyFont(10f),
                BorderColor = Color.LightGray,
                BorderThickness = 1,
                BorderRadius = 6,
                Margin = new Padding(0, 15, 0, 0)
            };
            _btnGuest.HoverState.FillColor = Color.FromArgb(240, 240, 240);
            _btnGuest.Click += BtnGuest_Click;
            formLayout.Controls.Add(_btnGuest);

            _btnSignup = new Guna2Button
            {
                Text = "Sign Up",
                Width = 350,
                Height = 45,
                FillColor = Color.Transparent,
                ForeColor = UITheme.AccentColor,
                Font = UITheme.GetBodyFont(10f),
                Margin = new Padding(0, 5, 0, 0)
            };
            _btnSignup.HoverState.FillColor = Color.FromArgb(240, 240, 240);
            _btnSignup.Click += BtnSignup_Click;
            formLayout.Controls.Add(_btnSignup);
            
            // Add Window Controls to rightPanel and BringToFront so they are on top of everything
            rightPanel.Controls.Add(_btnClose);
            rightPanel.Controls.Add(_btnMinimize);
            _btnClose.BringToFront();
            _btnMinimize.BringToFront();

            // Enter key support
            this.AcceptButton = _btnLogin;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var username = _txtUser.Text.Trim();
            var password = _txtPass.Text;
            
            if (UserService.ValidateUser(username, password))
            {
                Serilog.Log.Information("Login successful for user: {User}", username);
                LaunchHome(username);
            }
            else
            {
                Serilog.Log.Warning("Login failed for user: {User}", username);
                MessageBox.Show("Invalid credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnGuest_Click(object? sender, EventArgs e)
        {
            Serilog.Log.Information("Continuing as Guest");
            LaunchHome();
        }

        private void BtnSignup_Click(object? sender, EventArgs e)
        {
            this.Hide();
            new RegisterForm().Show();
        }

        private void LaunchHome(string username = "Guest")
        {
            this.Hide();
            var home = new HomeForm(username);
            home.FormClosed += (_, _) => this.Close();
            home.Show();
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
