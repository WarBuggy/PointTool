using System.Text.Json;
using PointTool.Utilities;
using PointTool.Validation;
using static PointTool.Managers.ClassManager;
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
            List<QuizInfo> quizList =
                LoadQuizzes(className);

            foreach (QuizInfo quizInfo in quizList)
            {
                UpdateQuizStats(quizInfo);
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

    public IReadOnlyList<QuizInfo> GetQuizzes(string className)
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

    private static List<QuizInfo> LoadQuizzes(
        string className)
    {
        string classDirectory = Path.Combine(
            PathManager.GetDataDirectory(),
            className);

        List<QuizInfo> quizList = [];

        if (!Directory.Exists(classDirectory))
        {
            return quizList;
        }

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

            if (!TryLoadQuizInfo(
                file,
                out QuizInfo? quizInfo))
            {
                continue;
            }

            quizList.Add(quizInfo!);
        }

        return quizList;
    }

    private static bool TryLoadQuizInfo(
        string file,
        out QuizInfo? quizInfo)
    {
        quizInfo = null;

        try
        {
            string json = File.ReadAllText(file);

            quizInfo =
                JsonSerializer.Deserialize<QuizInfo>(
                    json,
                    JsonOptions.Options);

            return quizInfo is not null;
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

    public ClassStats GetClassStats(
        string className)
    {
        IReadOnlyList<QuizInfo> quizList =
            GetQuizzes(className);

        HashSet<string> students = [];

        foreach (QuizInfo quiz in quizList)
        {
            foreach (QuizScore score in quiz.Scores)
            {
                students.Add(score.Student);
            }
        }

        return new ClassStats
        {
            QuizCount = quizList.Count,
            StudentCount = students.Count
        };
    }

    private static void UpdateQuizStats(QuizInfo quizInfo)
    {
        int studentCount =
            quizInfo.Scores.Count;

        int averageScore =
            studentCount > 0
                ? quizInfo.Scores.Sum(score => score.Score) /
                  studentCount
                : 0;

        quizInfo.Stats = new QuizStats
        {
            StudentCount = studentCount,
            AverageScore = averageScore
        };
    }

    public QuizInfo? GetQuiz(string className, string quizName)
    {
        if (!quizzes.TryGetValue(
            className,
            out List<QuizInfo>? quizList))
        {
            return null;
        }

        foreach (QuizInfo quizInfo in quizList)
        {
            if (quizInfo.Name.Equals(
                quizName,
                StringComparison.OrdinalIgnoreCase))
            {
                return quizInfo;
            }
        }

        return null;
    }

    public class QuizInfo
    {
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public List<QuizScore> Scores { get; init; } = [];

        public QuizStats Stats { get; set; } = new();
    }

    public class QuizStats
    {
        public int StudentCount { get; init; }

        public int AverageScore { get; init; }
    }
}