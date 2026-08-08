using PointTool.Modals;

namespace PointTool.Utilities;

public static class UIUtilities
{
    public static TableLayoutPanel CreateTable(
       int columns,
       int rows,
       DockStyle dock = DockStyle.Fill,
       bool autoSize = true)
    {
        TableLayoutPanel table = new();

        table.SuspendLayout();

        table.ColumnCount = columns;
        table.RowCount = rows;

        table.Dock = dock;

        table.AutoSize = autoSize;

        if (autoSize)
        {
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        table.Margin = Padding.Empty;
        table.Padding = Padding.Empty;

        for (int i = 0; i < columns; i++)
        {
            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100f / columns));
        }

        for (int i = 0; i < rows; i++)
        {
            table.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));
        }

        table.ResumeLayout(false);

        return table;
    }

    public static void ShowMessage(
        string title,
        string message,
        IWin32Window? owner = null)
    {
        using CustomMessageModal modal = new(
            title,
            message);

        modal.AddButton(
            "OK",
            DialogResult.OK,
            isDefault: true,
            isCancel: true);

        if (owner == null)
        {
            modal.ShowDialog();
        }
        else
        {
            modal.ShowDialog(owner);
        }
    }
}