using UnityEngine;


[System.Serializable]
public enum Language
{
    Ru,
    En
}

[System.Serializable]
public class LocalizationStrings
{
    public string key;

    public string russianText;
    public string englishText;
}

[CreateAssetMenu(fileName = "LocalizationTable", menuName = "LocalizationTable")]
public class LocalizationTable : ScriptableObject {
    public LocalizationStrings[] strings;
}