using PointTool.Utilities;

namespace PointTool.InputGroups;

public class InputFileGroup : InputGroup
{
    private readonly TableLayoutPanel inputPanel = new();

    private readonly Button browseButton = new();

    private readonly Label fileNameLabel = new();

    private readonly OpenFileDialog openFileDialog = new();

    public string FilePath { get; private set; } =
        string.Empty;

    public string Filter
    {
        get => openFileDialog.Filter;
        set => openFileDialog.Filter = value;
    }

    public string Title
    {
        get => openFileDialog.Title;
        set => openFileDialog.Title = value;
    }

    public string FileName =>
        string.IsNullOrEmpty(FilePath)
            ? string.Empty
            : Path.GetFileName(FilePath);

    public InputFileGroup(
        string labelText,
        bool isRequired = false)
        : base(
            labelText,
            isRequired)
    {

        Filter = "Score files (*.xlsx)|*.xlsx";

        Title = "Select Score File";

        //
        // inputPanel
        //
        inputPanel.ColumnCount = 2;
        inputPanel.RowCount = 1;
        inputPanel.AutoSize = true;
        inputPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        inputPanel.Margin = Padding.Empty;

        inputPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.AutoSize));

        inputPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100f));

        //
        // browseButton
        //
        browseButton.Text = "Browse...";
        browseButton.Size = new Size(
            UiConstants.ButtonWidth,
            UiConstants.ButtonHeight);
        browseButton.Click += BrowseButton_Click;

        //
        // fileNameLabel
        //
        fileNameLabel.AutoSize = true;
        fileNameLabel.Anchor =
            AnchorStyles.Left;
        fileNameLabel.Margin =
            new Padding(8, 0, 0, 0);
        fileNameLabel.Text = "No file selected";

        //
        // inputPanel
        //
        inputPanel.Controls.Add(
            browseButton,
            0,
            0);

        inputPanel.Controls.Add(
            fileNameLabel,
            1,
            0);

        InputControl = inputPanel;
    }

    private void BrowseButton_Click(
        object? sender,
        EventArgs e)
    {
        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        FilePath = openFileDialog.FileName;

        fileNameLabel.Text = FileName;
    }
}