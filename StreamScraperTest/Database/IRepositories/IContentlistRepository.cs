using StreamScraperTest.Database.Models;

namespace StreamScraperTest.Database.IRepositories;

public interface IContentlistRepository<T>
{
    //evtl noch async machen
    public Task UpdateContent(List<T> FullActualContent);
    public List<Contents> getFullContent();
    public List<Contents> GetContentWithoutContentinformation();
}