using System.Collections.ObjectModel;

namespace StreamScraperTest.Database.Models;

public class Contentinformations
{
   public int ContentinformationId { get; set; }
   public float? Rating { get; set; }
   public string? Fullname { get; set; }
   public int? Lastyear { get; set; }
   
   public DateTime DateTimeDataScraped { get; set; }
   public bool AvailableNetflix { get; set; }
   
   public string? urlPicture { get; set; }
   
   public int ContenttypeId {get; set; }
   public virtual Contenttypes Contenttype { get; set; }
   
   public virtual ICollection<Genres> Genre{ get; set; }
   public virtual ICollection<Countries> Country{ get; set; }

   public virtual Contents? Content { get; set; }
   public int ContentId { get; set; }

   public Contentinformations()
   {
      Genre = new Collection<Genres>();
      Country = new Collection<Countries>();
   }
}