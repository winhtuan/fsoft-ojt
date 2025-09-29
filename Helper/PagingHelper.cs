using Plantpedia.ViewModel;

namespace Plantpedia.Helper;

public static class PagingHelper
{
    public static PaginatedResult<T> Paginate<T>(
        this IEnumerable<T> source,
        int pageNumber,
        int pageSize,
        Func<int, string> urlGenerator
    )
    {
        int totalItems = source.Count();
        int totalPages = (totalItems > 0) ? (int)Math.Ceiling((double)totalItems / pageSize) : 0;

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }
        if (pageNumber > totalPages && totalPages > 0)
        {
            pageNumber = totalPages;
        }

        var itemsForCurrentPage = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var pagingInfo = new PagingViewModel
        {
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            GenerateUrl = urlGenerator,
        };

        return new PaginatedResult<T> { Items = itemsForCurrentPage, PagingInfo = pagingInfo };
    }
}
