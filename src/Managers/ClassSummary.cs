namespace PointTool.Managers;

public class ClassSummary
{
    public List<string> QuizNames { get; init; } = [];

    public List<StudentSummary> Students { get; init; } = [];

    public Dictionary<string, int> QuizAverageScores { get; } = [];
}

public class StudentSummary
{
    public string Name { get; init; } = string.Empty;

    public int TotalScore { get; set; }

    public int QuizzesTaken { get; set; }

    public Dictionary<string, int> Scores { get; init; } = [];
}