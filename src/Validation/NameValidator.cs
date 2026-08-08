using System.Text.RegularExpressions;

namespace PointTool.Validation;

public static class NameValidator
{
    private const int MaxLength = 20;
    private const int MinLength = 1;

    private static readonly Regex ValidCharacters =
        new("^[A-Z0-9._]+$", RegexOptions.Compiled);

    private static readonly HashSet<string> ReservedNames =
    [
        "CON",
        "PRN",
        "AUX",
        "NUL",
        "COM1",
        "COM2",
        "COM3",
        "COM4",
        "COM5",
        "COM6",
        "COM7",
        "COM8",
        "COM9",
        "LPT1",
        "LPT2",
        "LPT3",
        "LPT4",
        "LPT5",
        "LPT6",
        "LPT7",
        "LPT8",
        "LPT9"
    ];

    public static bool TryNormalizeName(
        string input,
        out string normalizedName,
        out List<string> errors)
    {
        normalizedName = string.Empty;
        errors = [];

        if (string.IsNullOrWhiteSpace(input))
        {
            errors.Add("Name cannot be empty.");
            return false;
        }

        normalizedName = input.Trim().ToUpperInvariant();

        if (normalizedName.Length < MinLength)
        {
            errors.Add($"Name cannot be shorter than {MinLength} character(s).");
        }

        if (normalizedName.Length > MaxLength)
        {
            errors.Add($"Name cannot be longer than {MaxLength} characters.");
        }

        if (!ValidCharacters.IsMatch(normalizedName))
        {
            errors.Add(
                "Name may only contain letters (A-Z), numbers (0-9), period (.) and underscore (_).");
        }

        if (normalizedName.StartsWith('.') ||
            normalizedName.StartsWith('_'))
        {
            errors.Add(
                "Name cannot begin with a period (.) or underscore (_).");
        }

        if (normalizedName.EndsWith('.') ||
            normalizedName.EndsWith('_'))
        {
            errors.Add(
                "Name cannot end with a period (.) or underscore (_).");
        }

        if (ReservedNames.Contains(normalizedName))
        {
            errors.Add(
                $"\"{normalizedName}\" is a reserved Windows name and cannot be used.");
        }

        return errors.Count == 0;
    }
}