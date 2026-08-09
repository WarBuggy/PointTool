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
        string selectedName = SelectedName;
        string selectedClassName = SelectedClassName;
        ClassExplorerSelectionType selectedType =
            CurrentlySelected;

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

        TreeNode? selectedNode =
            FindNode(
                selectedType,
                selectedName,
                selectedClassName);

        if (selectedNode is not null)
        {
            treeView.Focus();
            treeView.SelectedNode = selectedNode;
        }
        else
        {
            treeView.SelectedNode = null;
        }
    }

    private TreeNode? FindNode(
        ClassExplorerSelectionType selectionType,
        string selectedName,
        string selectedClassName)
    {
        if (selectionType == ClassExplorerSelectionType.None)
        {
            return null;
        }

        foreach (TreeNode classNode in treeView.Nodes)
        {
            if (selectionType ==
                ClassExplorerSelectionType.Class)
            {
                if (classNode.Text.Equals(
                    selectedName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return classNode;
                }

                continue;
            }

            if (!classNode.Text.Equals(
                selectedClassName,
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (TreeNode quizNode in classNode.Nodes)
            {
                if (quizNode.Text.Equals(
                    selectedName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return quizNode;
                }
            }
        }

        return null;
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

            SelectionChanged?.Invoke(
                this,
                EventArgs.Empty);

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