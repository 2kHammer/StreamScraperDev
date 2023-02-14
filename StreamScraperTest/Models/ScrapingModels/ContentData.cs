namespace StreamScraperTest.Models.ScrapingModels;

public class ContentData
{
    public string Contentname { get; set; }
    public string? Rating { get; set; }
    public Contenttype Type { get; set; }
    public string? PublicationYear { get; set; }
    public string? EndYear { get; set; }
    
    public DateTime ActTime { get; set; }
    
    public string? urlPicture { get; set; }
    public List<string>? Genres { get;}

    public List<string>? OriginCountries { get; }
    
    public bool NotFound { get; set; }
    
    public ContentData(){ 
        Genres = new List<string>();
        OriginCountries = new List<string>();
        NotFound = false;
    }
}
