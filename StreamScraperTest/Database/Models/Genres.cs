namespace StreamScraperTest.Database.Models;

public class Genres
{
    public int GenreId { get; set; }
    public string GenreName { get; set; }
    public virtual ICollection<Contentinformations> Contentinformation{ get; set; }

    public Genres()
    {
        Contentinformation = new List<Contentinformations>();
    }
    
    
    public Genres(string name, Contentinformations firstContentinformation=null)
    {
        GenreName= name;
        Contentinformation = new List<Contentinformations>();
        if(firstContentinformation != null) Contentinformation.Add(firstContentinformation);
    }
}