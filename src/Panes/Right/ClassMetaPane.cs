using System.Reflection;
using PointTool.Managers;
using PointTool.Utilities;

namespace PointTool.Panes.Right;

public class ClassMetaPane : TableLayoutPanel
{
    private readonly Label titleLabel = new();

    private readonly TableLayoutPanel infoTable;

    private readonly PropertyInfo[] classInfoProperties;

    public ClassMetaPane()
    {
        classInfoProperties = GetClassInfoProperties();

        infoTable = UIUtilities.CreateTable(
            columns: 2,
            rows: classInfoProperties.Length);

        InitializeComponent();

        SetVisible(false);
    }

    private static PropertyInfo[] GetClassInfoProperties()
    {
        return typeof(ClassManager.ClassInfo)
            .GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public);
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        AutoSize = false;

        RowStyles.Clear();
        RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));

        //
        // titleLabel
        //
        titleLabel.Text = "Class Info";
        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        titleLabel.Padding = new Padding(4);
        titleLabel.Font = new Font(
            titleLabel.Font.FontFamily,
            11f,
            FontStyle.Bold);

        //
        // infoTable
        //
        infoTable.AutoSize = true;
        infoTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        infoTable.Dock = DockStyle.Top;

        infoTable.ColumnStyles.Clear();
        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        for (int row = 0; row < classInfoProperties.Length; row++)
        {
            PropertyInfo property =
                classInfoProperties[row];

            AddLabel(
                property.Name,
                0,
                row);

            AddValue(
                string.Empty,
                1,
                row);
        }

        Controls.Add(
            titleLabel,
            0,
            0);

        Controls.Add(
            infoTable,
            0,
            1);
    }

    public void SetData(
        ClassManager.ClassInfo classInfo)
    {
        for (int row = 0; row < classInfoProperties.Length; row++)
        {
            PropertyInfo property =
                classInfoProperties[row];

            object? value =
                property.GetValue(classInfo);

            SetValue(
                row,
                value?.ToString() ?? string.Empty);
        }
    }

    private void AddLabel(
        string text,
        int column,
        int row)
    {
        Label label = new()
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Padding = new Padding(4)
        };

        infoTable.Controls.Add(
            label,
            column,
            row);
    }

    private void AddValue(
        string text,
        int column,
        int row)
    {
        Label label = new()
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Padding = new Padding(4)
        };

        infoTable.Controls.Add(
            label,
            column,
            row);
    }

    private void SetValue(
        int row,
        string value)
    {
        Control? control =
            infoTable.GetControlFromPosition(1, row);

        if (control is Label label)
        {
            label.Text = value;
        }
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
    }
}