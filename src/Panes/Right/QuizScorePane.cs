using PointTool.Managers;
using static PointTool.Panes.Work.UploadScorePane;

namespace PointTool.Panes.Right;

public class QuizScorePane : DisplayPane
{
    private const string StudentColumn = "Student";
    private const string ScoreColumn = "Score";

    public QuizScorePane()
        : base("Quiz Scores")
    {
        SetupDisplayTable();
    }

    private void SetupDisplayTable()
    {
        DisplayTable.ColumnCount = 3;
        DisplayTable.RowCount = 1;

        DisplayTable.ColumnStyles.Clear();

        DisplayTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        DisplayTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        DisplayTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        DisplayTable.RowStyles.Clear();

        DisplayTable.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        AddLabel(
            StudentColumn,
            0,
            0,
            AnchorStyles.Left,
            FontStyle.Bold);

        AddLabel(
            ScoreColumn,
            1,
            0,
            AnchorStyles.Right,
            FontStyle.Bold);
    }

    public void SetData(QuizManager.QuizInfo quizInfo)
    {
        ClearScores();

        for (int row = 0; row < quizInfo.Scores.Count; row++)
        {
            DisplayTable.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            QuizScore score =
                quizInfo.Scores[row];

            AddLabel(
                score.Student,
                0,
                row + 1);

            AddLabel(
                score.Score.ToString(),
                1,
                row + 1,
                AnchorStyles.Right);
        }
    }

    private void ClearScores()
    {
        while (DisplayTable.Controls.Count > 2)
        {
            DisplayTable.Controls.RemoveAt(
                DisplayTable.Controls.Count - 1);
        }

        DisplayTable.RowCount = 1;

        DisplayTable.RowStyles.Clear();

        DisplayTable.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));
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
            Padding = new Padding(4),
            Font = new Font(
                Font.FontFamily,
                Font.Size,
                fontStyle)
        };

        DisplayTable.Controls.Add(
            label,
            column,
            row);
    }
}