using ClosedXML.Excel;
using PointTool.Managers;

namespace PointTool.Export;

public static class ClassSummaryExcelExporter
{
    private const int NameColumn = 1;
    private const int TotalScoreColumn = 2;
    private const int QuizzesTakenColumn = 3;
    private const int FirstQuizColumn = 4;
    private const int ClassInfoTitleRow = 1;
    private const int ClassNameRow = 2;
    private const int QuizCountRow = 3;
    private const int StudentCountRow = 4;

    private const int SummaryTitleRow = 6;
    private const int HeaderRow = 7;
    private const int FirstStudentRow = 8;

    private const double MinimumQuizColumnWidth = 14;

    public static void Export(string className, ClassSummary summary, string filePath)
    {
        using XLWorkbook workbook = new();

        IXLWorksheet worksheet =
            workbook.AddWorksheet("Class Summary");

        WriteClassInfo(
            worksheet,
            className,
            summary);

        WriteClassSummary(
            worksheet,
            summary);

        worksheet.Columns().AdjustToContents();

        SetMinimumQuizColumnWidth(worksheet, summary);

        workbook.SaveAs(filePath);
    }

    private static void WriteClassInfo(
        IXLWorksheet worksheet,
        string className,
        ClassSummary summary)
    {
        worksheet.Cell(
            ClassInfoTitleRow,
            NameColumn)
            .Value = "Class Info";

        worksheet.Cell(
            ClassInfoTitleRow,
            NameColumn)
            .Style.Font.Bold = true;

        worksheet.Cell(
            ClassNameRow,
            NameColumn)
            .Value = "Name";

        worksheet.Cell(
            ClassNameRow,
            NameColumn + 1)
            .Value = className;

        worksheet.Cell(
            QuizCountRow,
            NameColumn)
            .Value = "Quiz";

        worksheet.Cell(
            QuizCountRow,
            NameColumn + 1)
            .Value = summary.QuizNames.Count;

        worksheet.Cell(
            StudentCountRow,
            NameColumn)
            .Value = "Students";

        worksheet.Cell(
            StudentCountRow,
            NameColumn + 1)
            .Value = summary.Students.Count;
    }

    private static void WriteClassSummary(
        IXLWorksheet worksheet, ClassSummary summary)
    {
        worksheet.Cell(
            SummaryTitleRow,
            NameColumn)
            .Value = "Class Summary";

        worksheet.Cell(
            SummaryTitleRow,
            NameColumn)
            .Style.Font.Bold = true;

        WriteHeaders(worksheet, summary);

        CreateSummaryTable(worksheet, summary);

        WriteStudents(worksheet, summary);
    }

    private static void WriteHeaders(IXLWorksheet worksheet, ClassSummary summary)
    {
        worksheet.Cell(
            HeaderRow,
            NameColumn)
            .Value = "Name";

        worksheet.Cell(
            HeaderRow,
            TotalScoreColumn)
            .Value = "Total Score";

        worksheet.Cell(
            HeaderRow,
            QuizzesTakenColumn)
            .Value = "Quizzes Taken";

        for (int quiz = summary.QuizNames.Count - 1; quiz >= 0; quiz--)
        {
            int displayColumn =
                FirstQuizColumn +
                summary.QuizNames.Count -
                1 -
                quiz;

            string quizName = summary.QuizNames[quiz];

            int averageScore = summary.QuizAverageScores[quizName];

            worksheet.Cell(
                HeaderRow,
                displayColumn)
                .Value = $"{quizName}\n({averageScore})";
        }

        int lastColumn =
            FirstQuizColumn +
            summary.QuizNames.Count -
            1;

        worksheet.Range(
            HeaderRow,
            NameColumn,
            HeaderRow,
            lastColumn)
            .Style.Font.Bold = true;

        worksheet.Range(
            HeaderRow,
            NameColumn,
            HeaderRow,
            lastColumn)
            .Style.Alignment.WrapText = true;
    }

    private static void WriteStudents(IXLWorksheet worksheet, ClassSummary summary)
    {
        for (int studentIndex = 0;
             studentIndex < summary.Students.Count;
             studentIndex++)
        {
            StudentSummary student =
                summary.Students[studentIndex];

            int row =
                FirstStudentRow + studentIndex;

            worksheet.Cell(
                row,
                NameColumn)
                .Value = student.Name;

            worksheet.Cell(
                row,
                QuizzesTakenColumn)
                .Value = student.QuizzesTaken;

            for (int quiz = summary.QuizNames.Count - 1; quiz >= 0; quiz--)
            {
                string quizName = summary.QuizNames[quiz];

                int displayColumn = FirstQuizColumn + summary.QuizNames.Count
                    - 1 - quiz;

                IXLCell scoreCell =
                    worksheet.Cell(
                        row,
                        displayColumn);

                if (student.Scores.TryGetValue(
                    quizName, out int score))
                {
                    scoreCell.Value = score;
                }
                else
                {
                    scoreCell.Value = string.Empty;
                    scoreCell.Style.Fill.BackgroundColor =
                        XLColor.FromArgb(235, 130, 130);
                }
            }

            if (summary.QuizNames.Count > 0)
            {
                int firstQuizColumn = FirstQuizColumn;

                int lastQuizColumn = FirstQuizColumn
                    + summary.QuizNames.Count - 1;

                string firstQuizCell = worksheet.Cell(
                    row, firstQuizColumn).Address.ToString()!;

                string lastQuizCell = worksheet.Cell(
                        row, lastQuizColumn).Address.ToString()!;

                IXLCell totalScoreCell = worksheet.Cell(
                    row, TotalScoreColumn);

                totalScoreCell.FormulaA1 =
                    $"SUM({firstQuizCell}:{lastQuizCell})";
            }
            else
            {
                worksheet.Cell(
                    row,
                    TotalScoreColumn)
                    .Value = 0;
            }
        }
    }

    private static void CreateSummaryTable(
        IXLWorksheet worksheet, ClassSummary summary)
    {
        int lastColumn =
            FirstQuizColumn +
            summary.QuizNames.Count -
            1;

        int lastStudentRow =
            FirstStudentRow +
            summary.Students.Count -
            1;

        IXLRange range =
            worksheet.Range(
                HeaderRow,
                NameColumn,
                lastStudentRow,
                lastColumn);

        range.CreateTable();
    }

    private static void SetMinimumQuizColumnWidth(
        IXLWorksheet worksheet, ClassSummary summary)
    {
        int lastQuizColumn = FirstQuizColumn
            + summary.QuizNames.Count - 1;

        for (int column = FirstQuizColumn;
             column <= lastQuizColumn;
             column++)
        {
            worksheet.Column(column).Width = Math.Max(
                MinimumQuizColumnWidth, worksheet.Column(column).Width);
        }
    }
}