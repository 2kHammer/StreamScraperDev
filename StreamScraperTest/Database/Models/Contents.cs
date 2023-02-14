namespace StreamScraperTest.Database.Models;

public class Contents
{
   public int ContentId { get; set; } 
   public string Name { get; set; }
   public int? Year { get; set; }

   public virtual Contentinformations? Contentinformation { get; set; }
   public Contents(){}
}