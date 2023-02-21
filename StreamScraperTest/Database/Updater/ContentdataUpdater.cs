using StreamScraperTest.Database.Models;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class ContentdataUpdater
{
   //immer zuerst alles updaten wo noch keine Contentinformation vorhanden sind 
   private IContentdataScraper<Tuple<string, string>> _contentdatascraper;

   private int ScrapedNewAtOnce = 3;
   private int ScrapedNotFoundAtOnce = 1;
   private int ScrapedLongestAgoAtOnce = 2; 
   private enum StateOfContentinformations{
       New,
       NotFound,
       Contained
   }

   private Dictionary<StateOfContentinformations, int> ScrapedAtOnce = new Dictionary<StateOfContentinformations, int>()
   {
       { StateOfContentinformations.New, 3 },
       { StateOfContentinformations.NotFound, 1 },
       { StateOfContentinformations.Contained, 2 }
   };

   public ContentdataUpdater(IContentdataScraper<Tuple<string, string>> contentdatascraper)
   {
       _contentdatascraper = contentdatascraper;
   }

   //Funktion von des Contentdata Repository kann man auf Insert oder Updata aufteilen
   public async Task<bool> updatePartOfContentData()
   {
       List<Contents> contentToBeUpdatedOrInserted = new List<Contents>();
       
       //Contents ohne Contentinformations
       List<Contents> withoutInformation = GetRightContent(StateOfContentinformations.New);
      contentToBeUpdatedOrInserted.AddRange(withoutInformation);

       //Contents die nicht gefunden wurden (URL null)
       List<Contents> contentNotFound = GetRightContent(StateOfContentinformations.NotFound);
       contentToBeUpdatedOrInserted.AddRange(contentNotFound);
       
       //Contents deren scrapen schon am längsten her ist
       List<Contents> contentsLongestAgoScraped = GetRightContent(StateOfContentinformations.Contained);
       contentToBeUpdatedOrInserted.AddRange(contentsLongestAgoScraped);

       List<Tuple<string, string>> criteriasDataWithoutInformation = ContentToCriteria(withoutInformation);
       List<Tuple<string, string>> criteriasDataInformationNotFound = ContentToCriteria(contentNotFound);
       List<Tuple<string, string>> criteriasDataScrapedLongestAgo = ContentToCriteria(contentsLongestAgoScraped);
       List<Tuple<string, string>> criteriasToBeUpdatedOrInserted = new List<Tuple<string, string>>();
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataWithoutInformation);
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataInformationNotFound);
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataScrapedLongestAgo);
       try
       {
           ContentdataRepository datarep = new ContentdataRepository();
           List<ContentData> scrapedData = await _contentdatascraper.GetFullDataAsync(criteriasToBeUpdatedOrInserted);
           for (int i = 0; i < criteriasToBeUpdatedOrInserted.Count; i++)
           {
               Console.WriteLine($"Scraped Data: {scrapedData[i].Contentname} und Contents: {contentToBeUpdatedOrInserted[i].Name}");
               await datarep.InsertOrUpdate(scrapedData[i], contentToBeUpdatedOrInserted[i].ContentId);
           }

           return true;
       }
      /*try
      {
          //Test warum Manta Manta nochmal eingefügt wird
          ContentdataRepository datarep = new ContentdataRepository();
          var test = await _contentdatascraper.GetDataAsync(new Tuple<string, string>("Manta Manta", "1991"));
          await datarep.InsertOrUpdate(test, 22);
          return true;
      }*/
       catch (Exception ex)
       {
           Console.WriteLine(ex);
           return false;
       }
   }

   private List<Tuple<string, string>> ContentToCriteria(List<Contents> listContent)
   {
       List<Tuple<string, string>> listCriteria = new List<Tuple<string, string>>();
       foreach (Contents c in listContent)
       {
           listCriteria.Add(new Tuple<string, string>(c.Name, c.Year.ToString()));
       }

       return listCriteria;
   }

   private List<Contents> GetRightContent(StateOfContentinformations stateContentinformations)
   {
       List<Contents> neededContent = new List<Contents>();
       int contentlistlength = ScrapedAtOnce[stateContentinformations];
       ContentlistRepository listrep = new ContentlistRepository();
       ContentdataRepository datarep = new ContentdataRepository();
       List<Contents> fullContent = listrep.getFullContent();
       switch (stateContentinformations)
       {
           case(StateOfContentinformations.New):
               neededContent = listrep.GetContentWithoutContentinformation();
               break;
           case(StateOfContentinformations.NotFound):
               List<Contentinformations> contentinformationsNotFound = datarep.GetNotFoundContentinformations();
               List<int> contentindexesContentinformationsNotFound = contentinformationsNotFound.Select(x => x.ContentId).ToList();
               neededContent = fullContent
                   .Where(con => contentindexesContentinformationsNotFound.Contains(con.ContentId)).ToList();
               break;
           case(StateOfContentinformations.Contained):
               List<Contentinformations> contentinformationsLongestAgoScraped = datarep.GetLongestAgoScrapedContentinformations(ScrapedLongestAgoAtOnce);
               List<int> contentindexesContentinformationsLongestAgoScraped = contentinformationsLongestAgoScraped.Select(coninf => coninf.ContentId).ToList();
               neededContent = fullContent.Where(con => contentindexesContentinformationsLongestAgoScraped.Contains(con.ContentId)).ToList();
               break;
       }

       if (neededContent.Count >= contentlistlength)
       {
           neededContent = neededContent.GetRange(0, contentlistlength);
       }

       return neededContent;
   }
}
