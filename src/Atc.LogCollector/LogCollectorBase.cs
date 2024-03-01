// ReSharper disable InconsistentNaming
namespace Atc.LogCollector;

public abstract class LogCollectorBase
{
    protected static string GetSourceSystemFromSourceIdentifier(
        string sourceIdentifier,
        string formatToExcludeDatePart = "yyyyMMdd")
    {
        ArgumentNullException.ThrowIfNull(sourceIdentifier);
        ArgumentNullException.ThrowIfNull(formatToExcludeDatePart);

        // Attempt to find the date at the end of the string first
        for (var i = sourceIdentifier.Length; i >= formatToExcludeDatePart.Length; i--)
        {
            // Extract a potential date substring from the end of the sourceIdentifier
            var potentialDate = sourceIdentifier.Substring(i - formatToExcludeDatePart.Length, formatToExcludeDatePart.Length);

            if (DateTime.TryParseExact(potentialDate, formatToExcludeDatePart, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                // If a date is found, return the string without this date part
                return string.Concat(sourceIdentifier.AsSpan(0, i - formatToExcludeDatePart.Length), sourceIdentifier.AsSpan(i));
            }
        }

        // If no date is found, return the original string
        return sourceIdentifier;
    }
}