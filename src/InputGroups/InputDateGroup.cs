using PointTool.Utilities;

namespace PointTool.InputGroups;

public class InputDateGroup : InputGroup
{
    private readonly DateTimePicker datePicker = new();

    public DateTime Date
    {
        get => datePicker.Value.Date;
        set => datePicker.Value = value.Date;
    }

    public InputDateGroup(
        string labelText,
        bool isRequired = false)
        : base(
            labelText,
            isRequired)
    {
        datePicker.Format =
            DateTimePickerFormat.Short;

        datePicker.Width =
            UiConstants.InputLabelColumnWidth;

        InputControl = datePicker;
    }

    public override void ResetInput()
    {
        datePicker.Value = DateTime.Today;
        HasError = false;
    }
}