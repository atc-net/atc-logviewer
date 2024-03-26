namespace Atc.LogCollector.Tests.Serilog;

[Collection(nameof(TestCollection))]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public class SerilogFileCollectorIntegrationTests : SerilogCollectorIntegrationTestBase
{
    [Fact]
    public async Task CanParseFileFormat()
    {
        // Arrange
        SendLogItemsToLogger();
        await CloseAndFlushLogger();

        var extractor = new SerilogFileExtractor();
        var collector = new SerilogFileCollector(extractor);

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
        await CloseAndFlushLogger();

        var extractor = new SerilogFileExtractor();
        var collector = new SerilogFileCollector(extractor);

        // Atc
        var (isSuccessFul, lastLineNumber) = await collector.ReadAndParseLines(
            CurrentLogFile,
            CancellationToken.None);

        // Assert
        Assert.True(isSuccessFul);
        Assert.Equal(GetLineNumbersFromLogItems(), lastLineNumber);
    }
}