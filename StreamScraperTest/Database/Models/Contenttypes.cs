using System.ComponentModel.DataAnnotations.Schema;
using StreamScraperTest.Models;

namespace StreamScraperTest.Database.Models;

public class Contenttypes
{
    public int ContenttypeId { get; set; }    
    public string ContenttypeDescription { get; set; }
    
    public virtual ICollection<Contentinformations> Contentinformations { get; set; }

    [NotMapped]
    public static IDictionary<Contenttype, int> mappingContenttypes = new Dictionary<Contenttype,int>()
    {
        { Contenttype.Unknown, 1},
        { Contenttype.Movie, 2},
        { Contenttype.Series, 3}
    };

    public Contenttypes(){}
}