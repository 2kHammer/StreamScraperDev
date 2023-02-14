using StreamScraperTest.Database.Models;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class ContentdataUpdater
{
   //immer zuerst alles updaten wo noch keine Contentinformation vorhanden sind 
   private IContentdataScraper<Tuple<string, string>> _contentdatascraper;
   
   public int ScrapedAtOnce { get; }

   public ContentdataUpdater(IContentdataScraper<Tuple<string, string>> contentdatascraper)
   {
       _contentdatascraper = contentdatascraper;
       ScrapedAtOnce = 5;
   }

   //Funktion von des Contentdata Repository kann man auf Insert oder Updata aufteilen
   public async Task<bool> updatePartOfContentData()
   {
       
       List<Contents> contentInformationToBeUpdatedOrInserted = new List<Contents>();
       ContentlistRepository listrep = new ContentlistRepository();
       //Contents ohne Contentinformations
       List<Contents> withoutInformation = listrep.GetContentWithoutContentinformation();
       if (withoutInformation.Count != 0)
       {
           contentInformationToBeUpdatedOrInserted.AddRange(withoutInformation.GetRange(0,ScrapedAtOnce));
       }
       //Contents die nicht gefunden wurden (URL null)
       ContentdataRepository datarep = new ContentdataRepository();
       List<Contentinformations> contentinformationsNotFound = datarep.GetNotFoundContentinformations();
       List<Contents> contentNotFound =
           listrep.getContentlist(contentinformationsNotFound.Select(coninf => coninf.ContentId).ToList());
       
       //Contents deren scrapen schon am längsten her ist
       List<Contentinformations> contentinformationsLongestAgoScraped =
           datarep.GetLongestAgoScrapedContentinformations(ScrapedAtOnce);
       //erstmal Fehler schauen sie Programm.cs
       //dies dann fertig Programmieren
       //vielleicht nur einmal den kompletten Content ganz oben holen und dann die Aufteilung machen (wahrsscheinlich am besten)


       List<Tuple<string, string>> criteriasDataWithoutInformation = new List<Tuple<string, string>>();
       foreach (Contents con in contentInformationToBeUpdatedOrInserted)
       {
           criteriasDataWithoutInformation.Add(new Tuple<string, string>(con.Name, con.Year.ToString()));
       }

       List<Tuple<string, string>> criteriasDataInformationNotFound = new List<Tuple<string, string>>();
       foreach (Contents con in contentNotFound)
       {
           criteriasDataInformationNotFound.Add(new Tuple<string, string>(con.Name, con.Year.ToString()));
       }

       try
       {
           List<ContentData> scrapedData = await _contentdatascraper.GetFullDataAsync(criteriasDataInformationNotFound);
           for (int i = 0; i < ScrapedAtOnce; i++)
               await datarep.InsertOrUpdate(scrapedData[i], contentInformationToBeUpdatedOrInserted[i].ContentId);
           return true;
       }
       catch (Exception ex)
       {
           Console.WriteLine(ex);
           return false;
       }
   } 
}