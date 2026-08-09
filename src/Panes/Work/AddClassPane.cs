using PointTool.InputGroups;
using PointTool.Managers;
using PointTool.Utilities;
using PointTool.Validation;
using static PointTool.Managers.ClassManager;

namespace PointTool.Panes.Work;

public class AddClassPane : WorkPane
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

    private readonly Button createButton = new();

    public override string Title =>
        "Create A New Class";

    private readonly ClassManager classManager;

    public event EventHandler? ClassCreated;

    public AddClassPane(ClassManager classManager)
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

        createButton.Text = "Create";
        createButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        createButton.Click += CreateButton_Click;

        ButtonList.Add(createButton);
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

        result.AddValue(NameKey, normalizedName);

        result.AddValue(DescriptionKey, description);

        return result;
    }

    private void CreateButton_Click(
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

        CreateClassResult createResult =
            classManager.CreateClass(
                name,
                description);

        Form? owner = ContentTable.FindForm();

        UIUtilities.ShowMessage(
            createResult.Success
                ? "Class Created"
                : "Unable to Create Class",
            createResult.Message,
            owner);

        if (createResult.Success)
        {
            ClassCreated?.Invoke(this, EventArgs.Empty);
        }
    }
}