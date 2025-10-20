using UnityEngine;

public static class SettingsKeys
{
    public const string General = "General";
    public const string Music = "Music";
    public const string Sfx = "SFX";
    public const string ResolutionIndex = "ResolutionIndex";
    public const string FullScreen = "FullScreen";
    
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
    
    public static float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key, defaultValue) : defaultValue;
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}
