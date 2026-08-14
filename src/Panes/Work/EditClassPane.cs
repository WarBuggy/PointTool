using PointTool.InputGroups;
using PointTool.Managers;
using PointTool.Modals;
using PointTool.Utilities;
using PointTool.Validation;
using static PointTool.Managers.ClassManager;

namespace PointTool.Panes.Work;

public class EditClassPane : WorkPane
{
    private const string NameKey = "Name";

    private const string DescriptionKey = "Description";

    private readonly InputTextBoxGroup nameGroup =
        new(
            labelText: NameKey,
            isRequired: true);

    private readonly InputTextBoxGroup descriptionGroup =
        new(
            labelText: DescriptionKey);

    private readonly Button saveButton = new();

    public override string Title =>
        $"Edit class {ClassName}";

    private readonly ClassManager classManager;

    private string className = string.Empty;

    public string UpdatedClassName { get; private set; } = string.Empty;

    public string ClassName
    {
        get => className;
        set => className = value;
    }

    public event EventHandler? ClassUpdated;

    public EditClassPane(ClassManager classManager)
    {
        this.classManager = classManager;

        AddInputGroup(
            nameGroup,
            row: 0,
            logicalColumn: 0);

        AddInputGroup(
            descriptionGroup,
            row: 0,
            logicalColumn: 1);

        saveButton.Text = "Save";
        saveButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        saveButton.Click += SaveButton_Click;

        ButtonList.Add(saveButton);
    }

    public void LoadClass(
        string name,
        string description)
    {
        ClassName = name;

        nameGroup.Text = name;
        descriptionGroup.Text = description;
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

    private void SaveButton_Click(object? sender, EventArgs e)
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

        UpdateClassResult updateResult = null!;

        WaitModal.Instance.Show(() =>
        {
            updateResult =
                classManager.UpdateClass(
                    ClassName,
                    name,
                    description);
        });

        Form? owner = ContentTable.FindForm();

        UIUtilities.ShowMessage(
            updateResult.Success
                ? "Class Updated"
                : "Unable to Update Class",
            updateResult.Message,
            owner);

        if (updateResult.Success)
        {
            UpdatedClassName = name;

            ClassUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}