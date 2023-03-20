using Microsoft.Extensions.Logging;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class StreamingcontentUpdater
{
    private IStreamingcontentScraper<SearchCriterias> _streamingcontentscraper;
    private ILogger<StreamingcontentUpdater> _logger;
    public StreamingcontentUpdater(IStreamingcontentScraper<SearchCriterias> streamingcontentscraper, ILogger<StreamingcontentUpdater> logger)
    {
        _streamingcontentscraper = streamingcontentscraper;
        _logger = logger;
    }

    public async Task<bool> updateStreamingContent()
    {
        try
        {
            List<SearchCriterias> actualcontent = await _streamingcontentscraper.GetContentAsync();
            IContentlistRepository<SearchCriterias> contentlistrep = new ContentlistRepository();
            await contentlistrep.UpdateContent(actualcontent);
            _logger.LogInformation("Contentlist updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating Contentlist: {ex}");
            return false;
        }
    }
}