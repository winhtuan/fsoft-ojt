using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plantpedia.Helper
{
    public static class DropdownHelper
    {
        public static List<SelectListItem> ToSelectListItem<T>(
            this IEnumerable<T> source,
            Func<T, string> valueSelector,
            Func<T, string> textSelector
        )
        {
            return source
                .Select(x => new SelectListItem
                {
                    Value = valueSelector(x),
                    Text = textSelector(x),
                })
                .ToList();
        }
    }
}
