using PointTool.Enums;
using PointTool.Managers;
using static PointTool.Managers.QuizManager;

namespace PointTool.Panes.Left;

public class ClassExplorer : UserControl
{
    private readonly TreeView treeView = new();

    private readonly ClassManager classManager;

    private readonly QuizManager quizManager;

    public ClassExplorerSelectionType CurrentlySelected { get; private set; } =
        ClassExplorerSelectionType.None;

    public string SelectedName { get; private set; } =
        string.Empty;

    public string SelectedClassName { get; private set; } =
        string.Empty;

    public event EventHandler? SelectionChanged;

    public ClassExplorer(
        ClassManager classManager,
        QuizManager quizManager)
    {
        this.classManager = classManager;
        this.quizManager = quizManager;

        SuspendLayout();

        //
        // ClassExplorer
        //
        Dock = DockStyle.Fill;
        Margin = Padding.Empty;

        //
        // treeView
        //
        treeView.Dock = DockStyle.Fill;

        Controls.Add(treeView);

        ResumeLayout(false);

        treeView.AfterSelect += TreeView_AfterSelect;
    }

    public void RefreshTree()
    {
        treeView.BeginUpdate();

        treeView.Nodes.Clear();

        foreach (string className in classManager.Classes.Keys)
        {
            TreeNode classNode =
                treeView.Nodes.Add(className);

            foreach (QuizInfo quizInfo in
                quizManager.GetQuizzes(className))
            {
                classNode.Nodes.Add(
                    quizInfo.Name);
            }
        }

        treeView.EndUpdate();
    }

    private void TreeView_AfterSelect(
        object? sender,
        TreeViewEventArgs e)
    {
        TreeNode? node = e.Node;

        if (node == null)
        {
            CurrentlySelected =
                ClassExplorerSelectionType.None;

            SelectedName = string.Empty;

            SelectedClassName = string.Empty;

            return;
        }

        if (node.Parent == null)
        {
            //
            // Class selected
            //
            CurrentlySelected =
                ClassExplorerSelectionType.Class;

            SelectedName = node.Text;

            SelectedClassName = node.Text;
        }
        else
        {
            //
            // Quiz selected
            //
            CurrentlySelected =
                ClassExplorerSelectionType.Quiz;

            SelectedName = node.Text;

            SelectedClassName = node.Parent.Text;
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

}