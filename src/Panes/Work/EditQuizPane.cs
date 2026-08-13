using PointTool.InputGroups;
using PointTool.Managers;
using PointTool.Modals;
using PointTool.Utilities;
using PointTool.Validation;
using static PointTool.Managers.QuizManager;

namespace PointTool.Panes.Work;

public class EditQuizPane : WorkPane
{
    private readonly QuizManager quizManager;

    private const string NameKey = "Name";

    private const string DescriptionKey = "Description";

    private const string QuizDateKey = "QuizDate";

    private readonly InputTextBoxGroup nameGroup =
        new(
            labelText: NameKey,
            isRequired: true);

    private readonly InputTextBoxGroup descriptionGroup =
        new(
            labelText: DescriptionKey);

    private readonly InputDateGroup dateGroup =
        new(
            labelText: "Quiz Date",
            isRequired: true);

    private readonly Button saveButton = new();

    public override string Title =>
        $"Edit quiz \"{quizName}\" for class \"{ClassName}\"";

    private string className = string.Empty;
    private string quizName = string.Empty;

    public string UpdatedQuizName { get; private set; } = string.Empty;

    private QuizInfo? quizInfo;

    public string ClassName
    {
        get => className;
        set => className = value;
    }

    public event EventHandler? QuizUpdated;

    public EditQuizPane(
        QuizManager quizManager)
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

        AddInputGroup(
            dateGroup,
            row: 1,
            logicalColumn: 0);

        saveButton.Text = "Save";
        saveButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        saveButton.Click += SaveButton_Click;

        ButtonList.Add(saveButton);
    }

    public void SetData(QuizInfo quizInfo)
    {
        this.quizInfo = quizInfo;
        quizName = quizInfo.Name;

        nameGroup.Text = quizInfo.Name;
        descriptionGroup.Text = quizInfo.Description;
        dateGroup.Date = quizInfo.QuizDate;
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

        result.AddValue(
            QuizDateKey,
            dateGroup.Date);

        return result;
    }

    private void SaveButton_Click(
        object? sender,
        EventArgs e)
    {
        if (quizInfo is null)
        {
            return;
        }

        ValidationResult result = Validate();

        if (!result.IsValid)
        {
            return;
        }

        string name =
            result.GetValue<string>(NameKey);

        string description =
            result.GetValue<string>(DescriptionKey);

        DateTime quizDate =
            result.GetValue<DateTime>(QuizDateKey);

        Form? owner = ContentTable.FindForm();

        string? errorMessage = null;

        WaitModal.Instance.Show(() =>
        {
            try
            {
                quizManager.UpdateQuiz(
                    ClassName,
                    quizInfo.Name,
                    name,
                    description,
                    quizDate);
            }
            catch (IOException ex)
            {
                errorMessage = ex.Message;
            }
        });

        if (errorMessage is not null)
        {
            UIUtilities.ShowMessage(
                "Edit Quiz",
                errorMessage,
                owner);

            return;
        }

        UIUtilities.ShowMessage(
            "Edit Quiz",
            $"Quiz \"{name}\" was updated successfully.",
            owner);

        UpdatedQuizName = name;

        QuizUpdated?.Invoke(this, EventArgs.Empty);
    }
}