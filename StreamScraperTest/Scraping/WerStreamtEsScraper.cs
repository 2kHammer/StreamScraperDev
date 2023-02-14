using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using AngleSharp;
using PuppeteerSharp;

namespace StreamScraperTest.Scraping;

public class WerStreamtEsScraper:Scraper, IStreamingcontentScraper<Tuple<string,string>>
{
    public WerStreamtEsScraper(): base(true)
    {
    }

    public async Task<List<Tuple<string,string>>> GetContentAsync()
    {
        StringBuilder url =
            new StringBuilder("https://www.werstreamt.es/filme-serien/anbieter-netflix/beliebt/?filterStart=0");
        await GetBrowser();
        await GoToUrl(url.ToString(), true);
        IElementHandle? endofNetflixContent = null;
        List<Tuple<string, string>> shownames = new List<Tuple<string,string>>();
        int oldamount = 0;

        while (/*endofNetflixContent == null*/ oldamount < 1000)

    {
        /*var elements = await page.QuerySelectorAllAsync("strong[itemprop=name]");
        var strings = await Task.WhenAll(elements.Select(async elem => await elem.GetPropertyAsync("innerText")).ToList());
        var names = await Task.WhenAll(strings.Select(async x => await x.JsonValueAsync()));
        var nameslist = names.ToList();
        shownames.AddRange(nameslist);*/
   
        var pageOfSeries = await Page.GetContentAsync();
        //Information holen mit Angle Sharp
        BrowsingContext ctxt = new BrowsingContext(Configuration.Default);
        var docOfSeries = await ctxt.OpenAsync(req => req.Content(pageOfSeries));
        //HTML Parsen nach H5, geht eigentlich ganz gut
        //Veraltet: var names = docOfSeries.QuerySelectorAll("strong").Where(elem => elem.GetAttribute("itemprop") == "name");
        var contentDetails = docOfSeries.QuerySelectorAll("div").Where(elem => elem.GetAttribute("class") == "details");
        var nameslist = contentDetails.Select(elem => elem.Children.First().InnerHtml);
        var yearslist =
            contentDetails.Select(elem => Regex.Match(elem.Children[2].InnerHtml, ".*, (\\d{4})").Groups[1].Value);
        for (int i = 0; i < nameslist.Count(); i++)
        {
            shownames.Add(new Tuple<string, string>(nameslist.ToList()[i], yearslist.ToList()[i])); 
            //Console.WriteLine($"{nameslist.ToList()[i]}, {yearslist.ToList()[i]}");
        }
          
        //Zu nächster Seite gehen  
        int amount = (oldamount == 0) ? shownames.Count : shownames.Count + 1;
        url.Replace(oldamount.ToString(), amount.ToString());
        Console.WriteLine($"{amount} and {oldamount}");
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
}
