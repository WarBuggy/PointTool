namespace PointTool.Panes.Left;

public class QuizButtonSet
{
    private readonly Button updateScoreButton = new();

    private readonly Button editButton = new();

    private readonly Button deleteButton = new();

    public IReadOnlyList<Button> Buttons { get; }

    public QuizButtonSet()
    {
        updateScoreButton.Text = "Update scores";
        editButton.Text = "Edit quiz";
        deleteButton.Text = "Delete quiz";

        foreach (Button button in new[]
        {
            updateScoreButton,
            editButton,
            deleteButton,
        })
        {
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(2);
        }

        Buttons =
        [
            updateScoreButton,
            editButton,
            deleteButton,
        ];
    }

    public Button UpdateScoreButton => updateScoreButton;

    public Button EditButton => editButton;

    public Button DeleteButton => deleteButton;
}