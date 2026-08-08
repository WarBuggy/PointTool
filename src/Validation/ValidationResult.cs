using PointTool.InputGroups;

namespace PointTool.Validation;

public class ValidationResult
{
    public Dictionary<string, object?> Values { get; } = [];

    public List<(string Message, InputGroup InputGroup)> Errors { get; } = [];

    public bool IsValid => Errors.Count == 0;

    public void AddValue(
        string name,
        object? value)
    {
        Values[name] = value;
    }

    public void AddError(
        string message,
        InputGroup inputGroup)
    {
        Errors.Add((message, inputGroup));
    }

    public T GetValue<T>(string name)
    {
        return (T)Values[name]!;
    }
}