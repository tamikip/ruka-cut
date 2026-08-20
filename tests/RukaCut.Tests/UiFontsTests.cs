using Xunit;

namespace RukaCut.Tests;

public sealed class UiFontsTests
{
    [Fact]
    public void FamilyName_UsesMicrosoftYaHeiUi()
    {
        Assert.Equal("Microsoft YaHei UI", UiFonts.FamilyName);
    }
}
