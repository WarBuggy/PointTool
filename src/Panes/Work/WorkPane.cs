using PointTool.Utilities;
using PointTool.InputGroups;
using PointTool.Validation;
using PointTool.Modals;

namespace PointTool.Panes.Work;

public abstract class WorkPane
{
    public virtual string Title =>
        "Untitled Work Pane";

    public TableLayoutPanel ContentTable { get; } =
        UIUtilities.CreateTable(columns: 1, rows: 1);

    protected List<Button> ButtonList { get; } = [];

    public IReadOnlyList<Button> Buttons => ButtonList;

    protected List<InputGroup> InputGroupList { get; } = [];

    protected WorkPane()
    {
        ContentTable.AutoSize = true;
        ContentTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        ContentTable.Dock = DockStyle.Top;
    }

    protected void AddInputGroup(InputGroup inputGroup,
       int row, int logicalColumn)
    {
        while (ContentTable.RowCount <= row)
        {
            ContentTable.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            ContentTable.RowCount++;
        }

        int labelColumn = logicalColumn * 2;
        int inputColumn = labelColumn + 1;

        while (ContentTable.ColumnCount <= inputColumn)
        {
            ContentTable.ColumnStyles.Add(
                new ColumnStyle(SizeType.AutoSize));

            ContentTable.ColumnStyles.Add(
               new ColumnStyle(SizeType.AutoSize));

            ContentTable.ColumnCount += 2;
        }

        ContentTable.Controls.Add(
            inputGroup.LabelPanel,
            labelColumn,
            row);

        ContentTable.Controls.Add(
            inputGroup.InputControl,
            inputColumn,
            row);

        InputGroupList.Add(inputGroup);
    }

    protected void ResetValidation()
    {
        foreach (InputGroup inputGroup in InputGroupList)
        {
            inputGroup.HasError = false;
        }
    }

    protected abstract ValidationResult TryGetInputData();


    public ValidationResult Validate()
    {
        ResetValidation();

        ValidationResult result = TryGetInputData();

        foreach ((_, InputGroup inputGroup) in result.Errors)
        {
            inputGroup.HasError = true;
        }

        if (!result.IsValid)
        {
            ShowValidationErrors(result);
        }

        return result;
    }

    protected void ShowValidationErrors(ValidationResult result)
    {
        using CustomMessageModal modal = new(
            "Validation Error",
            string.Join(
                Environment.NewLine,
                result.Errors.Select(x => x.Message)));

        modal.AddButton(
            "OK",
            DialogResult.OK,
            isDefault: true,
            isCancel: true);

        Form? owner = ContentTable.FindForm();

        if (owner == null)
        {
            modal.ShowDialog();
        }
        else
        {
            modal.ShowDialog(owner);
        }

        result.Errors[0].InputGroup.FocusInput();
    }

    protected void ResetInputs()
    {
        foreach (InputGroup inputGroup in InputGroupList)
        {
            inputGroup.ResetInput();
        }
    }
}