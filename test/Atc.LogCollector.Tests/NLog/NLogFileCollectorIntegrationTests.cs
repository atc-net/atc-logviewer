namespace Atc.LogCollector.Tests.NLog;

[Collection(nameof(TestCollection))]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public class NLogFileCollectorIntegrationTests : NLogCollectorIntegrationTestBase
{
    [Fact]
    public void CanParseFileFormat()
    {
        // Arrange
        SendLogItemsToLogger();
        FlushAndShutdownLogger();

        var extractor = new NLogFileExtractor();
        var collector = new NLogFileCollector(extractor);

        // Atc
        var actual = collector.CanParseFileFormat(
            CurrentLogFile);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task ReadAndParseLines()
    {
        // Arrange
        SendLogItemsToLogger();
        FlushAndShutdownLogger();

        var extractor = new NLogFileExtractor();
        var collector = new NLogFileCollector(extractor);

        // Atc
        var (isSuccessFul, lastLineNumber) = await collector.ReadAndParseLines(
            CurrentLogFile,
            CancellationToken.None);

        // Assert
        Assert.True(isSuccessFul);
        Assert.Equal(GetLineNumbersFromLogItems(), lastLineNumber);
    }
}