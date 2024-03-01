namespace Atc.LogCollector.Extensions;

public static class FileInfoExtensions
{
    public static bool IsCreatedToday(
        this FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        return DateTime.Now.ToShortDateString().Equals(
            fileInfo.CreationTime.ToShortDateString(),
            StringComparison.Ordinal);
    }
}