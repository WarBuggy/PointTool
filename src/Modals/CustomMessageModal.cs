using PointTool.Utilities;

namespace PointTool.Modals;

public class CustomMessageModal : BaseModal
{
    protected override int RootTableRows => 2;

    private readonly Label messageLabel = new();

    private readonly TableLayoutPanel buttonTable =
        UIUtilities.CreateTable(columns: 1, rows: 1, dock: DockStyle.Right);

    private readonly List<ModalButton> buttons = [];

    public CustomMessageModal(string title, Control content)
    {
        Text = title;

        ClientSize = new Size(420, 180);

        InitializeLayout(content);

        FinalizeLayout();

        SetRootTableRowStyles(
            new RowStyle(SizeType.Percent, 100f),
            new RowStyle(SizeType.AutoSize));
    }

    public CustomMessageModal(string title, string message)
        : this(title, CreateMessageLabel(message))
    {
    }

    public void AddButton(
        string text,
        DialogResult result,
        bool isDefault = false,
        bool isCancel = false,
        Action<Button>? action = null)
    {
        buttons.Add(new ModalButton
        {
            Text = text,
            Result = result,
            IsDefault = isDefault,
            IsCancel = isCancel,
            Action = action,
        });
    }

    public new DialogResult ShowDialog()
    {
        if (buttons.Count == 0)
        {
            throw new ArgumentException(
                "CustomMessageModal must contain at least one button.",
                nameof(buttons));
        }

        PrepareButtons();

        return base.ShowDialog();
    }

    public new DialogResult ShowDialog(IWin32Window? owner)
    {
        if (buttons.Count == 0)
        {
            throw new ArgumentException(
                "CustomMessageModal must contain at least one button.",
                nameof(buttons));
        }

        PrepareButtons();

        return base.ShowDialog(owner);
    }

    private void InitializeLayout(Control content)
    {
        RootTable.Controls.Add(content, 0, 0);
        RootTable.Controls.Add(buttonTable, 0, 1);
    }

    private void PrepareButtons()
    {
        if (buttons.Count == 0)
        {
            throw new InvalidOperationException(
                "QuestionModal must contain at least one button.");
        }

        buttonTable.SuspendLayout();

        buttonTable.Controls.Clear();

        buttonTable.ColumnStyles.Clear();
        buttonTable.RowStyles.Clear();

        buttonTable.ColumnCount = buttons.Count;
        buttonTable.RowCount = 1;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttonTable.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100f / buttons.Count));
        }

        buttonTable.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        for (int i = 0; i < buttons.Count; i++)
        {
            ModalButton modalButton = buttons[i];

            Button button = new()
            {
                Text = modalButton.Text,
                DialogResult = modalButton.Result,
                AutoSize = true,
                MinimumSize = new Size(
                    UiConstants.ButtonWidth, UiConstants.ButtonHeight),
            };

            button.Click += (_, _) =>
            {
                modalButton.Action?.Invoke(button);
            };

            buttonTable.Controls.Add(button, i, 0);

            if (modalButton.IsDefault)
            {
                AcceptButton = button;
            }

            if (modalButton.IsCancel)
            {
                CancelButton = button;
            }
        }

        buttonTable.ResumeLayout(false);
    }

    private static Label CreateMessageLabel(string message)
    {
        return new Label
        {
            Text = message,
            AutoSize = true,
            Dock = DockStyle.Fill
        };
    }
}