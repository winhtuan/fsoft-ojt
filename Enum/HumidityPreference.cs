using System.ComponentModel.DataAnnotations;

namespace Plantpedia.Enum;

public enum HumidityPreference
{
    [Display(Name = "Cao - Giữ đất luôn ẩm")]
    Cao,

    [Display(Name = "Trung bình - Tưới khi bề mặt đất khô")]
    TrungBinh,

    [Display(Name = "Thấp - Tưới khi đất khô hoàn toàn")]
    Thap,
}
