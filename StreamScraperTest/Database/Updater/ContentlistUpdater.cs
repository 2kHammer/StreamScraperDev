using Microsoft.Extensions.Logging;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class ContentlistUpdater
{
    private IContentlistScraper<SearchCriterias> _streamingcontentscraper;
    private ILogger<ContentlistUpdater> _logger;
    public ContentlistUpdater(IContentlistScraper<SearchCriterias> streamingcontentscraper, ILogger<ContentlistUpdater> logger)
    {
        _streamingcontentscraper = streamingcontentscraper;
        _logger = logger;
    }

    public async Task<bool> updateStreamingContent()
    {
        try
        {
            List<SearchCriterias> actualcriterias = await _streamingcontentscraper.GetContentAsync();
            IContentlistRepository<SearchCriterias> contentlistrep = new ContentlistRepository();
            await contentlistrep.UpdateContent(actualcriterias);
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