using System.Text.RegularExpressions;
using PuppeteerSharp;
using AngleSharp;
using AngleSharp.Dom;
using StreamScraperTest.Models;
using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Scraping.Contentdatascraper;

//Vielleicht das man das nach Anbieter aufteilt (abstrakte Klasse Streamingdatascraper, wo alle Anbieter abgeleitet sind (vor allem da Get Rating immer ähnlich ist)
public class MoviepilotScraper : Scraper, IContentdataScraper<Tuple<string, string>>
{
    public MoviepilotScraper() : base(true)
    {
    }


    public async Task<ContentData> GetDataAsync(Tuple<string, string> criteria)
    {
        await GetBrowser();
        string searchingElem = criteria.Item1;
        string url = "https://www.moviepilot.de/suche?q=";
        string selectorCookies = "#didomi-notice-agree-button";
        string selectorCookiesClicked = ".sc-89gwi2-6.iIqMSX";
        await GoToUrl(url + searchingElem, selectorCookies, selectorCookiesClicked, true);

        bool foundElementWithRightYear = await clickRightElement(criteria.Item2);
        //
        if (!foundElementWithRightYear)
            return new ContentData() { Contentname = criteria.Item1, NotFound = true, ActTime = DateTime.Now};


        ContentData contentdata = await HandleMoviepilotSearch(searchingElem);
        await Browser.CloseAsync();
        /*if (!contentdata.NotFound)
            Console.WriteLine(
                $"Name: {contentdata.Contentname},Rating: {contentdata.Rating},Jahre: {contentdata.PublicationYear}, Genre: {contentdata.Genres.First()}, {contentdata.Genres[1]}");*/
        contentdata.ActTime = DateTime.Now;
        return contentdata;
    }

    //Funktioniert akutell nicht da ich die Navigation von HandleMoviepilotSearch zu clickRightElement verschoben habe
    public async Task<List<ContentData>> GetFullDataAsync(List<Tuple<string, string>> criterias)
    {
        List<ContentData> completeData = new List<ContentData>();
        await GetBrowser();

        //Handle Cookies
        string url = "https://www.moviepilot.de/suche?q=";
        string selectorCookies = "#didomi-notice-agree-button";
        string selectorCookiesClicked = ".sc-89gwi2-6.iIqMSX";
        await GoToUrl(url + "Dark", selectorCookies, selectorCookiesClicked, true);

        try
        {

            int amount = 0;
            foreach (Tuple<string, string> criteria in criterias)
            {
                //Problem: manchmal kommen hier Cookies, manchmal nicht
                await GoToUrl(url + criteria.Item1, selectorCookies, selectorCookiesClicked, false);
                bool foundElementWithRightYear = await clickRightElement(criteria.Item2);
                ContentData contentdata;
                if (!foundElementWithRightYear)
                {
                    contentdata = new ContentData() { Contentname = criteria.Item1, NotFound = true, Type = Contenttype.Unknown};
                }
                else
                {
                    contentdata = await HandleMoviepilotSearch(criteria.Item1);
                    if (!contentdata.NotFound)
                    {
                        Console.WriteLine(
                            $"{amount}. Name: {contentdata?.Contentname},Rating: {contentdata?.Rating},Jahre: {contentdata?.PublicationYear}, Genre: {contentdata?.Genres.First()}, {contentdata.Genres.LastOrDefault("nur ein genre")}");
                    }
                    else
                    {
                        Console.WriteLine($"{contentdata.Contentname} not found");
                    }
                }
                contentdata.ActTime = DateTime.Now;
                completeData.Add(contentdata);
                amount++;
            }

            await Browser.CloseAsync();
            return completeData;
        }
        catch (Exception ex)
        {
            await Browser.CloseAsync();
            throw ex;
        }
        /*List<ContentData> data = new List<ContentData>();
        int i = 0;
        while (i < criterias.Count)
        {
            ContentData buffer = await GetDataAsync(criterias[i]);
            //var test = await Task.WhenAll(GetRatingAsync(criterias[i]), GetRatingAsync(criterias[i + 1]));
            if (!buffer.NotFound)
            {
                Console.WriteLine(
                     $"{i}:  Name: {buffer.Contentname},Rating: {buffer.Rating},Jahre: {buffer.PublicationYear}, Genre: {buffer.Genres.First()}, {buffer.Genres.LastOrDefault("nur ein genre")}");
            }
            else
            {
                Console.WriteLine($"{i}:{criterias[1].Item1} not found");
            }

            data.Add(buffer);
            i++;
        }

        return data;*/
    }

