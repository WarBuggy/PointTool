using System.Text.Json;
using PointTool.Settings;

namespace PointTool.Managers;

public class SettingManager
{
    private const string SettingFileName = "settings.json";

    private readonly Dictionary<Setting, object?> currentSettings = [];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public SettingManager()
    {
        if (!HasSettingFile())
        {
            CreateSettingFile();
        }

        LoadSettings();

        SaveSettings();
    }

    public T Get<T>(Setting<T> setting)
    {
        if (currentSettings.TryGetValue(setting, out object? value) &&
            value is T typedValue)
        {
            return typedValue;
        }

        return setting.Default;
    }

    public void Set<T>(Setting<T> setting, T value)
    {
        currentSettings[setting] = value;

        SaveSettings();
    }

    private static string GetSettingFilePath()
    {
        return Path.Combine(
            PathManager.GetExecutableDirectory(),
            SettingFileName);
    }

    private static bool HasSettingFile()
    {
        return File.Exists(GetSettingFilePath());
    }

    private static void CreateSettingFile()
    {
        var defaultSettings = BuildDefaultSettings();
        WriteSettingFile(defaultSettings);
    }

    private void LoadSettings()
    {
        string json = File.ReadAllText(
            GetSettingFilePath());

        Dictionary<string, JsonElement>? loadedSettings =
            JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        currentSettings.Clear();

        foreach (Setting setting in SettingDefs.All)
        {
            object? value = setting.DefaultValue;

            if (loadedSettings != null &&
                loadedSettings.TryGetValue(
                    setting.Key,
                    out JsonElement jsonValue))
            {
                object? convertedValue =
                    ConvertFromJsonElement(jsonValue);

                if (convertedValue?.GetType() ==
                    setting.DefaultValue?.GetType())
                {
                    value = convertedValue;
                }
            }

            currentSettings[setting] = value;
        }
    }

    private void SaveSettings()
    {
        Dictionary<string, object?> settings = [];

        foreach ((Setting setting, object? value) in currentSettings)
        {
            settings[setting.Key] = value;
        }

        WriteSettingFile(settings);
    }

    private static object? ConvertFromJsonElement(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when value.TryGetInt32(out int i) => i,
            JsonValueKind.Number when value.TryGetDouble(out double d) => d,
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Null => null,
            _ => value.ToString()
        };
    }

    private static Dictionary<string, object?> BuildDefaultSettings()
    {
        Dictionary<string, object?> settings = [];

        foreach (Setting setting in SettingDefs.All)
        {
            settings[setting.Key] = setting.DefaultValue;
        }

        return settings;
    }

    private static void WriteSettingFile(
        Dictionary<string, object?> settings)
    {
        string json = JsonSerializer.Serialize(
            settings,
            JsonOptions);

        File.WriteAllText(
            GetSettingFilePath(),
            json);
    }
}