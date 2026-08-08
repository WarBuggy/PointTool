using PointTool.Utilities;

namespace PointTool.Modals;

public abstract class BaseModal : Form
{
    protected readonly TableLayoutPanel RootTable = new();

    protected virtual int RootTableColumns => 1;
    protected virtual int RootTableRows => 1;

    protected BaseModal()
    {
        InitializeWindow();
        InitializeRootTable();
    }

    private void InitializeWindow()
    {
        StartPosition = FormStartPosition.CenterParent;

        FormBorderStyle = FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;

        ShowInTaskbar = false;
    }

    private void InitializeRootTable()
    {
        RootTable.Dock = DockStyle.Fill;
        RootTable.Padding = new Padding(UiConstants.Margin);

        Controls.Add(RootTable);
    }

    protected void FinalizeLayout()
    {
        RootTable.SuspendLayout();

        RootTable.ColumnStyles.Clear();
        RootTable.RowStyles.Clear();

        RootTable.ColumnCount = RootTableColumns;
        RootTable.RowCount = RootTableRows;

        for (int i = 0; i < RootTableColumns; i++)
        {
            RootTable.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100f / RootTableColumns));
        }

        for (int i = 0; i < RootTableRows; i++)
        {
            RootTable.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));
        }

        RootTable.ResumeLayout(false);
        RootTable.PerformLayout();
    }

    protected void SetRootTableRowStyles(params RowStyle[] rowStyles)
    {
        if (rowStyles.Length != RootTableRows)
        {
            throw new InvalidOperationException(
                $"Expected {RootTableRows} row styles but received {rowStyles.Length}.");
        }

        RootTable.RowStyles.Clear();

        foreach (RowStyle rowStyle in rowStyles)
        {
            RootTable.RowStyles.Add(rowStyle);
        }
    }
}