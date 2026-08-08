using PointTool.Utilities;

namespace PointTool.Panes.Work;

public class WorkArea : TableLayoutPanel
{
    private const int BUTTON_COLUMN_NUM = 10;

    private readonly Label titleLabel = new();

    private readonly TableLayoutPanel contentTable = new();

    private readonly TableLayoutPanel buttonTable =
          UIUtilities.CreateTable(columns: BUTTON_COLUMN_NUM, rows: 1);

    public WorkArea()
    {
        SuspendLayout();

        Dock = DockStyle.Fill;
        BorderStyle = BorderStyle.FixedSingle;
        Margin = Padding.Empty;

        ColumnCount = 1;
        RowCount = 3;

        ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));

        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        //
        // titleLabel
        //
        titleLabel.Font = new Font(
            titleLabel.Font.FontFamily,
            UiConstants.WorkAreaTitleFontSize,
            FontStyle.Bold);
        titleLabel.Padding = new Padding(16, 8, 16, 8);
        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Margin = Padding.Empty;

        //
        // contentTable
        //
        contentTable.ColumnCount = 2;
        contentTable.RowCount = 1;

        contentTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        contentTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        contentTable.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));

        contentTable.Dock = DockStyle.Fill;
        contentTable.Padding = new Padding(16, 8, 16, 8);
        contentTable.Margin = Padding.Empty;

        //
        // buttonTable
        //
        buttonTable.Dock = DockStyle.Fill;
        buttonTable.Padding = new Padding(16, 8, 16, 8);

        for (int i = 0; i < 10; i++)
        {
            buttonTable.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 10f));
        }

        //
        // WorkArea
        //
        Controls.Add(titleLabel, 0, 0);
        Controls.Add(contentTable, 0, 1);
        Controls.Add(buttonTable, 0, 2);

        ResumeLayout(false);
    }

    public void ShowPane(WorkPane pane)
    {
        //
        // Title
        //
        titleLabel.Text = pane.Title;

        //
        // Content
        //
        contentTable.Controls.Clear();

        contentTable.Controls.Add(
            pane.ContentTable,
            0,
            0);

        //
        // Buttons
        //
        buttonTable.Controls.Clear();

        for (int i = 0;
             i < pane.Buttons.Count && i < 10;
             i++)
        {
            Button button = pane.Buttons[i];

            button.Dock = DockStyle.Fill;

            buttonTable.Controls.Add(
                button,
                i,
                0);
        }
    }
}