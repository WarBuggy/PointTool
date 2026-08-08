namespace PointTool.Settings;

public static class SettingDefs
{
    public static readonly Setting<bool> ShowNoClassesMessage =
        new(
            "showNoClassesMessage",
            true);

    public static readonly IReadOnlyList<Setting> All =
    [
        ShowNoClassesMessage
    ];
}