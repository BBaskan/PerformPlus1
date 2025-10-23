using System.Configuration;
using System.Data;
using System.Windows;

namespace PerformPlus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void SwitchLanguage(string culture)
        {
            var dictUri = new Uri($"Resources/Localization/Strings.{culture}.xaml", UriKind.Relative);
            var locDict = new ResourceDictionary() { Source = dictUri };

            var merged = Current.Resources.MergedDictionaries;
            // remove any existing Strings.* dictionary
            for (int i = 0; i < merged.Count; i++)
            {
                if (merged[i].Source?.OriginalString.Contains("Strings.") == true)
                {
                    merged.RemoveAt(i);
                    break;
                }
            }
            // add the new one
            merged.Add(locDict);
        }

    }

}
