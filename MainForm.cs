using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Speech.Synthesis;

namespace RailwayKiosk
{
    /// <summary>
    /// The main kiosk window provides travellers with a simple interface
    /// for querying the status of trains departing from the station.  It
    /// exposes filters for train number, destination, train type and
    /// departure time and displays matching results in a data grid.  The
    /// form also includes accessibility features such as adjustable font
    /// sizes and text‑to‑speech output for the selected train.
    /// </summary>
    public class MainForm : Form
    {
        // Backing list of all trains read from disk
        private List<Train> _trains = new();

        // UI controls
        private readonly Guna2TextBox _txtTrainNumber;
        private readonly Guna2ComboBox _cmbDestination;
        private readonly Guna2ComboBox _cmbTrainType;
        private readonly DateTimePicker _dtpDeparture;
        private readonly Guna2Button _btnSearch;
        private readonly Guna2DataGridView _dgvResults;
        private readonly Guna2Button _btnVoice;
        private readonly Guna2Button _btnFeedback;
        private readonly Guna2Button _btnIncreaseFont;
        private readonly Guna2Button _btnDecreaseFont;

        // Track base font size to allow dynamic adjustments
        private float _baseFontSize = 10f;

        public MainForm()
        {
            // Configure window properties
            this.Text = "Railway Station Kiosk";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", _baseFontSize);

            // Top panel holds search controls
            var panelSearch = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(panelSearch);

            // Bottom panel holds results grid
            var panelResults = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            this.Controls.Add(panelResults);

            // Instantiate controls
            _txtTrainNumber = new Guna2TextBox
            {
                PlaceholderText = "Train Number",
                Width = 150,
                Margin = new Padding(5)
            };
            _cmbDestination = new Guna2ComboBox
            {
                Width = 150,
                Margin = new Padding(5),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbTrainType = new Guna2ComboBox
            {
                Width = 150,
                Margin = new Padding(5),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _dtpDeparture = new DateTimePicker
            {
                Width = 180,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Margin = new Padding(5)
            };
            _btnSearch = new Guna2Button
            {
                Text = "Search",
                Width = 100,
                Margin = new Padding(5)
            };
            _btnSearch.Click += BtnSearch_Click;
            _btnVoice = new Guna2Button
            {
                Text = "Speak Selected",
                Width = 120,
                Margin = new Padding(5)
            };
            _btnVoice.Click += BtnVoice_Click;
            _btnFeedback = new Guna2Button
            {
                Text = "Feedback",
                Width = 100,
                Margin = new Padding(5)
            };
            _btnFeedback.Click += BtnFeedback_Click;
            _btnIncreaseFont = new Guna2Button
            {
                Text = "A+",
                Width = 50,
                Margin = new Padding(5)
            };
            _btnIncreaseFont.Click += (_, _) => ChangeFontSize(1);
            _btnDecreaseFont = new Guna2Button
            {
                Text = "A-",
                Width = 50,
                Margin = new Padding(5)
            };
            _btnDecreaseFont.Click += (_, _) => ChangeFontSize(-1);

            // Compose search layout using a FlowLayoutPanel for natural wrapping
            var searchLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false
            };
            searchLayout.Controls.AddRange(new Control[]
            {
                _txtTrainNumber,
                _cmbDestination,
                _cmbTrainType,
                _dtpDeparture,
                _btnSearch,
                _btnVoice,
                _btnFeedback,
                _btnIncreaseFont,
                _btnDecreaseFont
            });
            panelSearch.Controls.Add(searchLayout);

            // Create results grid
            _dgvResults = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            // Define columns corresponding to Train properties
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Train.TrainNumber),
                HeaderText = "Train Number"
            });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Train.Destination),
                HeaderText = "Destination"
            });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Train.DepartureTime),
                HeaderText = "Departure Time",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "t" }
            });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Train.TrainType),
                HeaderText = "Type"
            });
            _dgvResults.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Train.Status),
                HeaderText = "Status"
            });
            panelResults.Controls.Add(_dgvResults);

            // Load train data and populate filters
            LoadTrains();
            PopulateFilters();
            ApplyAccessibilityDefaults();
        }

        /// <summary>
        /// Reads the train schedule from trains.json into the _trains list.  If
        /// the file cannot be found or parsed, a message is displayed and
        /// the list will remain empty.
        /// </summary>
        private void LoadTrains()
        {
            try
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "TrainData", "trains.json");
                if (File.Exists(jsonPath))
                {
                    var json = File.ReadAllText(jsonPath);
                    _trains = JsonSerializer.Deserialize<List<Train>>(json) ?? new List<Train>();
                    _dgvResults.DataSource = _trains;
                }
                else
                {
                    MessageBox.Show($"Train data file not found at {jsonPath}.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load train data:\n{ex.Message}", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Populates the destination and train type combo boxes based on the
        /// loaded train list.  An "All" option is added to allow users to
        /// bypass a filter criterion.
        /// </summary>
        private void PopulateFilters()
        {
            var destinations = new SortedSet<string>(_trains.Select(t => t.Destination));
            var types = new SortedSet<string>(_trains.Select(t => t.TrainType));
            _cmbDestination.Items.Clear();
            _cmbDestination.Items.Add("All");
            foreach (var dest in destinations)
                _cmbDestination.Items.Add(dest);
            _cmbDestination.SelectedIndex = 0;
            _cmbTrainType.Items.Clear();
            _cmbTrainType.Items.Add("All");
            foreach (var type in types)
                _cmbTrainType.Items.Add(type);
            _cmbTrainType.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the search button click.  Filters the train list based on
        /// the criteria entered by the user and binds the results to the
        /// DataGridView.  Departure time filtering matches trains within
        /// ±60 minutes of the selected time.
        /// </summary>
        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            // Create a copy of the full list so filtering does not modify
            // the backing collection
            var filtered = new List<Train>(_trains);
            var number = _txtTrainNumber.Text.Trim();
            var dest = _cmbDestination.SelectedItem?.ToString();
            var type = _cmbTrainType.SelectedItem?.ToString();
            var selectedTime = _dtpDeparture.Value;
            if (!string.IsNullOrEmpty(number))
            {
                filtered = filtered.Where(t => t.TrainNumber.Contains(number, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(dest) && dest != "All")
            {
                filtered = filtered.Where(t => t.Destination.Equals(dest, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(type) && type != "All")
            {
                filtered = filtered.Where(t => t.TrainType.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            // Filter trains within one hour of the selected departure time
            filtered = filtered.Where(t => Math.Abs((t.DepartureTime - selectedTime).TotalMinutes) < 60).ToList();
            _dgvResults.DataSource = filtered;
        }

        /// <summary>
        /// Uses the Microsoft SAPI to read aloud information about the
        /// currently selected train.  If no row is selected, a prompt is
        /// displayed asking the user to select a row.
        /// </summary>
        private void BtnVoice_Click(object? sender, EventArgs e)
        {
            if (_dgvResults.SelectedRows.Count > 0)
            {
                if (_dgvResults.SelectedRows[0].DataBoundItem is Train train)
                {
                    try
                    {
                        using var synthesizer = new SpeechSynthesizer();
                        synthesizer.Speak($"Train {train.TrainNumber} to {train.Destination} departs at {train.DepartureTime:hh:mm tt} and is currently {train.Status}.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to speak:\n{ex.Message}", "Speech Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a train row to hear its details.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Opens the feedback form as a modal dialog when the feedback
        /// button is clicked.
        /// </summary>
        private void BtnFeedback_Click(object? sender, EventArgs e)
        {
            using var fb = new FeedbackForm();
            fb.ShowDialog(this);
        }

        /// <summary>
        /// Adjusts the overall font size of the window by the specified
        /// increment.  Ensures the font never drops below 8 pt for
        /// readability.  Recursively updates child control fonts.
        /// </summary>
        private void ChangeFontSize(int delta)
        {
            _baseFontSize = Math.Max(8f, _baseFontSize + delta);
            this.Font = new Font(this.Font.FontFamily, _baseFontSize);
            foreach (Control c in this.Controls)
                UpdateFontRecursive(c, _baseFontSize);
        }

        private void UpdateFontRecursive(Control control, float size)
        {
            control.Font = new Font(control.Font.FontFamily, size);
            foreach (Control child in control.Controls)
            {
                UpdateFontRecursive(child, size);
            }
        }

        /// <summary>
        /// Applies sensible default accessibility settings: increase the
        /// baseline font size slightly to aid readability and ensure
        /// high‑contrast colours for text on the panels.
        /// </summary>
        private void ApplyAccessibilityDefaults()
        {
            ChangeFontSize(2); // enlarge fonts
            // Set high contrast for search panel items
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Guna2Panel panel)
                {
                    panel.ForeColor = Color.Black;
                    panel.BackColor = Color.WhiteSmoke;
                }
            }
        }
    }
}