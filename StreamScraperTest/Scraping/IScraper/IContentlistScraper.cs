namespace StreamScraperTest.Scraping;

public interface IContentlistScraper<T>
{
    Task<List<T>> GetContentAsync();
}