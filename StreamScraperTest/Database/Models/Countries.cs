namespace StreamScraperTest.Database.Models;

public class Countries
{
    public int CountryId { get; set; }
    public string CountryName { get; set; }

    public virtual ICollection<Contentinformations> Contentinformation { get; set; }

    public Countries()
    {
        Contentinformation = new List<Contentinformations>();
    }

    public Countries(string name, Contentinformations firstContentinformation=null)
    {
        CountryName = name;
        Contentinformation = new List<Contentinformations>();
        if(firstContentinformation != null) Contentinformation.Add(firstContentinformation);
    }
}