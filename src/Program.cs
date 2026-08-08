using PointTool.Managers;
using PointTool.Modals;

namespace PointTool
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!CheckDataDirectory())
            {
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static bool CheckDataDirectory()
        {
            if (PathManager.HasDataDirectory())
            {
                return true;
            }

            MessageBox.Show(
                $"{AppInfo.Name} could not find the required \"{AppInfo.DataFolderName}\" folder.\n\n" +
                $"The \"{AppInfo.DataFolderName}\" folder and {AppInfo.Name}.exe must be located in the same parent folder.\n\n" +
                $"For example:\n\n" +
                $"    Any Parent Folder/\n" +
                $"    │\n" +
                $"    ├── {AppInfo.Name}.exe\n" +
                $"    │\n" +
                $"    └── {AppInfo.DataFolderName}/\n\n" +
                $"To continue, do one of the following:\n\n" +
                $"• If you do not already have a {AppInfo.DataFolderName} folder, create one in the same parent folder as {AppInfo.Name}.exe.\n\n" +
                $"• If you already have a {AppInfo.DataFolderName} folder elsewhere, copy {AppInfo.Name}.exe into the parent folder that contains {AppInfo.DataFolderName}.\n\n" +
                $"• Or copy the existing {AppInfo.DataFolderName} folder into the same parent folder as {AppInfo.Name}.exe.\n\n" +
                $"Run {AppInfo.Name} again when {AppInfo.DataFolderName} folder and {AppInfo.Name}.exe are in the same parent folder.\n\n" +
                $"{AppInfo.Name} will now close.",
                $"{AppInfo.DataFolderName} Folder Not Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }
    }
}