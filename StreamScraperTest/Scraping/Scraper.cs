using PuppeteerSharp;

namespace StreamScraperTest.Scraping;

public abstract class Scraper
{
    protected IBrowser Browser { get; private set; }
    protected IPage Page { get; private set; }
    protected bool Headless { get; set; }
    protected Scraper(bool _headless)
    {
        Headless = _headless;
    }
    
    protected async Task GetBrowser()
    {
        Browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = true,
            ExecutablePath = "/usr/bin/google-chrome-stable",
            Args = new[]
            {
                "--disable-gpu",
                "--disable-dev-shm-usage",
                "--disable-setuid-sandbox",
                "--no-sandbox"
            }
        });
    }

    protected async Task GoToUrl(string url, bool newPage)
    {
        //Zu gewünschter Seite navigieren
        if(newPage) Page = await Browser.NewPageAsync();
        await Page.GoToAsync(url, WaitUntilNavigation.Load);
    }

    protected async Task GoToUrl(string url, string selectorCookies, string selectorWaitCookiesFinished, bool newPage)
    {
        await GoToUrl(url, newPage);

        WaitForSelectorOptions wfso = new WaitForSelectorOptions { Timeout = 6000 };
        try
        {
            await Page.WaitForSelectorAsync(selectorCookies, wfso);
            await Page.ClickAsync(selectorCookies);
            await Page.WaitForSelectorAsync(selectorWaitCookiesFinished);
        }
        //falls keine Cookies kommen
        catch (WaitTaskTimeoutException wtte)
        {
            //Console.WriteLine("NoCookies");
        }


    }
    
   
}