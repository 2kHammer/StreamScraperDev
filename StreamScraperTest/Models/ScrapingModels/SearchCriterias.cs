namespace StreamScraperTest.Models.ScrapingModels;

public class SearchCriterias
{
    public string ContentName { get; set; }
    public string PublicationYear { get; set; }

    public SearchCriterias()
    {
    }

    public SearchCriterias(string name, string year)
    {
        ContentName = name;
        PublicationYear = year;
    }

}