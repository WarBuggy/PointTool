namespace PointTool.Modals;

public sealed class ModalButton
{
    public required string Text { get; init; }

    public required DialogResult Result { get; init; }

    public bool IsDefault { get; init; }

    public bool IsCancel { get; init; }
}
