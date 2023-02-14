using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Database.IRepositories;

public interface IContentdataRepository
{
    public Task InsertOrUpdate(ContentData scrapeddata, int listid);
}