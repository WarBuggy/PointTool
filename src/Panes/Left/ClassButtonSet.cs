namespace PointTool.Panes.Left;

public class ClassButtonSet
{
    private readonly Button exportButton = new();

    private readonly Button editButton = new();

    private readonly Button uploadScoreButton = new();

    private readonly Button archiveButton = new();

    public IReadOnlyList<Button> Buttons { get; }

    public ClassButtonSet()
    {
        exportButton.Text = "Export scores";
        editButton.Text = "Edit class";
        uploadScoreButton.Text = "Upload scores";
        archiveButton.Text = "Archive class";

        foreach (Button button in new[]
        {
            uploadScoreButton,
            exportButton,
            editButton,
            archiveButton,
        })
        {
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(2);
        }

        Buttons =
        [
            uploadScoreButton,
            exportButton,
            editButton,
            archiveButton,
        ];
    }

    public Button ExportButton => exportButton;

    public Button EditButton => editButton;

    public Button UploadScoreButton => uploadScoreButton;

    public Button ArchiveButton => archiveButton;

}