using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Scraping;

public interface IContentdataScraper<T>
{
    Task<ContentData> GetDataAsync (T criteria);
    Task<List<ContentData>> GetFullDataAsync(List<T> criterias);
}