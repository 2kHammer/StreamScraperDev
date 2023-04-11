using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamScraperTest.Buffer;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Models;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping.Contentdatascraper;
using Cronos;
using PuppeteerSharp;
using StreamScraperTest.Database.Updater;
using StreamScraperTest.Scraping;

namespace StreamScraperTest;

public sealed class Timeworker : BackgroundService
{
    private readonly ILogger<Timeworker> _logger;
    
    //Cronjobs dürfen sich nicht überschneiden, funktioniert sonst nicht
    private readonly string cronjobContentlist;
    private readonly string cronjobContentdata;
    private readonly ContentlistUpdater contentupdater;
    private readonly ContentdataUpdater dataupdater;
   


    public Timeworker(ILogger<Timeworker> logger, IContentlistScraper<SearchCriterias> streamingcontentscraper, IContentdataScraper<SearchCriterias> contentdatascraper, ILogger<ContentlistUpdater> loggercontent, ILogger<ContentdataUpdater> loggerdata)
    {
        _logger = logger;
        /*cronjobContentlist = "34 * * * *";
        cronjobContentdata = "6,8,10,12,14,16 * * * *";*/
        //Um 3 und 4 Uhr am Freitag Samstag Sonntag
         cronjobContentlist = "0 3,4 * * 5,6,7";
         //zu den jeweiligen Minuten um 3,4 Uhr am Montag , Dienstag, Mittwoch, Donnerstag, Freitag
        cronjobContentdata = "0,6,12,18,24,30,36,42,48,54 3,4 * * 1,2,3,4";
        contentupdater = new ContentlistUpdater(streamingcontentscraper, loggercontent);
        dataupdater = new ContentdataUpdater(contentdatascraper, loggerdata);
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartAsync has been called.");
        //Installs Chromium for Webscrapping
        using (var browserFetcher = new BrowserFetcher()){
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        }
        _logger.LogInformation("1.a) Chrome Instance is checked");
        base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await TestScrapeDatabaseAdd();
        
        while (!stoppingToken.IsCancellationRequested)
        { 
            // zu Debug Zwecken deaktiviert da man nicht immer auf die aktuelle Zeit warten will
           // int test = await WaitForNextSchedule(cronjob1, cronjob2);
           // _logger.LogInformation($"task {test} done at {DateTime.Now}");
           //await contentupdater.updateStreamingContent();
           int completedTask = await WaitForNextSchedule(cronjobContentlist, cronjobContentdata);
           if (completedTask == 1)
           {
               await contentupdater.updateStreamingContent();
           }
           else if (completedTask == 2)
           {
                await dataupdater.updatePartOfContentData();     
           }
          
        }


        //google: c# scheduler in background service
    }

    //Quelle für den Scheduler: httpos://ankitvijay.net/2021/02/22/a-poor-mans-scheduler-using-net-background-serivce/
    // vielleicht muss man die Lebenszeit dieses Service noch zu Scoped machen, siehe Quelle
    private async Task<int> WaitForNextSchedule(string cronExpression1, string cronExpression2)
    {
        CronExpression? parsedExp1 = CronExpression.Parse(cronExpression1);
        CronExpression? parsedExp2 = CronExpression.Parse(cronExpression2);

        var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;

        DateTime? occurrenceTime1 = parsedExp1.GetNextOccurrence(currentUtcTime);
        DateTime? occurrenceTime2 = parsedExp2.GetNextOccurrence(currentUtcTime);

        TimeSpan delay1 = occurrenceTime1.GetValueOrDefault() - currentUtcTime;
        TimeSpan delay2 = occurrenceTime2.GetValueOrDefault() - currentUtcTime;
        //test
        if (delay1 < delay2)
        {
            await Task.Delay(delay1);
            return 1;
        }
        else
        {
            await Task.Delay(delay2);
            return 2;
        }
        
    }

    private async Task TestScrapeDatabaseAdd()
    {
        /*ContentlistRepository contentlistrep = new ContentlistRepository();
        List<Contents> nextcon = new List<Contents>();
        List<Tuple<string, string>> nextcrit = new List<Tuple<string, string>>();
        for (int i = 11; i < 12; i++)
        {
            nextcon.Add(await contentlistrep.getContent(i));
            nextcrit.Add(new Tuple<string, string>(nextcon.Last().Name, nextcon.Last().Year.ToString()));
        }
        _logger.LogInformation("Content is queried from Database");
        MoviepilotScraper ms = new MoviepilotScraper(true);
        var data = await ms.GetFullDataAsync(nextcrit);
        DataBuffer<List<ContentData>>.Bufferdata(data, "ContentData.dat");
        _logger.LogInformation("Contentinformation is scraped and buffered");
        IContentdataRepository contentdatrep = new ContentdataRepository();
        for (int i = 0; i < 1; i++)
        {
            await contentdatrep.InsertOrUpdate(data[i], nextcon[i].ContentId);
        }

        _logger.LogInformation("Contentinformation is inserted/updated");*/
    }
}