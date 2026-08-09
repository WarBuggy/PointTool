namespace PointTool.Panes.Right;

public abstract class DisplayPane : TableLayoutPanel
{
    public string Title { get; }

    protected readonly TableLayoutPanel DisplayTable =
        new();

    private readonly List<Button> buttons = [];

    public IReadOnlyList<Button> Buttons => buttons;

    protected DisplayPane(string title)
    {
        Title = title;

        SuspendLayout();

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Dock = DockStyle.Top;

        //
        // DisplayTable
        //
        DisplayTable.AutoSize = true;
        DisplayTable.AutoSizeMode =
            AutoSizeMode.GrowAndShrink;
        DisplayTable.Dock = DockStyle.Top;

        Controls.Add(
            DisplayTable,
            0,
            0);

        ResumeLayout(false);
    }
}