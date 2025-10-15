// ViewModel/AdminUserQuery.cs
using Plantpedia.Enum;

public class AdminUserQuery
{
    public string? Q { get; set; } // search theo username/email/name
    public UserStatus? Status { get; set; } // lọc trạng thái
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; }
    public int Total { get; set; }
}
