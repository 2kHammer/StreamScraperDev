using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamScraperTest;
using StreamScraperTest.Database.Updater;
using StreamScraperTest.Models.ScrapingModels;
using StreamScraperTest.Scraping;
using StreamScraperTest.Scraping.Contentdatascraper;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Timeworker>();
        services.AddScoped<IContentdataScraper<SearchCriterias>, MoviepilotScraper>();
        services.AddScoped<IStreamingcontentScraper<SearchCriterias>, WerStreamtEsScraper>();

        
    })
    .Build();

host.Run();


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
