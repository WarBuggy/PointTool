using PointTool.InputGroups;
using PointTool.Utilities;
using PointTool.Validation;

namespace PointTool.Panes.Work;

public class UploadScorePane : WorkPane
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

    public UploadScorePane()
    {
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

        // TODO:
        // Upload the scores.

        Form? owner = ContentTable.FindForm();

        UIUtilities.ShowMessage(
            "Upload Scores",
            "Quiz score upload is not implemented yet.",
            owner);
    }
}