    private ContentData getDataFromDoc(Contenttype typ, IDocument doc, string contentname)
    {
        ContentData data = new ContentData();
        data.Type = typ;

        data.Rating = doc.QuerySelector(
                "#header > div.layout--content-width.layout--background > div > div.grid--col-sm-12 > div.header--poster > div > meta:nth-child(5)")
            ?.GetAttribute("content");

        data.Contentname = doc.QuerySelector(
                "#header > div.layout--content-width.layout--background > div > div.grid--col-sm-12 > div.meta > h1")
            ?.InnerHtml;
        //Enter vor und nach Name entfernen
        char[] removeStartEnd = new[] { '\n', ' ' };
        data.Contentname = data.Contentname?.TrimEnd(removeStartEnd).TrimStart(removeStartEnd);

        if (data.Contentname == null)
        {
            return new ContentData { NotFound = true, Contentname = contentname };
        }
        else
        {
            IEnumerable<string?> countries = doc.QuerySelectorAll("span")
                .Where(span => span.GetAttribute("itemprop") == "countryOfOrigin")?.Select(elem => elem.InnerHtml);
            IEnumerable<string?> genres = doc.QuerySelectorAll("span")
                .Where(span => span.GetAttribute("itemprop") == "genre")?.Select(elem => elem.InnerHtml);
            string yearspan = doc.QuerySelectorAll("span")
                .Where(span => span.GetAttribute("itemprop") == "copyrightYear").FirstOrDefault()?.InnerHtml;
            data.urlPicture = doc.QuerySelectorAll("img")
                .Where(img => img.GetAttribute("class") == "poster--image")?.FirstOrDefault()?
                .GetAttribute("src");
            data.Genres.AddRange(genres);
            data.OriginCountries.AddRange(countries);
            if(yearspan != null)data.PublicationYear = Regex.Match(yearspan, "\\d{4}").Value;
            if(yearspan != null) data.EndYear = Regex.Match(yearspan,"\\d{4}").NextMatch().Value;
            return data;
        }
    }


    //Gets the data if the Page show the moviepilot search
    private async Task<ContentData> HandleMoviepilotSearch(string actcontentname)
    {
        try
        {
            //Get the type (movie, series) from url
            string type = Regex.Match(Page.Url, "\\/([a-z]{5,6})\\/").Groups[1].ToString();
            Contenttype typecontent = Contenttype.Unknown;
            if (type == "movies")
            {
                typecontent = Contenttype.Movie;
            }
            else if (type == "serie")
            {
                typecontent = Contenttype.Series;
            }

            var pageOfSeries = await Page.GetContentAsync();
            //Information holen mit Angle Sharp
            BrowsingContext ctxt = new BrowsingContext(Configuration.Default);
            var pagedoc = await ctxt.OpenAsync(req => req.Content(pageOfSeries));

            //HTML Parsen nach H5, geht eigentlich ganz gut
            return getDataFromDoc(typecontent, pagedoc, actcontentname);
        }
        catch (WaitTaskTimeoutException)
        {
            Page.WaitForNavigationAsync();
            return new ContentData { NotFound = true, Contentname = actcontentname };
        }
    }

    private async Task<bool> clickRightElement(string year)
    {
        //Diese 3 Zeilen kann man noch in einer Methode in Scraper abstrahieren
        var pageOfSeries = await Page.GetContentAsync();
        BrowsingContext ctxt = new BrowsingContext(Configuration.Default);
        var pagedoc = await ctxt.OpenAsync(req => req.Content(pageOfSeries));

        var divYears = pagedoc.QuerySelectorAll("div")
            .Where(elem => elem.GetAttribute("class") == "sc-89gwi2-5 kLwtza");
        var years = divYears.Select(x => Regex.Match(x.InnerHtml, "(\\d{4}) \\|").Groups[1].Value).ToList();

        //Falls kein Element gefunden wurde
        if (years.Count == 0) return false;

        int indexRightElem = 0;
        while (years[indexRightElem] != year)
        {
            indexRightElem++;

            if (indexRightElem >= years.Count)
            {
                Console.WriteLine($"IndexOutOfBounds {indexRightElem}");
                break;
            }

            Console.WriteLine($"{year} und {years[indexRightElem]}");
        }

        if (indexRightElem < years.Count)
        {
            /*Hier wird ein Navigationsfehler geworfen, wahrscheinlich weil das wait for Navigation beim Timeout nicht ausgeführt wird*/
            await Page.WaitForSelectorAsync(".sc-89gwi2-6.iIqMSX", new WaitForSelectorOptions { Timeout = 800 });
            var searchresults = await Page.QuerySelectorAllAsync(".sc-89gwi2-6.iIqMSX");
            await searchresults[indexRightElem].ClickAsync();
            await Page.WaitForNavigationAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
}



