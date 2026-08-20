using System.Drawing.Drawing2D;

namespace RukaCut;

internal static class RoundedGeometry
{
    public static GraphicsPath Create(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        if (diameter <= 0) { path.AddRectangle(bounds); return path; }
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal sealed class RoundedPanel : Panel
{
    public int Radius { get; init; } = 18;

    public RoundedPanel()
    {
        DoubleBuffered = true;
        Resize += (_, _) => UpdateRegion();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = RoundedGeometry.Create(ClientRectangle, Radius);
        using var brush = new SolidBrush(BackColor);
        e.Graphics.FillPath(brush, path);
    }

    private void UpdateRegion()
    {
        using var path = RoundedGeometry.Create(ClientRectangle, Radius);
        Region = new Region(path);
    }
}

internal sealed class ModernButton : Button
{
    public int Radius { get; init; } = 13;

    public ModernButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.White;
        ForeColor = Color.Black;
        Font = new Font(UiFonts.FamilyName, 10, FontStyle.Bold);
        Cursor = Cursors.Hand;
        UseVisualStyleBackColor = false;
        Resize += (_, _) => UpdateRegion();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? BackColor);
        using var path = RoundedGeometry.Create(ClientRectangle, Radius);
        using var brush = new SolidBrush(Enabled ? BackColor : Color.FromArgb(55, 55, 55));
        e.Graphics.FillPath(brush, path);
        var color = Enabled ? ForeColor : Color.FromArgb(125, 125, 125);
        TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
    }

    private void UpdateRegion()
    {
        using var path = RoundedGeometry.Create(ClientRectangle, Radius);
        var previous = Region;
        Region = new Region(path);
        previous?.Dispose();
    }
}
