using PointTool.Utilities;

namespace PointTool.InputGroups;

public class InputTextBoxGroup : InputGroup
{
    private readonly TextBox textBox = new();

    public InputTextBoxGroup(string labelText, bool isRequired = false)
        : base(labelText, isRequired)
    {
        InputControl = textBox;

        textBox.Width = UiConstants.TextBoxWidth;
        textBox.Anchor = AnchorStyles.Left;
    }

    public string Text
    {
        get => textBox.Text;
        set => textBox.Text = value;
    }

    public int MaxLength
    {
        get => textBox.MaxLength;
        set => textBox.MaxLength = value;
    }

    public CharacterCasing CharacterCasing
    {
        get => textBox.CharacterCasing;
        set => textBox.CharacterCasing = value;
    }

    public bool ReadOnly
    {
        get => textBox.ReadOnly;
        set => textBox.ReadOnly = value;
    }

    public TextBox TextBox => textBox;

    public override void ResetInput()
    {
        InputControl.Text = string.Empty;
        HasError = false;
    }
}