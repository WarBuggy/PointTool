using PointTool.InputGroups;
using PointTool.Managers;
using PointTool.Modals;
using PointTool.Utilities;
using PointTool.Validation;

namespace PointTool.Panes.Work;

public class CheckNamePane : WorkPane
{
    private const string QuizDateKey = "QuizDate";
    private const string ScoresKey = "Scores";

    private readonly InputFileGroup scoreFileGroup =
        new(
            labelText: "Score File",
            isRequired: true);

    private readonly InputDateGroup dateGroup =
        new(
            labelText: "Quiz Date",
            isRequired: true);

    private readonly Button checkButton = new();

    public override string Title =>
        $"Check names for a new quiz for class {ClassName}";

    private string className = string.Empty;

    public string ClassName
    {
        get => className;
        set => className = value;
    }

    private readonly QuizManager quizManager;

    public CheckNamePane(QuizManager quizManager)
    {
        this.quizManager = quizManager;

        AddInputGroup(
            dateGroup,
            row: 0,
            logicalColumn: 0);

        //
        // scoreFileGroup
        //
        while (ContentTable.RowCount <= 0)
        {
            ContentTable.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            ContentTable.RowCount++;
        }

        ContentTable.Controls.Add(
            scoreFileGroup.InputControl,
            2,
            1);

        ContentTable.SetColumnSpan(
            scoreFileGroup.InputControl,
            ContentTable.ColumnCount);

        dateGroup.Date = DateTime.Today;

        checkButton.Text = "Check";
        checkButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        checkButton.Click += CheckButton_Click;

        ButtonList.Add(checkButton);
    }

    protected override ValidationResult TryGetInputData()
    {
        ValidationResult result = new();

        result.AddValue(QuizDateKey, dateGroup.Date);

        List<UploadScorePane.QuizScore> scores =
            UploadScorePane.CreateTestScores();

        result.AddValue(ScoresKey, scores);

        return result;
    }

    private void CheckButton_Click(object? sender, EventArgs e)
    {
        ValidationResult result = Validate();

        if (!result.IsValid)
        {
            return;
        }

        DateTime quizDate =
            result.GetValue<DateTime>(QuizDateKey);

        List<UploadScorePane.QuizScore> scores =
            UploadScorePane.CreateTestScores();

        List<string> newStudents =
            quizManager.FindNewStudents(
                ClassName,
                quizDate,
                scores);

        Form? owner = ContentTable.FindForm();

        if (newStudents.Count == 0)
        {
            UIUtilities.ShowMessage(
                "Check Names",
                "No new student names were found.",
                owner);

            return;
        }

        string studentNames = string.Join(", ", newStudents);

        string namesToCopy = string.Join(
                Environment.NewLine, newStudents);

        CustomMessageModal modal = new(
            "Check Names",
            $"The following {newStudents.Count} student names were not found in the previous quizzes:"
            + Environment.NewLine
            + Environment.NewLine
            + studentNames + ".");

        modal.AddButton(
            "Copy names",
            DialogResult.None,
            action: button =>
            {
                Clipboard.SetText(namesToCopy);

                button.Text = "Copied!";
                button.Enabled = false;

                System.Windows.Forms.Timer timer = new()
                {
                    Interval = 1500
                };

                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    timer.Dispose();

                    button.Text = "Copy names";
                    button.Enabled = true;
                };

                timer.Start();
            }
        );

        modal.AddButton(
            "OK",
            DialogResult.OK,
            isDefault: true,
            isCancel: true);

        modal.ShowDialog(owner);
    }
}