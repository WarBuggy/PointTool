using PointTool.Managers;
using PointTool.Settings;
using PointTool.Utilities;

namespace PointTool.Modals;

public class NoClassMessageModal : CustomMessageModal
{
    private readonly SettingManager settingManager;

    private readonly CheckBox showAgainCheckBox = new();

    public NoClassMessageModal(
        SettingManager settingManager)
        : base(
            "Welcome to PointTool",
            CreateContent(settingManager, out CheckBox checkBox))
    {
        this.settingManager = settingManager;

        showAgainCheckBox = checkBox;

        AddButton(
            "OK",
            DialogResult.OK,
            isDefault: true,
            isCancel: true);
    }

    public new DialogResult ShowDialog()
    {
        DialogResult result = base.ShowDialog();

        settingManager.Set(
            SettingDefs.ShowNoClassesMessage,
            showAgainCheckBox.Checked);

        return result;
    }

    public new DialogResult ShowDialog(IWin32Window? owner)
    {
        DialogResult result = base.ShowDialog(owner);

        settingManager.Set(
            SettingDefs.ShowNoClassesMessage,
            showAgainCheckBox.Checked);

        return result;
    }

    private static Control CreateContent(
        SettingManager settingManager,
        out CheckBox checkBox)
    {
        TableLayoutPanel table =
            UIUtilities.CreateTable(columns: 1, rows: 2);

        Label messageLabel = new()
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Text =
                $"{AppInfo.Name} requires at least one class before you can begin working.\n\n" +
                $"No classes were found in the Data folder.\n\n" +
                $"Please create your first class to get started."
        };

        checkBox = new CheckBox
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Text = "Show this message next time",
            Checked = settingManager.Get(
                SettingDefs.ShowNoClassesMessage)
        };

        table.Controls.Add(messageLabel, 0, 0);
        table.Controls.Add(checkBox, 0, 1);

        table.RowStyles.Clear();
        table.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100f));
        table.RowStyles.Add(
            new RowStyle(SizeType.AutoSize));

        return table;
    }
}