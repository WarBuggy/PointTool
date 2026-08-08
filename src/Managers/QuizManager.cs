using PointTool.Validation;

namespace PointTool.Managers;

public class QuizManager(
    ClassManager classManager)
{
    private readonly ClassManager classManager = classManager;

    private readonly Dictionary<string, List<string>> quizzes = [];

    public IReadOnlyDictionary<string, List<string>> Quizzes =>
        quizzes;

    public void Refresh()
    {
        quizzes.Clear();

        foreach (string className in classManager.Classes)
        {
            string classDirectory = Path.Combine(
                PathManager.GetDataDirectory(),
                className);

            List<string> quizList = [];

            if (Directory.Exists(classDirectory))
            {
                foreach (string directory in Directory.GetDirectories(classDirectory))
                {
                    string quizName =
                        Path.GetFileName(directory);

                    if (NameValidator.TryNormalizeName(
                        quizName,
                        out string normalizedQuizName,
                        out _))
                    {
                        quizList.Add(normalizedQuizName);
                    }
                }
            }

            quizList.Sort();

            quizzes.Add(
                className,
                quizList);
        }
    }

    public IReadOnlyList<string> GetQuizzes(
        string className)
    {
        if (quizzes.TryGetValue(
            className,
            out List<string>? quizList))
        {
            return quizList;
        }

        return [];
    }
}