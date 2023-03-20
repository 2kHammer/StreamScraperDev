using Microsoft.Extensions.Logging;
using StreamScraperTest.Database.Models;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;

namespace StreamScraperTest.Database.Updater;

public class ContentdataUpdater
{
   //immer zuerst alles updaten wo noch keine Contentinformation vorhanden sind 
   private IContentdataScraper<SearchCriterias> _contentdatascraper;
   private ILogger<ContentdataUpdater> _logger;

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

   public ContentdataUpdater(IContentdataScraper<SearchCriterias> contentdatascraper, ILogger<ContentdataUpdater>logger)
   {
       _contentdatascraper = contentdatascraper;
       _logger = logger;
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

       List<SearchCriterias> criteriasDataWithoutInformation = ContentsToCriterias(withoutInformation);
       List<SearchCriterias> criteriasDataInformationNotFound = ContentsToCriterias(contentNotFound);
       List<SearchCriterias> criteriasDataScrapedLongestAgo = ContentsToCriterias(contentsLongestAgoScraped);
       List<SearchCriterias> criteriasToBeUpdatedOrInserted = new List<SearchCriterias>();
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataWithoutInformation);
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataInformationNotFound);
       criteriasToBeUpdatedOrInserted.AddRange(criteriasDataScrapedLongestAgo);
       try
       {
           ContentdataRepository datarep = new ContentdataRepository();
           List<ContentData> scrapedData = await _contentdatascraper.GetFullDataAsync(criteriasToBeUpdatedOrInserted);
           for (int i = 0; i < criteriasToBeUpdatedOrInserted.Count; i++)
           {
               //Console.WriteLine($"Scraped Data: {scrapedData[i].Contentname} und Contents: {contentToBeUpdatedOrInserted[i].Name}");
               await datarep.InsertOrUpdate(scrapedData[i], contentToBeUpdatedOrInserted[i].ContentId);
           }
            _logger.LogInformation("Contentinformation successfully updated");
           return true;
       }
       catch (Exception ex)
       {
           _logger.LogError($"Error updating Contentinformation: {ex}");
           return false;
       }
   }

   private List<SearchCriterias> ContentsToCriterias(List<Contents> listContent)
   {
       List<SearchCriterias> listCriteria = new List<SearchCriterias>();
       foreach (Contents c in listContent)
       {
           listCriteria.Add(new SearchCriterias(c.Name, c.Year.ToString()));
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
