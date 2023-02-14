namespace StreamScraperTest.Scraping;

public interface IStreamingcontentScraper<T>
{
    Task<List<T>> GetContentAsync();
}