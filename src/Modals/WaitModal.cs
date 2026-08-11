namespace PointTool.Modals;

public sealed class WaitModal : Form
{
    private static readonly WaitModal instance = new();

    public static WaitModal Instance => instance;

    private readonly Label messageLabel = new();

    private WaitModal()
    {
        SuspendLayout();

        //
        // WaitModal
        //
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        BackColor = SystemColors.ControlDark;

        //
        // messageLabel
        //
        messageLabel.Text = "Please wait...";
        messageLabel.AutoSize = true;
        messageLabel.Padding = new Padding(24, 16, 24, 16);
        messageLabel.ForeColor = SystemColors.ControlLightLight;

        Controls.Add(
            messageLabel);

        ResumeLayout(false);
    }

    public static void Initialize()
    {
        if (instance is not null)
        {
            throw new InvalidOperationException(
                "WaitModal has already been initialized.");
        }
    }

    public void Show(Action action)
    {
        Form? owner = GetMainForm();

        if (owner is not null)
        {
            StartPosition =
                FormStartPosition.CenterParent;

            Show(owner);
            CenterToParent();
        }
        else
        {
            StartPosition =
                FormStartPosition.CenterScreen;

            Show();
        }

        Refresh();

        try
        {
            action();
        }
        finally
        {
            Hide();
        }
    }

    private static Form? GetMainForm()
    {
        return Application.OpenForms
            .Cast<Form>()
            .FirstOrDefault(form => form is MainForm);
    }
}