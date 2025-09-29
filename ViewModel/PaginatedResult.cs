namespace Plantpedia.ViewModel;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public PagingViewModel PagingInfo { get; set; } = new PagingViewModel();
}
