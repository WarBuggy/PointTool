namespace PointTool.Settings;

public abstract class Setting(string key)
{
    internal string Key { get; } = key;

    public abstract object? DefaultValue { get; }
}

public sealed class Setting<T>(
    string key,
    T defaultValue) : Setting(key)
{
    public T Default { get; } = defaultValue;

    public override object? DefaultValue => Default;
}