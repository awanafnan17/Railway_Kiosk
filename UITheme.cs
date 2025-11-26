using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace RailwayKiosk
{
    public static class UITheme
    {
        // Color Palette
        public static readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);    // Midnight Blue
        public static readonly Color SecondaryColor = Color.FromArgb(230, 126, 34); // Carrot Orange
        public static readonly Color AccentColor = Color.FromArgb(52, 152, 219);    // Blue
        public static readonly Color BackgroundColor = Color.FromArgb(236, 240, 241); // Light Gray
        public static readonly Color SurfaceColor = Color.White;
        public static readonly Color TextColor = Color.Black;
        public static readonly Color TextSecondaryColor = Color.FromArgb(64, 64, 64);
        public static readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        public static readonly Color ErrorColor = Color.FromArgb(192, 57, 43);

        // Fonts
        public static Font GetTitleFont(float size = 24f) => new Font("Segoe UI", size, FontStyle.Bold);
        public static Font GetHeaderFont(float size = 16f) => new Font("Segoe UI", size, FontStyle.Bold);
        public static Font GetBodyFont(float size = 11f) => new Font("Segoe UI", size, FontStyle.Regular);

        // Styling Helpers
        public static void ApplyButtonTheme(Guna2Button button, bool isPrimary = true)
        {
            button.BorderRadius = 6;
            button.Font = GetHeaderFont(12f);
            button.FillColor = isPrimary ? PrimaryColor : SecondaryColor;
            button.ForeColor = Color.White;
            button.HoverState.FillColor = isPrimary ? ControlPaint.Light(PrimaryColor) : ControlPaint.Light(SecondaryColor);
            button.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        public static void ApplyInputTheme(Guna2TextBox textBox)
        {
            textBox.BorderRadius = 6;
            textBox.BorderColor = Color.Silver;
            textBox.FocusedState.BorderColor = AccentColor;
            textBox.Font = GetBodyFont();
            textBox.ForeColor = TextColor;
            textBox.FillColor = SurfaceColor;
            // Removed PlaceholderText as it's not supported in all Guna versions or caused issues previously
        }

        public static void ApplyGridTheme(Guna2DataGridView grid)
        {
            grid.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            grid.ThemeStyle.HeaderStyle.BackColor = PrimaryColor;
            grid.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            grid.ThemeStyle.HeaderStyle.Font = GetHeaderFont(12f);
            grid.ThemeStyle.RowsStyle.Font = GetBodyFont();
            grid.ThemeStyle.RowsStyle.ForeColor = TextColor;
            grid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(245, 245, 245);
            grid.RowHeadersVisible = false;
            grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(231, 229, 255);
        }
    }
}
