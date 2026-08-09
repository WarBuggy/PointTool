using PointTool.Utilities;

namespace PointTool.Panes.Right;

public class DetailArea : TableLayoutPanel
{
    private const int DividerRow = 0;
    private const int TitleRow = 1;
    private const int ContentRow = 2;
    private const int ButtonRow = 3;

    private readonly Panel contentArea = new();

    private const int ButtonCount = 4;

    private readonly TableLayoutPanel buttonArea =
        UIUtilities.CreateTable(
            columns: ButtonCount + 1,
            rows: 1);

    private readonly Label titleLabel = new();

    public DetailArea()
    {
        SuspendLayout();

        AutoSize = false;
        Dock = DockStyle.Fill;

        RowCount = 4;
        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));
        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));
        RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));
        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        GroupBox divider = new();
        divider.Text = string.Empty;
        divider.Dock = DockStyle.Fill;
        divider.Height = 2;
        divider.Margin = new Padding(4, 4, 4, 4);

        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        titleLabel.Padding = new Padding(4);
        titleLabel.Font = new Font(
            titleLabel.Font.FontFamily,
            11f,
            FontStyle.Bold);
        titleLabel.Text = "Class Summary";

        //
        // contentArea
        //
        contentArea.AutoSize = false;
        contentArea.AutoScroll = true;
        contentArea.Dock = DockStyle.Fill;

        //
        // buttonArea
        //
        buttonArea.AutoSize = false;
        buttonArea.Dock = DockStyle.Fill;
        buttonArea.Height =
            UiConstants.LeftActionAreaButtonHeight;

        buttonArea.ColumnStyles.Clear();

        for (int column = 0; column < ButtonCount; column++)
        {
            buttonArea.ColumnStyles.Add(
                new ColumnStyle(SizeType.AutoSize));
        }

        buttonArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        Controls.Add(
            divider,
            0,
            DividerRow);

        Controls.Add(
            titleLabel,
            0,
            TitleRow);

        Controls.Add(
            contentArea,
            0,
            ContentRow);

        Controls.Add(
            buttonArea,
            0,
            ButtonRow);

        ResumeLayout(false);
    }

    public void ShowDisplayPane(DisplayPane displayPane)
    {
        Clear();

        titleLabel.Text = displayPane.Title;

        contentArea.Controls.Add(
            displayPane);

        for (int i = 0; i < displayPane.Buttons.Count; i++)
        {
            buttonArea.Controls.Add(
                displayPane.Buttons[i],
                i,
                0);
        }
    }

    public void Clear()
    {
        titleLabel.Text = string.Empty;
        contentArea.Controls.Clear();
        buttonArea.Controls.Clear();
    }
}