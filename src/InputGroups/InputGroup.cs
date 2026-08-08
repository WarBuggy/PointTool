using PointTool.Utilities;

namespace PointTool.InputGroups;

public abstract class InputGroup
{
    public Control InputControl { get; protected init; } = null!;
    private readonly TableLayoutPanel labelPanel = new();
    public TableLayoutPanel LabelPanel => labelPanel;
    private readonly Label requiredLabel = new();
    private readonly Label textLabel = new();
    private string labelText;
    private readonly bool isRequired;
    private bool hasError;

    protected InputGroup(
        string labelText,
        bool isRequired = false)
    {
        this.labelText = labelText;

        this.isRequired = isRequired;

        labelPanel.SuspendLayout();

        //
        // labelPanel
        //
        labelPanel.ColumnCount = 2;
        labelPanel.RowCount = 1;
        labelPanel.Height = UiConstants.InputRowHeight;
        labelPanel.Margin = Padding.Empty;

        labelPanel.Width =
            UiConstants.RequiredColumnWidth + UiConstants.InputLabelColumnWidth;

        labelPanel.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                UiConstants.RequiredColumnWidth));

        labelPanel.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                UiConstants.InputLabelColumnWidth));

        //
        // requiredLabel
        //
        requiredLabel.Dock = DockStyle.Fill;
        requiredLabel.TextAlign = ContentAlignment.MiddleLeft;

        //
        // textLabel
        //
        textLabel.Dock = DockStyle.Fill;
        textLabel.TextAlign = ContentAlignment.MiddleLeft;

        //
        // labelPanel
        //
        labelPanel.Controls.Add(requiredLabel, 0, 0);
        labelPanel.Controls.Add(textLabel, 1, 0);

        labelPanel.ResumeLayout(false);
        labelPanel.PerformLayout();

        HasError = false;
        SetLabelTexts();
    }

    public string LabelText
    {
        get => labelText;
        set
        {
            labelText = value;
            SetLabelTexts();
        }
    }

    public bool HasError
    {
        get => hasError;
        set
        {
            hasError = value;

            Color color = value ? UiConstants.ValidationErrorColor : SystemColors.ControlText;

            textLabel.ForeColor = color;
            requiredLabel.ForeColor = color;
        }
    }

    private void SetLabelTexts()
    {
        requiredLabel.Text = isRequired
            ? "(*)"
            : string.Empty;

        textLabel.Text = labelText;
    }

    public void FocusInput()
    {
        InputControl.Focus();
    }
}