using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Scraping;

public class WerStreamtEsScraper : Scraper, IStreamingcontentScraper<SearchCriterias>
{
    private ILogger<WerStreamtEsScraper> _logger;

    public WerStreamtEsScraper(ILogger<WerStreamtEsScraper> logger) : base(true)
    {
        _logger = logger;
    }

    public async Task<List<SearchCriterias>> GetContentAsync()
    {
        StringBuilder url =
            new StringBuilder("https://www.werstreamt.es/filme-serien/anbieter-netflix/beliebt/?filterStart=0");
        await GetBrowser();
        await GoToUrl(url.ToString(), true);
        IElementHandle? endofNetflixContent = null;
        List<SearchCriterias> shownames = new List<SearchCriterias>();
        int oldamount = 0;

        while (/*endofNetflixContent == null*/ oldamount < 300)
        {
            var pageOfSeries = await Page.GetContentAsync();
            //Information holen mit Angle Sharp
            BrowsingContext ctxt = new BrowsingContext(Configuration.Default);
            var docOfSeries = await ctxt.OpenAsync(req => req.Content(pageOfSeries));
           shownames.AddRange(GetContentFromDocument(docOfSeries));
            //Zu nächster Seite gehen  
            int amount = (oldamount == 0) ? shownames.Count : shownames.Count + 1;
            url.Replace(oldamount.ToString(), amount.ToString());
            _logger.LogInformation($"Contentlistscraping already scraped: {amount}");
            oldamount = amount;
            //Wirft manchmal timeouts, besser geworden seit nicht mehr auf das Load Event sondern auf NetworkIdle2 gewartet wird
            await Page.GoToAsync(url.ToString(), WaitUntilNavigation.Networkidle2);
            //Wartet jetzt jedesmal bis "Worum geht es" im footer geladen ist
            //await Page.WaitForSelectorAsync("body > div.Layout > div > section > div > div > h1");
            //Nochmal anschauen ob man so alles erwischt
            endofNetflixContent =
                await Page.QuerySelectorAsync("body > div.Layout > div > section > div > div > div > div > h5");
        }

        await Browser.CloseAsync();

        return shownames;
    }

    private List<SearchCriterias> GetContentFromDocument(IDocument docOfSeries)
    {
            IEnumerable<IElement> contentDetails = docOfSeries.QuerySelectorAll("div")
                .Where(elem => elem.GetAttribute("class") == "details");
            IEnumerable<string> namesList = contentDetails.Select(elem => elem.Children.First().InnerHtml);
            IEnumerable<string> yearsList =
                contentDetails.Select(elem => Regex.Match(elem.Children[2].InnerHtml, ".*, (\\d{4})").Groups[1].Value);
            List<SearchCriterias> content = new List<SearchCriterias>();
            for (int i = 0; i < namesList.Count(); i++)
            {
                content.Add(new SearchCriterias(namesList.ToList()[i], yearsList.ToList()[i]));
            }

            return content;
    }
}