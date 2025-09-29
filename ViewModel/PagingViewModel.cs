namespace Plantpedia.ViewModel
{
    public class PagingViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public Func<int, string> GenerateUrl { get; set; } = page => string.Empty;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
