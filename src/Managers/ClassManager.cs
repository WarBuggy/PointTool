using System.Text.Json;
using PointTool.Utilities;
using PointTool.Validation;

namespace PointTool.Managers;

public class ClassManager
{
    public const string ClassInfoFileName = "classInfo.json";

    private readonly Dictionary<string, ClassData> classes = [];

    public IReadOnlyDictionary<string, ClassData> Classes => classes;

    public ClassManager() { }

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

            if (!TryGetClassInfo(
                    directory,
                    out ClassInfo classInfo))
            {
                classInfo = CreateClassInfoFile(
                    directory,
                    folderName,
                    string.Empty);
            }

            ClassData classData = new(classInfo);

            classes.Add(
                normalizedName,
                classData);
        }
    }

    public CreateClassResult CreateClass(
        string name,
        string description)
    {
        if (Classes.ContainsKey(name))
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

            CreateClassInfoFile(classDirectory, name, description);

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

    private ClassInfo CreateClassInfoFile(
        string classDirectory,
        string name,
        string description)
    {
        ClassInfo classInfo = new()
        {
            Name = name,
            Description = description
        };

        string json = JsonSerializer.Serialize(
            classInfo,
            JsonOptions.Options);

        File.WriteAllText(
            Path.Combine(
                classDirectory,
                ClassInfoFileName),
            json);

        return classInfo;
    }

    public bool TryGetClassInfo(
        string className,
        out ClassInfo classInfo)
    {
        classInfo = new ClassInfo();

        string classDirectory = Path.Combine(
            PathManager.GetDataDirectory(),
            className);

        string filePath = Path.Combine(
            classDirectory,
            ClassInfoFileName);

        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(filePath);

            ClassInfo? loadedClassInfo =
                JsonSerializer.Deserialize<ClassInfo>(
                    json, JsonOptions.Options);

            if (loadedClassInfo is null)
            {
                return false;
            }

            classInfo = loadedClassInfo;

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public void UpdateStats(QuizManager quizManager)
    {
        foreach ((string className, ClassData classData)
            in classes)
        {
            classData.Stats =
                quizManager.GetClassStats(className);
        }
    }

    public UpdateClassResult UpdateClass(
        string currentName, string name, string description)
    {
        if (!currentName.Equals(name, StringComparison.OrdinalIgnoreCase)
            && Classes.ContainsKey(name))
        {
            return new UpdateClassResult
            {
                Success = false,
                Message = $"Class \"{name}\" already exists."
            };
        }

        try
        {
            string dataDirectory =
                PathManager.GetDataDirectory();

            string currentDirectory =
                Path.Combine(
                    dataDirectory,
                    currentName);

            string newDirectory =
                Path.Combine(
                    dataDirectory,
                    name);

            if (!currentName.Equals(
                    name, StringComparison.OrdinalIgnoreCase))
            {
                Directory.Move(currentDirectory, newDirectory);
            }

            CreateClassInfoFile(
                newDirectory,
                name,
                description);

            Refresh();

            return new UpdateClassResult
            {
                Success = true,
                Message = $"Class \"{name}\" was updated successfully."
            };
        }
        catch (Exception ex)
        {
            return new UpdateClassResult
            {
                Success = false,
                Message = $"Failed to update class \"{name}\".\n\n{ex.Message}"
            };
        }
    }

    public class ClassInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    public class CreateClassResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;
    }

    public class ClassStats
    {
        public int QuizCount { get; init; }

        public int StudentCount { get; init; }
    }

    public class ClassData(ClassInfo classInfo)
    {
        public ClassInfo Info { get; } = classInfo;

        public ClassStats Stats { get; set; } = new();
    }

    public class UpdateClassResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;
    }
}