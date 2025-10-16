using System.Linq.Expressions;

namespace Plantpedia.Queries;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate
    )
    {
        if (condition)
        {
            return source.Where(predicate);
        }
        return source;
    }

    public static List<string> MapJoinCollectionToNames<TJoinEntity, TNavEntity>(
        ICollection<TJoinEntity>? joinCollection,
        Func<TJoinEntity, TNavEntity?> navigationSelector,
        Func<TNavEntity, string> nameSelector
    )
        where TNavEntity : class
    {
        // Nếu tập hợp đầu vào là null, trả về một danh sách rỗng.
        if (joinCollection == null)
        {
            return new List<string>();
        }

        // Thực hiện logic LINQ
        return joinCollection
            .Select(navigationSelector) // Lấy ra đối tượng điều hướng (ví dụ: Region, SoilType)
            .Where(navEntity => navEntity != null) // Lọc bỏ những đối tượng điều hướng bị null
            .Select(navEntity => nameSelector(navEntity!) ?? "Không rõ") // Lấy tên và cung cấp giá trị mặc định
            .ToList();
    }
}
