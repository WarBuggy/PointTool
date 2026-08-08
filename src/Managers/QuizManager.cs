using System.Text.Json;
using PointTool.Utilities;
using PointTool.Validation;
using static PointTool.Panes.Work.UploadScorePane;

namespace PointTool.Managers;

public class QuizManager(ClassManager classManager)
{
    private readonly ClassManager classManager = classManager;

    private readonly Dictionary<string, List<QuizInfo>> quizzes = [];

    public IReadOnlyDictionary<string, List<QuizInfo>> Quizzes =>
        quizzes;

    public void Refresh()
    {
        quizzes.Clear();

        foreach (string className in classManager.Classes.Keys)
        {
            string classDirectory = Path.Combine(
                PathManager.GetDataDirectory(),
                className);

            List<QuizInfo> quizList = [];

            if (Directory.Exists(classDirectory))
            {
                foreach (string file in Directory.GetFiles(
                    classDirectory,
                    "*.json"))
                {
                    string fileName =
                        Path.GetFileName(file);

                    if (fileName.Equals(
                        ClassManager.ClassInfoFileName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string quizName =
                        Path.GetFileNameWithoutExtension(file);

                    if (!NameValidator.TryNormalizeName(
                        quizName,
                        out _,
                        out _))
                    {
                        continue;
                    }

                    try
                    {
                        string json = File.ReadAllText(file);

                        QuizInfo? quizInfo =
                            JsonSerializer.Deserialize<QuizInfo>(
                                json,
                                JsonOptions.Options);

                        if (quizInfo is not null)
                        {
                            quizList.Add(quizInfo);
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore invalid quiz files for now.
                    }
                    catch (IOException)
                    {
                        // Ignore unreadable files for now.
                    }
                }
            }

            quizList.Sort(
                (a, b) => string.Compare(
                    a.Name,
                    b.Name,
                    StringComparison.OrdinalIgnoreCase));

            quizzes.Add(
                className,
                quizList);
        }
    }

    public IReadOnlyList<QuizInfo> GetQuizzes(
    string className)
    {
        if (quizzes.TryGetValue(
            className,
            out List<QuizInfo>? quizList))
        {
            return quizList;
        }

        return [];
    }

    public QuizInfo CreateQuizScoreFile(
        string className,
        string name,
        string description,
        List<QuizScore> scores)
    {
        if (!quizzes.TryGetValue(
            className, out List<QuizInfo>? quizList))
        {
            quizList = [];
        }

        bool quizExists = quizList.Any(quiz => quiz.Name.Equals(
            name, StringComparison.OrdinalIgnoreCase));

        if (quizExists)
        {
            throw new IOException(
                $"Quiz \"{name}\" already exists for class \"{className}\".");
        }

        QuizInfo quizInfo = new()
        {
            Name = name,
            Description = description,
            Scores = scores
        };

        string classDirectory = Path.Combine(
            PathManager.GetDataDirectory(),
            className);

        string filePath = Path.Combine(
            classDirectory,
            $"{name}.json");

        string json = JsonSerializer.Serialize(
            quizInfo,
            JsonOptions.Options);

        File.WriteAllText(
            filePath,
            json);

        return quizInfo;
    }

    public class QuizInfo
    {
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public List<QuizScore> Scores { get; init; } = [];
    }
}