using PointTool.Utilities;
using PointTool.Managers;
using PointTool.Modals;
using PointTool.Settings;
using PointTool.Panes.Work;
using PointTool.Panes.Left;
using PointTool.Enums;
using PointTool.Panes.Right;
using static PointTool.Managers.ClassManager;
using static PointTool.Managers.QuizManager;
using PointTool.Export;

namespace PointTool;

public class MainForm : Form
{
    private readonly ClassManager classManager;

    private readonly QuizManager quizManager;

    private readonly SettingManager settingManager;

    private readonly TableLayoutPanel rootTable =
        UIUtilities.CreateTable(columns: 1, rows: 2);

    private readonly TableLayoutPanel topTable =
        UIUtilities.CreateTable(columns: 2, rows: 1);

    private readonly TableLayoutPanel leftPane =
        UIUtilities.CreateTable(columns: 1, rows: 4);

    private readonly TableLayoutPanel rightPane =
        UIUtilities.CreateTable(columns: 1, rows: 2);

    private readonly WorkArea workArea = new();

    private readonly Button createClassButton = new();

    private readonly LeftActionArea leftActionArea = new();

    private readonly ClassButtonSet classButtonSet = new();
    private readonly QuizButtonSet quizButtonSet = new();

    private readonly ClassExplorer classExplorer;

    private readonly ClassMetaPane classMetaPane = new();

    private readonly QuizMetaPane quizMetaPane = new();

    private readonly DetailArea detailArea = new();

    private readonly Dictionary<string, QuizScorePane> quizScorePanes = [];

    private readonly Dictionary<string, ClassSummaryPane> classSummaryPanes = [];

    private readonly Dictionary<Type, WorkPane> workPanes = [];

    public MainForm()
    {
        settingManager = new();

        classManager = new();

        quizManager = new(classManager);

        classExplorer = new(classManager, quizManager);

        InitializeComponent();

        CreateWorkPane();

        CreateLeftPane();

    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1136, 681);
        StartPosition = FormStartPosition.CenterScreen;
        Name = "MainForm";
        Text = AppInfo.Name;

        //
        // createClassButton
        //
        createClassButton.Text = "Create new class";
        createClassButton.AutoSize = true;
        createClassButton.Margin = new Padding(2);
        createClassButton.Anchor = AnchorStyles.None;

        //
        // divider
        //
        GroupBox divider = new();
        divider.Text = string.Empty;
        divider.Dock = DockStyle.Fill;
        divider.Height = 2;
        divider.Margin = new Padding(4, 4, 4, 4);

        //
        // leftPane
        //
        leftPane.BorderStyle = BorderStyle.FixedSingle;
        leftPane.Dock = DockStyle.Fill;

