using System.Drawing;
using Xunit;

namespace RukaCut.Tests;

public sealed class ModernButtonTests
{
    [Trait("Category", "WindowsIntegration")]
    [Fact]
    public void DarkButton_PaintsBottomAndRightEdgesCleanly()
    {
        using var button = new ModernButton { Size = new Size(140, 40), BackColor = Color.FromArgb(42, 42, 42) };
        using var bitmap = new Bitmap(button.Width, button.Height);
        using (var graphics = Graphics.FromImage(bitmap)) graphics.Clear(Color.White);

        button.DrawToBitmap(bitmap, button.ClientRectangle);

        Assert.Equal(button.BackColor.ToArgb(), bitmap.GetPixel(button.Width / 2, button.Height - 1).ToArgb());
        Assert.Equal(button.BackColor.ToArgb(), bitmap.GetPixel(button.Width - 1, button.Height / 2).ToArgb());
    }
}
