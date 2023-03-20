using Microsoft.EntityFrameworkCore;
using StreamScraperTest.Database.IRepositories;
using StreamScraperTest.Database.Models;
using StreamScraperTest.Models.ScrapingModels;

namespace StreamScraperTest.Database.Repositories;

public class ContentlistRepository : IContentlistRepository<SearchCriterias>
{
    public ContentlistRepository()
    {
        
    }
    public async Task UpdateContent(List<SearchCriterias> FullActualContent)
    {
        List<Contents> dbcontent  = null; 
        using (StreamScraperContext ssc = new StreamScraperContext())
        {
             dbcontent = ssc.content.Select(x => x).ToList();
        }

        List<Contents> contentNotInDb = CheckIfInTable(FullActualContent, dbcontent);
        await UpdateAvailability(FullActualContent, dbcontent);
        await InsertContents(contentNotInDb);
    }
    
    public List<Contents> getFullContent()
    {
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            return scc.content.ToList();
        }
    }

    public async Task<Contents> getContent(int id)
    {
        using (StreamScraperContext scc = new StreamScraperContext())
        {
            return await scc.content.FindAsync(id);
        }
    }

    public List<Contents> GetContentWithoutContentinformation()
    {
        using (StreamScraperContext ssc = new StreamScraperContext())
        {
            return ssc.content.Where(con => con.Contentinformation == null).ToList();
        }
    }

    private List<Contents> CheckIfInTable(List<SearchCriterias> actaulContent, List<Contents> contentdb)
    {
        List<Contents> newContent = new List<Contents>();
        foreach (var content in actaulContent)
        {
            int year = 0;
            Int32.TryParse(content.PublicationYear, out year);
            var contentinDB = contentdb.Where(condb => (condb.Name == content.ContentName) && (condb.Year == year));
            if(contentinDB.Count() == 0) newContent.Add(new Contents(){Name = content.ContentName, Year = (year != 0)? year : null });
        }
        return newContent;
    }

    private async Task<List<Contents>> InsertContents(List<Contents> insertContent)
    {
        using (StreamScraperContext ssc = new StreamScraperContext())
        {
            ssc.AttachRange(insertContent);
            await ssc.content.AddRangeAsync(insertContent);
            await ssc.SaveChangesAsync();
        }

        return insertContent;
    }

    /*bei Contents für die es schon Contentinformationen gibt wird update ich die Verfügbarkeit in
    Netflix zu falsch für die, die nicht mehr verfügbar sind 
    für die wo neue Contentinformations hinzugefügt werden ist die verfügbarkeit automatisch true*/
    private async Task UpdateAvailability(List<SearchCriterias> actualcontent, List<Contents> contentdb)
    {
        //Finden der Titel die nicht mehr bei Netflix verfügbar sind
        List<Contents> notinacutalcontent = new List<Contents>();
        foreach (var content in contentdb)
        {
            var contentInActualContent = actualcontent.Where(x => x.ContentName== content.Name && x.PublicationYear== content.Year.ToString());
            if (contentInActualContent.Count() == 0)
            {
                notinacutalcontent.Add(content);
            }
        }
        //Ändern der Verfügbarkeit in der Datenbank
        foreach (var notonnetflix in notinacutalcontent)
        {
            using (StreamScraperContext scc = new StreamScraperContext())
            {
                Contentinformations coninf = await scc.contentinformation.FirstOrDefaultAsync(coninf => coninf.ContentId == notonnetflix.ContentId);
                //Verfügbarkeit wird nur Upgedatet wenn es schon Contentinformation dazu gibt
                if (coninf != null)
                {
                    coninf.AvailableNetflix = false;
                    scc.contentinformation.Update(coninf);
                    await scc.SaveChangesAsync();
                }

            } 
            
        }
    }

}