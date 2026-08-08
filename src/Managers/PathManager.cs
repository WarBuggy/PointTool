namespace PointTool.Managers;

public static class PathManager
{
    public static string GetExecutableDirectory()
    {
        return AppContext.BaseDirectory;
    }

    public static string GetDataDirectory()
    {
        return Path.Combine(GetExecutableDirectory(), AppInfo.DataFolderName);
    }

    public static bool HasDataDirectory()
    {
        return Directory.Exists(GetDataDirectory());
    }

}
