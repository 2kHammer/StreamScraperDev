
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PuppeteerSharp;
using StreamScraperTest;
using StreamScraperTest.Buffer;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Models;
using StreamScraperTest.Database.Repositories;
using StreamScraperTest.Database.Updater;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;
using StreamScraperTest.Scraping.Contentdatascraper;




IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Timeworker>();
        services.AddScoped<IContentdataScraper<Tuple<string, string>>, MoviepilotScraper>();
        services.AddScoped<IStreamingcontentScraper<Tuple<string, string>>, WerStreamtEsScraper>();
    })
    .Build();

host.Run();


/*WerStreamtEsScraper namescraper = new WerStreamtEsScraper(true);
List<Tuple<string,string>>test3 = await namescraper.GetContentAsync();
DataBuffer<List<Tuple<string,string>>>.Bufferdata(test3, "Contentnames.dat");*/


//var test3 = DataBuffer<List<Tuple<string, string>>>.Restoredata("Contentnames.dat");

/*
 * Probleme:
 *  - ganzer Name gefällt mir nicht (stimmt aber anscheinend)
 *  - AvailableOnNext gehört in die Contents Tabelle eventuell
 *  - Das Parameter mit den Tupel in Moviepilot Scraper muss eventuell umgeschrieben werden
 *  - Ginny &amp Georgia: amp entfernen
 *  - hoher Ram Verbrauch: bei 80 Prozent aktuell
 *  - bei manchen Namen steht ein \n danach, Leerzeichen davor und danach entfernen
 * To Do
 *  - Contentdataupdater abschließen
 *      - Problem: bei neues Scrapen der noch nicht gefunden Contentinformation wurden diese neu hinzugefügt (wenn sich Typ geändert hat)
 *      - macht er nicht immer sondern nur machmal
 *      - bei ändern der Url wurde es nicht neu hinzugefügt, vielleicht wird es nur bei bestimmten Attributen neu hinzugefügt
 *  - Wert ändern und dann testen, vielleicht findet man den Fehler durch fertig schreiben des Codes (alte Contentinformations updaten)
 *  - Contentlistupdater mit try catch schließen
 *  - passende Zeitpunkte per Cronjob festlegen
 *  - Containerizen und erster test
 */ 

/*

MoviepilotScraper contentscraper = new MoviepilotScraper(true);
List<ContentData> data  = await contentscraper.GetFullDataAsync(test3);

DataBuffer<List<ContentData>>.Bufferdata(data, "CompleteData.dat");*/

/*IContentdataScraper<Tuple<string,string>> mpScraper = new MoviepilotScraper(true);
var data = await mpScraper.GetDataAsync(new Tuple<string, string>("Saw", "2004"));

Console.WriteLine(data.PublicationYear);*/

/* Nochmal anschauen ob die Behandlung passt fals kein Element gefunden wird */

/* Komisches Verhalten bei Headless Browser und Moviepilot: es wird nicht nach cookies gefragt,
   erstmal geklärt über try/catch, aber er warte ziemlich lange auf cookie selector (evtl timeout
   ändern)*/

