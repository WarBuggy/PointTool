using PointTool.Utilities;
using static PointTool.Managers.QuizManager;

namespace PointTool.Panes.Right;

public class QuizMetaPane : TableLayoutPanel
{
    private readonly Label titleLabel = new();

    private readonly TableLayoutPanel infoTable =
        UIUtilities.CreateTable(
            columns: 4,
            rows: 3);

    private const int NameRow = 0;
    private const int StatsRow = 1;
    private const int NewStudentsRow = 2;

    private const int LabelColumn = 0;
    private const int ValueColumn = 1;
    private const int SecondLabelColumn = 2;
    private const int SecondValueColumn = 3;

    private const string NameLabel = "Name";
    private const string QuizDateLabel = "Quiz Date";
    private const string StudentCountLabel = "Students";
    private const string AverageScoreLabel = "Average Score";
    private const string NewStudentLabel = "New Students";

    private readonly Label nameValue = new();
    private readonly Label quizDateValue = new();
    private readonly Label studentCountValue = new();
    private readonly Label averageScoreValue = new();

    private Label newStudentLabel = new();
    private readonly Label newStudentValue = new();

    public QuizMetaPane()
    {
        InitializeComponent();

        SetVisible(false);
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

        //
        // infoTable
        //
        infoTable.AutoSize = true;
        infoTable.AutoSizeMode =
            AutoSizeMode.GrowAndShrink;
        infoTable.Dock = DockStyle.Top;

        infoTable.ColumnStyles.Clear();

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        infoTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        newStudentLabel = new()
        {
            Text = NewStudentLabel,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Padding = new Padding(4),
            Font = new Font(
               SystemFonts.DefaultFont,
               FontStyle.Bold)
        };
        newStudentLabel.Text = NewStudentLabel;

        AddLabel(
            NameLabel,
            LabelColumn,
            NameRow);

        AddValue(
            nameValue,
            ValueColumn,
            NameRow);

        AddLabel(
            QuizDateLabel,
            SecondLabelColumn,
            NameRow);

        AddValue(
            quizDateValue,
            SecondValueColumn,
            NameRow);

        AddLabel(
            StudentCountLabel,
            LabelColumn,
            StatsRow);

        AddValue(
            studentCountValue,
            ValueColumn,
            StatsRow);

        AddLabel(
            AverageScoreLabel,
            SecondLabelColumn,
            StatsRow);

        AddValue(
            averageScoreValue,
            SecondValueColumn,
            StatsRow);

        infoTable.Controls.Add(
            newStudentLabel,
            LabelColumn,
            NewStudentsRow);

        AddValue(
            newStudentValue,
            ValueColumn,
            NewStudentsRow);

        Controls.Add(
            titleLabel,
            0,
            0);

        Controls.Add(
            infoTable,
            0,
            1);
    }

    public void SetData(QuizInfo quizInfo)
    {
        nameValue.Text =
            quizInfo.Name;

        quizDateValue.Text =
            quizInfo.QuizDate.ToShortDateString();

        studentCountValue.Text =
            quizInfo.Stats.StudentCount.ToString();

        averageScoreValue.Text =
            quizInfo.Stats.AverageScore.ToString();

        newStudentValue.Text =
            quizInfo.Stats.NewStudents.Count.ToString();

        Color newStudentColor = quizInfo.Stats.NewStudents.Count > 0
            ? UiConstants.NewStudentColor : SystemColors.ControlText;
        newStudentLabel.ForeColor = newStudentColor;
        newStudentValue.ForeColor = newStudentColor;

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
            Padding = new Padding(4),
            Font = new Font(
                SystemFonts.DefaultFont,
                FontStyle.Bold)
        };

        infoTable.Controls.Add(
            label,
            column,
            row);
    }

    private void AddValue(
        Label label,
        int column,
        int row)
    {
        label.AutoSize = true;
        label.Anchor = AnchorStyles.Left;
        label.Padding = new Padding(4);

        infoTable.Controls.Add(
            label,
            column,
            row);
    }

    public void SetVisible(bool visible)
    {
        Visible = visible;
    }
}