using Microsoft.EntityFrameworkCore;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Models;
using StreamScraperTest.Models.ScrapingModels;
using System;
using System.Globalization;

namespace StreamScraperTest.Database.Repositories;

public class ContentdataRepository : IContentdataRepository
{
    public async Task InsertOrUpdate(ContentData scrapeddata, int listid)
    {
        Contentinformations databasecontentinfo;
        bool alreadyExisted = false;
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            databasecontentinfo = await scc.contentinformation.FirstOrDefaultAsync(cinf => cinf.ContentId == listid);
        }

        Contentinformations actualcontentinfo;
        if (databasecontentinfo != null)
        {
            actualcontentinfo = databasecontentinfo;
            alreadyExisted = true;
        }
        else
        {
            //Man geht davon aus das der Typ, die Länder und die Genres konstant bleiben
            actualcontentinfo = new Contentinformations();
            // Elemente die kein Contentinfomartions besitzen wurden erst hinzugefügt, verfügbar auf Netflix
            actualcontentinfo.AvailableNetflix = true;
            //Hier kann man auch das Objekt aus der Datenbank suchen
            actualcontentinfo.ContentId = listid;
            //Type
            int typemapper = Contenttypes.mappingContenttypes[scrapeddata.Type];
            using (StreamScraperContext scc = new StreamScraperContext())
            {
                var acttype = await scc.contenttype.FindAsync(typemapper);
                actualcontentinfo.Contenttype = acttype;
            }
            //Country
            foreach (string countryname in scrapeddata.OriginCountries)
            {
                Countries? coun = await CheckIfCountryOrGenreExistsInDatabase<Countries>(countryname);
                if (coun == null) coun = new Countries(countryname);
                actualcontentinfo.Country.Add(coun);
            }
            //Fullname
            actualcontentinfo.Fullname = scrapeddata.Contentname;
            foreach (string genrename in scrapeddata.Genres)
            {
                Genres? gen= await CheckIfCountryOrGenreExistsInDatabase<Genres>(genrename);
                if (gen == null) gen= new Genres(genrename);
                actualcontentinfo.Genre.Add(gen);
            }
        }
        //Endyear, falls vorhanden
        int lastyearbuffer = -1;
        Int32.TryParse(scrapeddata.EndYear, out lastyearbuffer);
        if (lastyearbuffer != -1) actualcontentinfo.Lastyear = lastyearbuffer;
        //Rating
        float ratingbuffer = -1;
        float.TryParse(scrapeddata.Rating,NumberStyles.Any, CultureInfo.InvariantCulture, out ratingbuffer);
        if (ratingbuffer != -1) actualcontentinfo.Rating = ratingbuffer;
        //URL des Bildes
        actualcontentinfo.urlPicture = scrapeddata.urlPicture;
        //Time when last scraped
        actualcontentinfo.DateTimeDataScraped = scrapeddata.ActTime;
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            if (!alreadyExisted)
            {
                scc.contentinformation.Attach(actualcontentinfo);
            }
            else
            {
                scc.contentinformation.Update(actualcontentinfo);
            }
            int j = await scc.SaveChangesAsync();
            Console.WriteLine(j);
        }
    }

    public List<Contentinformations> GetNotFoundContentinformations()
    {
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            return scc.contentinformation.Where(coninf => coninf.urlPicture == null).ToList();
        }
    }

    public List<Contentinformations> GetLongestAgoScrapedContentinformations(int amount)
    {
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            return scc.contentinformation.Where(coninf => coninf.urlPicture != null).OrderBy(coninf => coninf.DateTimeDataScraped).ToList().GetRange(0, amount);
        }
    }

    //vielleicht noch zu einer Funktion umschreiben die die komplette String Liste auf einmal abarbeitet

    private async Task<T> CheckIfCountryOrGenreExistsInDatabase<T>(string Name) where T : class
    {
        T searched =null;
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            if (typeof(T) == typeof(Countries))
            {
                searched = await scc.country.FirstOrDefaultAsync(country => country.CountryName == Name) as T;
            } else if (typeof(T) == typeof(Genres))
            {
                searched = await scc.genre.FirstOrDefaultAsync(genre => genre.GenreName == Name) as T;
            }else
            {
                throw new Exception("False Type");
            }
            return searched;
        }
    }
    
}
    
