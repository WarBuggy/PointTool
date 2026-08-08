using System.Text.Json;
using PointTool.Validation;

namespace PointTool.Managers;

public class ClassManager
{
    public const string ClassInfoFileName = "classInfo.json";

    private readonly HashSet<string> classes = [];

    public IReadOnlyCollection<string> Classes => classes;

    readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public ClassManager()
    {
    }

    public void Refresh()
    {
        classes.Clear();

        string dataDirectory = PathManager.GetDataDirectory();

        foreach (string directory in Directory.GetDirectories(dataDirectory))
        {
            string folderName = Path.GetFileName(directory);

            if (!NameValidator.TryNormalizeName(
                    folderName,
                    out string normalizedName,
                    out _))
            {
                continue;
            }

            classes.Add(normalizedName);
        }
    }

    public CreateClassResult CreateClass(
        string name,
        string description)
    {
        if (Classes.Contains(name))
        {
            return new CreateClassResult
            {
                Success = false,
                Message = $"Class \"{name}\" already exists."
            };
        }

        try
        {
            string classDirectory = Path.Combine(
                PathManager.GetDataDirectory(),
                name);

            Directory.CreateDirectory(classDirectory);

            ClassInfo classInfo = new()
            {
                Description = description
            };

            string json = JsonSerializer.Serialize(
                classInfo,
                JsonOptions);

            File.WriteAllText(
                Path.Combine(
                    classDirectory,
                    ClassInfoFileName),
                json);

            Refresh();

            return new CreateClassResult
            {
                Success = true,
                Message = $"Class \"{name}\" was created successfully."
            };
        }
        catch (Exception ex)
        {
            return new CreateClassResult
            {
                Success = false,
                Message = $"Failed to create class \"{name}\".\n\n{ex.Message}"
            };
        }
    }

    public class ClassInfo
    {
        public string Description { get; init; } = string.Empty;
    }

    public class CreateClassResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;
    }
}