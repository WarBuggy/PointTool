using System.Text.Json;
using System.Text.Json.Serialization;
using PointTool.Utilities;
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
                quizInfo.Scores.Sort(
                    (a, b) => string.Compare(
                        a.Student,
                        b.Student,
                        StringComparison.OrdinalIgnoreCase));

                UpdateQuizStats(quizInfo);
            }

            quizList.Sort(
                (a, b) => b.QuizDate.CompareTo(a.QuizDate));

            UpdateStudentHistory(
                quizList);

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
        DateTime quizDate,
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
            QuizDate = quizDate,
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
                ClassInfoFileName,
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

    public ClassSummary GetClassSummary(string className)
    {
        IReadOnlyList<QuizInfo> quizList =
            GetQuizzes(className);

        ClassSummary summary = new();

        foreach (QuizInfo quizInfo in quizList)
        {
            summary.QuizNames.Add(quizInfo.Name);

            summary.QuizAverageScores.Add(
                quizInfo.Name, quizInfo.Stats.AverageScore);
        }

        Dictionary<string, StudentSummary> students = [];

        foreach (QuizInfo quizInfo in quizList)
        {
            foreach (QuizScore score in quizInfo.Scores)
            {
                if (!students.TryGetValue(
                    score.Student,
                    out StudentSummary? studentSummary))
                {
                    studentSummary = new StudentSummary
                    {
                        Name = score.Student
                    };

                    students.Add(
                        score.Student,
                        studentSummary);
                }

                studentSummary.Scores[quizInfo.Name] =
                    score.Score;
            }
        }

        foreach (StudentSummary student in students.Values)
        {
            int totalScore = 0;
            int quizzesTaken = 0;

            foreach (QuizInfo quizInfo in quizList)
            {
                if (student.Scores.TryGetValue(
                    quizInfo.Name,
                    out int score))
                {
                    totalScore += score;
                    quizzesTaken++;
                }
            }

            student.TotalScore = totalScore;
            student.QuizzesTaken = quizzesTaken;

            summary.Students.Add(
                student);
        }

        summary.Students.Sort(
            (a, b) =>
            {
                int result =
                    b.TotalScore.CompareTo(
                        a.TotalScore);

                if (result != 0)
                {
                    return result;
                }

                return string.Compare(
                    a.Name,
                    b.Name,
                    StringComparison.OrdinalIgnoreCase);
            });

        return summary;
    }

    private static void UpdateStudentHistory(
        List<QuizInfo> quizList)
    {
        HashSet<string> previousStudents = [];

        for (int index = quizList.Count - 1;
             index >= 0;
             index--)
        {
            QuizInfo quizInfo =
                quizList[index];

            quizInfo.Stats.PreviousStudents.Clear();
            quizInfo.Stats.NewStudents.Clear();

            foreach (string student in previousStudents)
            {
                quizInfo.Stats.PreviousStudents.Add(
                    student);
            }

            foreach (QuizScore score in quizInfo.Scores)
            {
                if (!previousStudents.Contains(
                    score.Student))
                {
                    quizInfo.Stats.NewStudents.Add(
                        score.Student);
                }
            }

            foreach (QuizScore score in quizInfo.Scores)
            {
                previousStudents.Add(
                    score.Student);
            }
        }
    }

    public void DeleteQuiz(string className, string quizName)
    {
        string classDirectory = Path.Combine(
            PathManager.GetDataDirectory(),
            className);

        string filePath = Path.Combine(
            classDirectory, $"{quizName}.json");

        File.Delete(filePath);

        if (quizzes.TryGetValue(className, out List<QuizInfo>? quizList))
        {
            quizList.RemoveAll(quiz =>
                quiz.Name.Equals(
                    quizName,
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    public void UpdateQuiz(string className, string currentName,
        string name, string description, DateTime quizDate)
    {
        QuizInfo? quizInfo =
            GetQuiz(className, currentName);

        if (quizInfo is null)
        {
            throw new IOException(
                $"Quiz \"{currentName}\" was not found for class \"{className}\".");
        }

        if (!currentName.Equals(
            name, StringComparison.OrdinalIgnoreCase))
        {
            QuizInfo? existingQuiz =
                GetQuiz(className, name);

            if (existingQuiz is not null)
            {
                throw new IOException(
                    $"Quiz \"{name}\" already exists for class \"{className}\".");
            }
        }

        string classDirectory = Path.Combine(
            PathManager.GetDataDirectory(),
            className);

        string currentFilePath = Path.Combine(
            classDirectory,
            $"{currentName}.json");

        string newFilePath = Path.Combine(
            classDirectory,
            $"{name}.json");

        QuizInfo updatedQuizInfo = new()
        {
            Name = name,
            Description = description,
            QuizDate = quizDate,
            Scores = quizInfo.Scores,
        };

        string json = JsonSerializer.Serialize(
            updatedQuizInfo,
            JsonOptions.Options);

        File.WriteAllText(
            newFilePath,
            json);

        if (!currentFilePath.Equals(
            newFilePath,
            StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(currentFilePath);
        }
    }

    public void UpdateQuizScores(string className, string quizName)
    {
        QuizInfo? quizInfo = GetQuiz(className, quizName);

        if (quizInfo is null)
        {
            throw new IOException(
                $"Quiz \"{quizName}\" was not found for class \"{className}\".");
        }

        DeleteQuiz(className, quizName);

        var scores = CreateTestScores();

        CreateQuizScoreFile(
            className,
            quizInfo.Name,
            quizInfo.Description,
            quizInfo.QuizDate,
            scores);
    }

    public class QuizInfo
    {
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public List<QuizScore> Scores { get; init; } = [];

        public DateTime QuizDate { get; init; }

        [JsonIgnore]
        public QuizStats Stats { get; set; } = new();
    }

    public class QuizStats
    {
        public int StudentCount { get; init; }

        public int AverageScore { get; init; }

        public HashSet<string> PreviousStudents { get; init; } = [];

        public HashSet<string> NewStudents { get; init; } = [];
    }
}