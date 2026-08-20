using Xunit;

namespace RukaCut.Tests;

public sealed class UiTextTests
{
    [Theory]
    [InlineData(AppLanguage.Chinese, "Record", "录音")]
    [InlineData(AppLanguage.English, "Record", "Record")]
    [InlineData(AppLanguage.Chinese, "Export", "导出 MP3")]
    [InlineData(AppLanguage.English, "Export", "Export MP3")]
    public void Get_ReturnsRequestedLanguage(AppLanguage language, string key, string expected)
    {
        Assert.Equal(expected, UiText.Get(language, key));
    }

    [Fact]
    public void Catalog_HasMatchingKeysForBothLanguages()
    {
        Assert.Empty(UiText.MissingKeys);
    }
}
