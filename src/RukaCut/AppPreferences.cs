using System.Globalization;

namespace RukaCut;

internal static class AppPreferences
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RukaCut");
    private static readonly string LanguageFile = Path.Combine(Folder, "language.txt");

    public static AppLanguage LoadLanguage()
    {
        try
        {
            if (File.Exists(LanguageFile))
                return File.ReadAllText(LanguageFile).Trim() == "en" ? AppLanguage.English : AppLanguage.Chinese;
        }
        catch { }
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh" ? AppLanguage.Chinese : AppLanguage.English;
    }

    public static void SaveLanguage(AppLanguage language)
    {
        Directory.CreateDirectory(Folder);
        File.WriteAllText(LanguageFile, language == AppLanguage.English ? "en" : "zh");
    }
}
