using PointTool.Utilities;

namespace PointTool.Panes.Left;

public class LeftActionArea : TableLayoutPanel
{
    private const int RowCountValue = 2;
    private const int ColumnCountValue = 2;

    public LeftActionArea()
    {
        SuspendLayout();

        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Dock = DockStyle.Top;
        Margin = Padding.Empty;

        RowCount = RowCountValue;
        ColumnCount = ColumnCountValue;

        for (int i = 0; i < ColumnCountValue; i++)
        {
            ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50f));
        }

        for (int i = 0; i < RowCountValue; i++)
        {
            RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    UiConstants.LeftActionAreaButtonHeight));
        }

        ResumeLayout(false);

        ClearAll();
    }

    public void ShowButtons(
        IReadOnlyList<Button> buttons)
    {
        Controls.Clear();

        int maxButtonCount = RowCountValue * ColumnCountValue;
        for (int i = 0; i < maxButtonCount; i++)
        {
            Control control;

            if (i < buttons.Count)
            {
                control = buttons[i];
            }
            else
            {
                control = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = Padding.Empty
                };
            }

            Controls.Add(
                control,
                i % ColumnCountValue,
                i / ColumnCountValue);
        }
    }

    public void ClearAll()
    {
        ShowButtons([]);
    }
}