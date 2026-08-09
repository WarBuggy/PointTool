using PointTool.InputGroups;
using PointTool.Managers;
using PointTool.Utilities;
using PointTool.Validation;

namespace PointTool.Panes.Work;

public class UploadScorePane : WorkPane
{
    private readonly QuizManager quizManager;

    private const string NameKey = "Name";

    private const string DescriptionKey = "Description";

    private readonly InputTextBoxGroup nameGroup =
        new(
            labelText: NameKey,
            isRequired: true);

    private readonly InputTextBoxGroup descriptionGroup =
        new(
            labelText: DescriptionKey);

    private readonly InputFileGroup scoreFileGroup =
        new(
            labelText: "Score File",
            isRequired: true);

    private readonly Button uploadButton = new();

    public override string Title =>
        $"Upload a quiz scores for class {ClassName}";

    private string className = string.Empty;

    public string ClassName
    {
        get => className;
        set => className = value;
    }

    public event EventHandler? QuizCreated;

    public UploadScorePane(QuizManager quizManager)
    {
        this.quizManager = quizManager;

        AddInputGroup(
            nameGroup,
            row: 0,
            logicalColumn: 0);

        AddInputGroup(
            descriptionGroup,
            row: 0,
            logicalColumn: 1);

        //
        // scoreFileGroup
        //
        while (ContentTable.RowCount <= 1)
        {
            ContentTable.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            ContentTable.RowCount++;
        }

        ContentTable.Controls.Add(
            scoreFileGroup.InputControl,
            0,
            1);

        ContentTable.SetColumnSpan(
            scoreFileGroup.InputControl,
            ContentTable.ColumnCount);

        uploadButton.Text = "Upload";
        uploadButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        uploadButton.Click += UploadButton_Click;

        ButtonList.Add(uploadButton);
    }

    protected override ValidationResult TryGetInputData()
    {
        ValidationResult result = new();

        NameValidator.TryNormalizeName(
            nameGroup.Text,
            out string normalizedName,
            out List<string> nameErrors);

        foreach (string error in nameErrors)
        {
            result.AddError(
                error,
                nameGroup);
        }

        string description =
            descriptionGroup.Text.Trim();

        if (description.Length > 255)
        {
            result.AddError(
                "Description cannot be longer than 255 characters.",
                descriptionGroup);
        }

        if (!result.IsValid)
        {
            return result;
        }

        result.AddValue(
            NameKey,
            normalizedName);

        result.AddValue(
            DescriptionKey,
            description);

        return result;
    }

    private void UploadButton_Click(
        object? sender,
        EventArgs e)
    {
        ValidationResult result = Validate();

        if (!result.IsValid)
        {
            return;
        }

        string name =
            result.GetValue<string>(NameKey);

        string description =
            result.GetValue<string>(DescriptionKey);

        List<QuizScore> scores = CreateTestScores();

        Form? owner = ContentTable.FindForm();

        try
        {
            quizManager.CreateQuizScoreFile(
                ClassName,
                name,
                description,
                scores);

            UIUtilities.ShowMessage(
                "Upload Scores",
                $"Quiz \"{name}\" was uploaded successfully.",
                owner);

            QuizCreated?.Invoke(
                this,
                EventArgs.Empty);
        }
        catch (IOException ex)
        {
            UIUtilities.ShowMessage(
                "Upload Scores",
                ex.Message,
                owner);
        }
    }

    private static List<QuizScore> CreateTestScores()
    {
        string[] students =
        [
            "Alice",
            "Bob",
            "Charlie",
            "Diana",
            "Ethan",
            "Fiona",
            "George",
            "Hannah",
            "Ian",
            "Julia",
            "Kevin",
            "Laura",
            "Michael",
            "Natalie",
            "Oliver",
            "Penelope",
            "Quinn",
            "Rachel",
            "Samuel",
            "Tara",
            "Victor",
            "Wendy",
            "Xavier",
            "Yvonne",
            "Zachary",
            "Aaron",
            "Bella",
            "Caleb",
            "Delilah",
            "Elijah",
            "Grace",
            "Henry",
            "Isla",
            "Jack",
            "Katherine",
            "Liam",
            "Maya",
            "Noah",
            "Olivia",
            "Peter",
        ];

        int minStudentCount = 30;

        Random random = new();

        int studentCount =
            random.Next(
                minStudentCount,
                students.Length + 1);

        return [.. students
            .OrderBy(_ => random.Next())
            .Take(studentCount)
            .Select(student => new QuizScore
            {
                Student = student,
                Score = random.Next(0, 21)
            })];
    }

    public class QuizScore
    {
        public string Student { get; init; } = string.Empty;

        public int Score { get; init; }
    }
}