namespace PointTool.Panes.Left;

public class ClassButtonSet
{
    private readonly Button checkNamesButton = new();

    private readonly Button editButton = new();

    private readonly Button uploadScoreButton = new();

    private readonly Button archiveButton = new();

    public IReadOnlyList<Button> Buttons { get; }

    public ClassButtonSet()
    {
        checkNamesButton.Text = "Check names";
        editButton.Text = "Edit class";
        uploadScoreButton.Text = "Upload scores";
        archiveButton.Text = "Archive class";

        foreach (Button button in new[]
        {
            checkNamesButton,
            uploadScoreButton,
            editButton,
            archiveButton,
        })
        {
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(2);
        }

        Buttons =
        [
            checkNamesButton,
            uploadScoreButton,
            editButton,
            archiveButton,
        ];
    }

    public Button CheckNamesButton => checkNamesButton;

    public Button EditButton => editButton;

    public Button UploadScoreButton => uploadScoreButton;

    public Button ArchiveButton => archiveButton;
}