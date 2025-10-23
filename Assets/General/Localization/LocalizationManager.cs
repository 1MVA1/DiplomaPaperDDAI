using UnityEngine;


public class LocalizationManager : ScriptableObject
{
    public static string GetText(LocalizationTable table, string key)
    {
        foreach (var str in table.strings)
        {
            if (str.key == key)
            {
                switch (SaveManager.Instance.language)
                {
                    case Language.Ru:
                        return str.russianText;

                    case Language.En:
                        return str.englishText;
                }
            }
        }

        Debug.LogWarning($"Key '{key}' not found in table.");
        return "@error";
    }
}