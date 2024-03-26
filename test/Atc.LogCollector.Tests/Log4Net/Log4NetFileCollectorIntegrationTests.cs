using Atc.LogCollector.Log4Net;

namespace Atc.LogCollector.Tests.Log4Net;

[Collection(nameof(TestCollection))]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public class Log4NetFileCollectorIntegrationTests : Log4NetCollectorIntegrationTestBase
{
    [Fact]
    public void CanParseFileFormat()
    {
        // Arrange
        SendLogItemsToLogger();
        FlushAndShutdownLogger();

        var extractor = new Log4NetFileExtractor();
        var collector = new Log4NetFileCollector(extractor);

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

        var extractor = new Log4NetFileExtractor();
        var collector = new Log4NetFileCollector(extractor);

        // Atc
        var (isSuccessFul, lastLineNumber) = await collector.ReadAndParseLines(
            CurrentLogFile,
            CancellationToken.None);

        // Assert
        Assert.True(isSuccessFul);
        Assert.Equal(GetLineNumbersFromLogItems(), lastLineNumber);
    }
}