using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Plantpedia.Helper
{
    public static class EnumHelper
    {
        public static string GetDisplayName(System.Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    return displayAttr.Name!;
                }
            }

            return enumValue.ToString();
        }
    }
}
