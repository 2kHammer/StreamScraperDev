using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class StreamingcontentUpdater
{
    private IStreamingcontentScraper<Tuple<string, string>> _streamingcontentscraper;

    public StreamingcontentUpdater(IStreamingcontentScraper<Tuple<string, string>> streamingcontentscraper)
    {
        _streamingcontentscraper = streamingcontentscraper;
    }

    public async Task updateStreamingContent()
    {
        List<Tuple<string,string>> actualcontent = await _streamingcontentscraper.GetContentAsync();
        ContentlistRepository contentlistrep = new ContentlistRepository();
        await contentlistrep.UpdateContent(actualcontent);
    }
}