        leftPane.RowStyles.Clear();
        leftPane.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));

        leftPane.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        leftPane.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        leftPane.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        leftPane.Controls.Add(
            classExplorer,
            0,
            0);

        leftPane.Controls.Add(
            createClassButton,
            0,
            1);

        leftPane.Controls.Add(
           divider,
           0,
           2);

        leftPane.Controls.Add(
            leftActionArea,
            0,
            3);

        //
        // rightPane
        //
        TableLayoutPanel topRightTable =
            UIUtilities.CreateTable(columns: 2, rows: 1);
        topRightTable.AutoSize = false;
        topRightTable.Dock = DockStyle.Fill;
        topRightTable.ColumnStyles.Clear();
        topRightTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50f));
        topRightTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50f));
        topRightTable.Controls.Add(
            classMetaPane,
            0,
            0);
        topRightTable.Controls.Add(
            quizMetaPane,
            1,
            0);

        rightPane.AutoSize = false;
        rightPane.Dock = DockStyle.Fill;
        rightPane.RowStyles.Clear();
        rightPane.RowStyles.Add(
            new RowStyle(SizeType.Percent, 23f));

        rightPane.RowStyles.Add(
            new RowStyle(SizeType.Percent, 77f));

        rightPane.Controls.Add(
            topRightTable,
            0,
            0);
        rightPane.Controls.Add(
            detailArea,
            0,
            1);

        //
        // rootTable
        //
        rootTable.AutoSize = false;
        rootTable.Dock = DockStyle.Fill;

        rootTable.RowStyles.Clear();
        rootTable.RowStyles.Add(
            new RowStyle(SizeType.Percent, 67f));
        rootTable.RowStyles.Add(
            new RowStyle(SizeType.Percent, 33f));


        topTable.AutoSize = false;
        topTable.Dock = DockStyle.Fill;

        topTable.ColumnStyles.Clear();
        topTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 20f));
        topTable.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 80f));

        leftPane.Dock = DockStyle.Fill;
        rightPane.Dock = DockStyle.Fill;

        topTable.Controls.Add(leftPane, 0, 0);
        topTable.Controls.Add(rightPane, 1, 0);

        rootTable.Controls.Add(topTable, 0, 0);
        rootTable.Controls.Add(workArea, 0, 1);

        Controls.Add(rootTable);

        ResumeLayout(false);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        Startup();
    }

    private void Startup()
    {
        RefreshData();

        if (classManager.Classes.Count < 1)
        {
            if (settingManager.Get(SettingDefs.ShowNoClassesMessage))
            {
                using NoClassMessageModal modal =
                    new(settingManager);

                modal.ShowDialog(this);
            }

            ShowWorkPane<AddClassPane>();

            return;
        }

        ShowClassExplorer();
    }

    private void ShowWorkPane<T>() where T : WorkPane
    {
        workArea.ShowPane(workPanes[typeof(T)]);
    }

    private void ShowClassExplorer()
    {
        //
        // Explorer
        //
        // leftPane.Controls.RemoveAt(0);

        // TODO:
        // Add TreeView when it is implemented.

        //
        // Actions
        //
    }

    private void CreateWorkPane()
    {
        workPanes.Add(typeof(AddClassPane), new AddClassPane(classManager));
        workPanes.Add(typeof(UploadScorePane), new UploadScorePane(quizManager));
        workPanes.Add(typeof(EditQuizPane), new EditQuizPane(quizManager));

        GetWorkPane<UploadScorePane>().QuizCreated += UploadScorePane_QuizCreated;
        GetWorkPane<AddClassPane>().ClassCreated += AddClassPane_ClassCreated;
        GetWorkPane<EditQuizPane>().QuizUpdated += EditQuizPane_QuizUpdated;

    }

    private void CreateLeftPane()
    {

        createClassButton.Click += CreateButton_Click;

        classExplorer.SelectionChanged +=
            ClassExplorer_SelectionChanged;

        CreateLeftActionArea();
    }

    private void CreateLeftActionArea()
    {
        classButtonSet.UploadScoreButton.Click +=
            UploadQuizButton_Click;

        quizButtonSet.UpdateScoreButton.Click +=
            UpdateScoreButton_Click;

        quizButtonSet.EditButton.Click +=
            EditQuizButton_Click;

        quizButtonSet.DeleteButton.Click +=
            DeleteQuizButton_Click;
    }

    private void CreateButton_Click(
        object? sender,
        EventArgs e)
    {
        ShowWorkPane<AddClassPane>();
    }

    private void UploadQuizButton_Click(
        object? sender,
        EventArgs e)
    {
        UploadScorePane uploadScorePane = GetWorkPane<UploadScorePane>();

        uploadScorePane.ClassName = classExplorer.SelectedClassName;

        ShowWorkPane<UploadScorePane>();
    }

    private void RefreshData()
    {
        ClearQuizScorePaneCache();
        ClearClassSummaryPaneCache();

        classManager.Refresh();

        quizManager.Refresh();

        classManager.UpdateStats(quizManager);

        classExplorer.RefreshTree();
    }

    private void UploadScorePane_QuizCreated(
        object? sender,
        EventArgs e)
    {
        RefreshData();

        classExplorer.ExpandClass(classExplorer.SelectedClassName);
    }

    private void AddClassPane_ClassCreated(
        object? sender,
        EventArgs e)
    {
        RefreshData();
    }

    private void ClassExplorer_SelectionChanged(
        object? sender,
        EventArgs e)
    {
        WaitModal.Instance.Show(() =>
        {
            workArea.Clear();

            switch (classExplorer.CurrentlySelected)
            {
                case ClassExplorerSelectionType.Class:
                    leftActionArea.ShowButtons(
                        classButtonSet.Buttons);

                    ShowClassInfo(
                        classManager.Classes[
                            classExplorer.SelectedClassName]);

                    HideQuizInfo();

                    ShowClassSummary();

                    break;

                case ClassExplorerSelectionType.Quiz:
                    leftActionArea.ShowButtons(
                       quizButtonSet.Buttons);


                    ShowClassInfo(
                        classManager.Classes[
                            classExplorer.SelectedClassName]);

                    ShowQuizData();

                    break;

                default:
                    leftActionArea.ClearAll();

                    HideClassInfo();
                    HideQuizInfo();
                    HideDetailPane();

                    break;
            }
        });
    }

    private T GetWorkPane<T>() where T : WorkPane
    {
        return (T)workPanes[typeof(T)];
    }

    private void ShowClassInfo(ClassData classData)
    {
        classMetaPane.SetData(classData);
        classMetaPane.SetVisible(true);
    }

    private void HideClassInfo()
    {
        classMetaPane.SetVisible(false);
    }

    private void ShowQuizInfo(QuizInfo quizInfo)
    {
        quizMetaPane.SetData(quizInfo);
        quizMetaPane.SetVisible(true);
    }

    private void HideQuizInfo()
    {
        quizMetaPane.SetVisible(false);
    }

    private void ShowQuizData()
    {
        QuizInfo? quizInfo =
            quizManager.GetQuizzes(
                classExplorer.SelectedClassName)
                .FirstOrDefault(quiz =>
                    quiz.Name.Equals(
                        classExplorer.SelectedName,
                        StringComparison.OrdinalIgnoreCase));

        if (quizInfo is null)
        {
            HideQuizInfo();
            return;
        }

        ShowQuizInfo(quizInfo);

        QuizScorePane pane = GetQuizScorePane(
            classExplorer.SelectedClassName, quizInfo);

        if (!quizScorePanes.ContainsKey(
            $"{classExplorer.SelectedClassName}:{quizInfo.Name}"))
        {
            pane.SetData(quizInfo);
        }

        detailArea.ShowDisplayPane(pane);

    }

    private void ShowClassSummary()
    {
        ClassSummaryPane classSummaryPane =
            GetClassSummaryPane(classExplorer.SelectedClassName);

        detailArea.ShowDisplayPane(
            classSummaryPane);
    }

    private void HideDetailPane()
    {
        detailArea.Clear();
    }

    private QuizScorePane GetQuizScorePane(
        string className,
        QuizInfo quizInfo)
    {
        string key =
            $"{className}:{quizInfo.Name}";

        if (quizScorePanes.TryGetValue(
            key,
            out QuizScorePane? pane))
        {
            return pane;
        }

        pane = new QuizScorePane();

        pane.SetData(quizInfo);

        quizScorePanes.Add(
            key,
            pane);

        return pane;
    }

    private void ClearQuizScorePaneCache()
    {
        quizScorePanes.Clear();
    }

    private ClassSummaryPane GetClassSummaryPane(
        string className)
    {
        if (classSummaryPanes.TryGetValue(
            className,
            out ClassSummaryPane? pane))
        {
            return pane;
        }

        pane = new ClassSummaryPane();

        pane.ExportRequested += ClassSummaryPane_ExportRequested;


        ClassSummary summary =
            quizManager.GetClassSummary(className);

        pane.SetData(summary);

        classSummaryPanes.Add(
            className,
            pane);

        return pane;
    }

    private void ClearClassSummaryPaneCache()
    {
        foreach (ClassSummaryPane pane in classSummaryPanes.Values)
        {
            pane.ExportRequested -=
                ClassSummaryPane_ExportRequested;
        }

        classSummaryPanes.Clear();
    }

    private void ClassSummaryPane_ExportRequested(
        object? sender,
        EventArgs e)
    {
        if (sender is not ClassSummaryPane pane)
        {
            return;
        }

        string className =
            classExplorer.SelectedClassName;

        using SaveFileDialog dialog = new()
        {
            Title = "Export Class Summary",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            DefaultExt = "xlsx",
            AddExtension = true,
            FileName = $"{className} Summary.xlsx"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ClassSummaryExcelExporter.Export(
            className,
            pane.Summary,
            dialog.FileName);
    }

    private void DeleteQuizButton_Click(
        object? sender, EventArgs e)
    {
        string className =
            classExplorer.SelectedClassName;

        string quizName =
            classExplorer.SelectedName;

        DialogResult result = MessageBox.Show(
            this,
            $"Are you sure you want to delete the quiz \"{quizName}\"?",
            "Delete Quiz",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        quizManager.DeleteQuiz(className, quizName);

        RefreshData();

        classExplorer.SelectClass(className);
    }

    private void EditQuizButton_Click(
        object? sender, EventArgs e)
    {
        string className =
            classExplorer.SelectedClassName;

        string quizName =
            classExplorer.SelectedName;

        QuizInfo? quizInfo =
            quizManager.GetQuiz(
                className,
                quizName);

        if (quizInfo is null)
        {
            return;
        }

        EditQuizPane editQuizPane = GetWorkPane<EditQuizPane>();

        editQuizPane.ClassName = className;
        editQuizPane.SetData(quizInfo);

        ShowWorkPane<EditQuizPane>();
    }

    private void EditQuizPane_QuizUpdated(
        object? sender, EventArgs e)
    {
        if (sender is not EditQuizPane pane)
        {
            return;
        }

        string className =
            classExplorer.SelectedClassName;

        string quizName =
            classExplorer.SelectedName;

        RefreshData();

        workArea.Clear();

        classExplorer.SelectQuiz(
            className, pane.UpdatedQuizName);
    }

    private void UpdateScoreButton_Click(
        object? sender, EventArgs e)
    {
        string className =
            classExplorer.SelectedClassName;

        string quizName =
            classExplorer.SelectedName;

        using OpenFileDialog dialog = new();
        dialog.Title = "Select New Score File";
        dialog.Filter = "Score files (*.xlsx)|*.xlsx";

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            quizManager.UpdateQuizScores(
                className, quizName);

            RefreshData();
        }
        catch (IOException ex)
        {
            UIUtilities.ShowMessage(
                "Update Scores",
                ex.Message,
                this);
        }
    }
}
