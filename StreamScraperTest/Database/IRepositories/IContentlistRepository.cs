namespace StreamScraperTest.Database.IRepositories;

public interface IContentlistRepository
{
    //evtl noch async machen
    public Task UpdateContent(List<Tuple<string, string>> FullActualContent);
}