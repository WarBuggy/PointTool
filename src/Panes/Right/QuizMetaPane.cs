using System.Reflection;
using PointTool.Utilities;
using static PointTool.Managers.QuizManager;

namespace PointTool.Panes.Right;

public class QuizMetaPane : TableLayoutPanel
{
    private readonly Label titleLabel = new();

    private readonly TableLayoutPanel infoTable;

    private readonly PropertyInfo[] quizInfoProperties;

    private static readonly HashSet<string> ExcludedProperties =
    [
        nameof(QuizInfo.Description),
        nameof(QuizInfo.Scores),
        nameof(QuizInfo.Stats)
    ];

    private const string StudentCountLabel = "Students";
    private const string AverageScoreLabel = "Average Score";

    public QuizMetaPane()
    {
        quizInfoProperties = GetQuizInfoProperties();

        infoTable = UIUtilities.CreateTable(
            columns: 2,
            rows: quizInfoProperties.Length + 2);

        InitializeComponent();

        SetVisible(false);
    }

    private static PropertyInfo[] GetQuizInfoProperties()
    {
        return [.. typeof(QuizInfo)
            .GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public)
            .Where(property =>
                !ExcludedProperties.Contains(property.Name))];
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
        titleLabel.Text = "Quiz Info";
        titleLabel.AutoSize = true;
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        titleLabel.Padding = new Padding(4);
        titleLabel.Font = new Font(
            titleLabel.Font.FontFamily,
            11f,
            FontStyle.Bold);

        SetupInfoTable();

        Controls.Add(
            titleLabel,
            0,
            0);

        Controls.Add(
            infoTable,
            0,
            1);
    }

    private void SetupInfoTable()
    {
        infoTable.AutoSize = true;
        infoTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        infoTable.Dock = DockStyle.Top;

        infoTable.ColumnStyles.Clear();
        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        for (int row = 0; row < quizInfoProperties.Length; row++)
        {
            PropertyInfo property =
                quizInfoProperties[row];

            AddLabel(
                property.Name,
                0,
                row);

            AddValue(
                string.Empty,
                1,
                row);
        }

        int studentCountRow =
            quizInfoProperties.Length;

        int averageScoreRow =
            quizInfoProperties.Length + 1;

        AddLabel(
            StudentCountLabel,
            0,
            studentCountRow);

        AddValue(
            string.Empty,
            1,
            studentCountRow);

        AddLabel(
            AverageScoreLabel,
            0,
            averageScoreRow);

        AddValue(
            string.Empty,
            1,
            averageScoreRow);
    }

    public void SetData(QuizInfo quizInfo)
    {
        for (int row = 0; row < quizInfoProperties.Length; row++)
        {
            PropertyInfo property =
                quizInfoProperties[row];

            object? value =
                property.GetValue(quizInfo);

            SetValue(
                row,
                value?.ToString() ?? string.Empty);
        }

        int studentCountRow =
            quizInfoProperties.Length;

        int averageScoreRow =
            quizInfoProperties.Length + 1;

        SetValue(
            studentCountRow,
            quizInfo.Stats.StudentCount.ToString());

        SetValue(
            averageScoreRow,
            quizInfo.Stats.AverageScore.ToString());
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