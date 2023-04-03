using StreamScraperTest.Database.Models;
using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Database.IRepositories;

public interface IContentdataRepository
{
    public Task InsertOrUpdate(ContentData scrapeddata, int listid);
    public List<Contentinformations> GetNotFoundContentinformations();
    public List<Contentinformations> GetLongestAgoScrapedContentinformations(int amount);

}