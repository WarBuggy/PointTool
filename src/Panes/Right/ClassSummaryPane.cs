using PointTool.Managers;

namespace PointTool.Panes.Right;

public class ClassSummaryPane : DisplayPane
{
    private const string NameColumn = "Name";
    private const string TotalScoreColumn = "Total Score";
    private const string QuizzesTakenColumn = "Quizzes Taken";

    private readonly Button exportButton = new()
    {
        Text = "Export to Excel",
        AutoSize = true
    };

    public event EventHandler? ExportRequested;

    public ClassSummaryPane()
        : base("Class Summary")
    {
        SetupExportButton();
    }

    private void SetupExportButton()
    {
        exportButton.Click += ExportButton_Click;

        AddButton(exportButton);
    }

    private void ExportButton_Click(
        object? sender,
        EventArgs e)
    {
        ExportRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void SetData(
        ClassSummary summary)
    {
        ClearTable();
        SetupDisplayTable(summary);

        for (int row = 0; row < summary.Students.Count; row++)
        {
            StudentSummary student =
                summary.Students[row];

            int tableRow = row + 1;

            AddLabel(
                student.Name,
                0,
                tableRow);

            AddLabel(
                student.TotalScore.ToString(),
                1,
                tableRow,
                AnchorStyles.Right);

            AddLabel(
                student.QuizzesTaken.ToString(),
                2,
                tableRow,
                AnchorStyles.Right);

            for (int quiz = 0;
                 quiz < summary.QuizNames.Count;
                 quiz++)
            {
                string quizName =
                    summary.QuizNames[quiz];

                student.Scores.TryGetValue(
                    quizName,
                    out int score);

                AddLabel(
                    score.ToString(),
                    quiz + 3,
                    tableRow,
                    AnchorStyles.Right);
            }
        }
    }

    private void SetupDisplayTable(
        ClassSummary summary)
    {
        int dataColumnCount =
            3 + summary.QuizNames.Count;

        int columnCount =
            dataColumnCount + 1;

        DisplayTable.ColumnCount =
            columnCount;

        DisplayTable.RowCount = 1;

        DisplayTable.ColumnStyles.Clear();

        for (int column = 0;
             column < columnCount;
             column++)
        {
            DisplayTable.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.AutoSize));
        }

        DisplayTable.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100f));

        DisplayTable.RowStyles.Clear();

        DisplayTable.RowStyles.Add(
            new RowStyle(
                SizeType.AutoSize));

        AddLabel(
            NameColumn,
            0,
            0,
            AnchorStyles.Left,
            FontStyle.Bold);

        AddLabel(
            TotalScoreColumn,
            1,
            0,
            AnchorStyles.Right,
            FontStyle.Bold);

        AddLabel(
            QuizzesTakenColumn,
            2,
            0,
            AnchorStyles.Right,
            FontStyle.Bold);

        for (int quiz = 0;
             quiz < summary.QuizNames.Count;
             quiz++)
        {
            AddLabel(
                summary.QuizNames[quiz],
                quiz + 3,
                0,
                AnchorStyles.Right,
                FontStyle.Bold);
        }
    }

    private void ClearTable()
    {
        DisplayTable.Controls.Clear();

        DisplayTable.ColumnStyles.Clear();
        DisplayTable.RowStyles.Clear();

        DisplayTable.ColumnCount = 0;
        DisplayTable.RowCount = 0;
    }

    private void AddLabel(
        string text,
        int column,
        int row,
        AnchorStyles anchor = AnchorStyles.Left,
        FontStyle fontStyle = FontStyle.Regular)
    {
        Label label = new()
        {
            Text = text,
            AutoSize = true,
            Anchor = anchor,
            Padding = new Padding(4)
        };

        if (fontStyle != FontStyle.Regular)
        {
            label.Font = new Font(
                label.Font,
                fontStyle);
        }

        DisplayTable.Controls.Add(
            label,
            column,
            row);
    }
}