using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    public class AdminDashboardForm : Form
    {
        private Guna2TabControl _tabControl;
        private TabPage _tabUsers;
        private TabPage _tabTrains;

        // User Management Controls
        private DataGridView _gridUsers;
        private Guna2Button _btnDeleteUser;
        private Guna2Button _btnToggleRole;

        // Train Management Controls
        private DataGridView _gridTrains;
        private Guna2Button _btnAddTrain;
        private Guna2Button _btnEditTrain;
        private Guna2Button _btnDeleteTrain;
        private Guna2Button _btnSaveTrains;

        private List<Train> _trains;

        public AdminDashboardForm()
        {
            this.Text = "Admin Dashboard";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UITheme.BackgroundColor;
            
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            // Header
            var headerPanel = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FillColor = UITheme.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "ADMIN DASHBOARD",
                Font = UITheme.GetHeaderFont(16f),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(lblTitle);
            
             // Close Button
            var btnClose = new Guna2ControlBox
            {
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                IconColor = Color.White
            };
            headerPanel.Controls.Add(btnClose);

            this.Controls.Add(headerPanel);

            // Tabs
            _tabControl = new Guna2TabControl
            {
                Dock = DockStyle.Fill,
                TabButtonHoverState = { InnerColor = UITheme.AccentColor },
                TabButtonSelectedState = { InnerColor = UITheme.PrimaryColor, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                TabButtonIdleState = { InnerColor = UITheme.SurfaceColor, Font = new Font("Segoe UI", 10) },
            };

            _tabUsers = new TabPage { Text = "User Management", BackColor = UITheme.SurfaceColor };
            _tabTrains = new TabPage { Text = "Train Management", BackColor = UITheme.SurfaceColor };

            _tabControl.Controls.Add(_tabUsers);
            _tabControl.Controls.Add(_tabTrains);
            
            // Adjust Tab alignment/style if needed, but defaults are usually okay for Guna2TabControl
            // Actually Guna2TabControl needs alignment setup sometimes.
            _tabControl.Alignment = TabAlignment.Top;
            
            this.Controls.Add(_tabControl);
            headerPanel.SendToBack(); // Ensure header is top
            _tabControl.BringToFront();

            InitializeUserTab();
            InitializeTrainTab();
        }

        private void InitializeUserTab()
        {
            var panel = new Guna2Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            _tabUsers.Controls.Add(panel);

            // Buttons Panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight
            };

            _btnDeleteUser = CreateButton("Delete User", Color.Red);
            _btnDeleteUser.Click += BtnDeleteUser_Click;
            
            _btnToggleRole = CreateButton("Toggle Role (Admin/User)", UITheme.SecondaryColor);
            _btnToggleRole.Click += BtnToggleRole_Click;

            buttonsPanel.Controls.Add(_btnToggleRole);
            buttonsPanel.Controls.Add(_btnDeleteUser);
            
            panel.Controls.Add(buttonsPanel);

            // Grid
            _gridUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            panel.Controls.Add(_gridUsers);
            buttonsPanel.SendToBack();
            _gridUsers.BringToFront();
        }

        private void InitializeTrainTab()
        {
            var panel = new Guna2Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            _tabTrains.Controls.Add(panel);

            // Buttons Panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight
            };

            _btnAddTrain = CreateButton("Add Train", UITheme.SuccessColor);
            _btnAddTrain.Click += BtnAddTrain_Click;

            _btnEditTrain = CreateButton("Edit Train", UITheme.SecondaryColor);
            _btnEditTrain.Click += BtnEditTrain_Click;

            _btnDeleteTrain = CreateButton("Delete Train", Color.Red);
            _btnDeleteTrain.Click += BtnDeleteTrain_Click;
            
            _btnSaveTrains = CreateButton("Save Changes", UITheme.PrimaryColor);
            _btnSaveTrains.Click += BtnSaveTrains_Click;

            buttonsPanel.Controls.Add(_btnAddTrain);
            buttonsPanel.Controls.Add(_btnEditTrain);
            buttonsPanel.Controls.Add(_btnDeleteTrain);
            buttonsPanel.Controls.Add(_btnSaveTrains);
            
            panel.Controls.Add(buttonsPanel);

            // Grid
            _gridTrains = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true, // We'll edit via dialog or allow cell edit? Let's use Dialog for safety/validation.
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            panel.Controls.Add(_gridTrains);
            buttonsPanel.SendToBack();
            _gridTrains.BringToFront();
        }

        private Guna2Button CreateButton(string text, Color color)
        {
            var btn = new Guna2Button
            {
                Text = text,
                FillColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Height = 35,
                AutoSize = true,
                BorderRadius = 5,
                Margin = new Padding(0, 0, 10, 0)
            };
            return btn;
        }

        private void LoadData()
        {
            LoadUsers();
            LoadTrains();
        }

        private void LoadUsers()
        {
            var users = UserService.GetAllUsers();
            // Bind to grid
            // We can't bind Dictionary directly easily to show Key and Value as columns without transformation
            var list = users.Select(u => new { Username = u.Key, Role = u.Value }).ToList();
            _gridUsers.DataSource = list;
        }

        private void LoadTrains()
        {
            _trains = TrainService.LoadTrains(AppDomain.CurrentDomain.BaseDirectory);
            RefreshTrainGrid();
        }

        private void RefreshTrainGrid()
        {
            _gridTrains.DataSource = null;
            _gridTrains.DataSource = _trains;
            // Format columns if needed
             if (_gridTrains.Columns["DepartureTime"] != null)
                _gridTrains.Columns["DepartureTime"].DefaultCellStyle.Format = "g";
        }

        // --- User Actions ---

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (_gridUsers.SelectedRows.Count == 0) return;
            var username = _gridUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            
            if (MessageBox.Show($"Are you sure you want to delete user '{username}'?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (UserService.DeleteUser(username))
                {
                    MessageBox.Show("User deleted.");
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("Could not delete user (Admin cannot be deleted).");
                }
            }
        }

        private void BtnToggleRole_Click(object sender, EventArgs e)
        {
            if (_gridUsers.SelectedRows.Count == 0) return;
            var username = _gridUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            var currentRole = _gridUsers.SelectedRows[0].Cells["Role"].Value.ToString();
            
            var newRole = currentRole == "Admin" ? "User" : "Admin";
            
            if (UserService.UpdateUserRole(username, newRole))
            {
                MessageBox.Show($"User '{username}' is now {newRole}.");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Could not update role (Main Admin role cannot be changed).");
            }
        }

        // --- Train Actions ---

        private void BtnAddTrain_Click(object sender, EventArgs e)
        {
             // Simple Input Dialogs for now, or a custom small form
             // For simplicity, let's just add a dummy and let them edit, or show a simple input form.
             // Better: Create a dedicated TrainEditForm.
             
             var form = new TrainEditForm();
             if (form.ShowDialog() == DialogResult.OK)
             {
                 _trains.Add(form.Train);
                 RefreshTrainGrid();
             }
        }

        private void BtnEditTrain_Click(object sender, EventArgs e)
        {
            if (_gridTrains.SelectedRows.Count == 0) return;
            var train = _gridTrains.SelectedRows[0].DataBoundItem as Train;
            if (train == null) return;

            var form = new TrainEditForm(train);
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Update happens in reference if reference passed, or we replace
                // Since we passed reference, it might be updated already.
                RefreshTrainGrid();
            }
        }

        private void BtnDeleteTrain_Click(object sender, EventArgs e)
        {
            if (_gridTrains.SelectedRows.Count == 0) return;
            var train = _gridTrains.SelectedRows[0].DataBoundItem as Train;
            
            if (MessageBox.Show($"Delete train {train.TrainNumber}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _trains.Remove(train);
                RefreshTrainGrid();
            }
        }

        private void BtnSaveTrains_Click(object sender, EventArgs e)
        {
            TrainService.SaveTrains(AppDomain.CurrentDomain.BaseDirectory, _trains);
            MessageBox.Show("Trains saved successfully.");
        }
    }
}
