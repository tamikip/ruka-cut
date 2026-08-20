using System.Drawing.Drawing2D;

namespace RukaCut;

internal sealed class WaveformPanel : Control
{
    private float[] peaks = [];
    private bool draggingStart;
    private float? playbackRatio;
    private float displayGain = 1;

    public float StartRatio { get; private set; }
    public float EndRatio { get; private set; } = 1;
    public string EmptyText { get; set; } = "Record or open audio";
    public event EventHandler? SelectionChanged;

    public WaveformPanel()
    {
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
    }

    public void SetAudio(float[] values)
    {
        peaks = values;
        displayGain = WaveformScaler.CalculateGain(values);
        StartRatio = 0;
        EndRatio = 1;
        Invalidate();
    }

    public void SetPlaybackRatio(float? ratio)
    {
        playbackRatio = ratio is null ? null : Math.Clamp(ratio.Value, StartRatio, EndRatio);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var background = new SolidBrush(Color.FromArgb(13, 13, 13));
        e.Graphics.FillRoundedRectangle(background, ClientRectangle, 12);
        if (peaks.Length == 0)
        {
            TextRenderer.DrawText(e.Graphics, EmptyText, Font, ClientRectangle,
                Color.FromArgb(128, 128, 128), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            return;
        }

        var middle = Height / 2f;
        using var wavePen = new Pen(Color.FromArgb(145, 145, 145), 2);
        for (var i = 0; i < peaks.Length; i++)
        {
            var x = i * Width / (float)Math.Max(1, peaks.Length - 1);
            var amplitude = Math.Min(1, peaks[i] * displayGain) * (Height - 24) / 2f;
            e.Graphics.DrawLine(wavePen, x, middle - amplitude, x, middle + amplitude);
        }

        var startX = StartRatio * Width;
        var endX = EndRatio * Width;
        using var shade = new SolidBrush(Color.FromArgb(185, 0, 0, 0));
        e.Graphics.FillRectangle(shade, 0, 0, startX, Height);
        e.Graphics.FillRectangle(shade, endX, 0, Width - endX, Height);
        using var accent = new Pen(Color.White, 3);
        e.Graphics.DrawLine(accent, startX, 5, startX, Height - 5);
        e.Graphics.DrawLine(accent, endX, 5, endX, Height - 5);
        if (playbackRatio is float ratio)
        {
            var playX = ratio * Width;
            using var playhead = new Pen(Color.White, 2);
            e.Graphics.DrawLine(playhead, playX, 0, playX, Height);
            e.Graphics.FillEllipse(Brushes.White, playX - 4, 0, 8, 8);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        draggingStart = Math.Abs(e.X - StartRatio * Width) <= Math.Abs(e.X - EndRatio * Width);
        UpdateHandle(e.X);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) UpdateHandle(e.X);
    }

    private void UpdateHandle(int x)
    {
        var ratio = Math.Clamp(x / (float)Math.Max(1, Width), 0, 1);
        if (draggingStart) StartRatio = Math.Min(ratio, EndRatio - 0.001f);
        else EndRatio = Math.Max(ratio, StartRatio + 0.001f);
        Invalidate();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, float radius)
    {
        using var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        graphics.FillPath(brush, path);
    }
}
