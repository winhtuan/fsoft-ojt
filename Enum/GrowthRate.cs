using System.ComponentModel.DataAnnotations;

namespace Plantpedia.Enum;

public enum GrowthRate
{
    [Display(Name = "Nhanh")]
    Nhanh,

    [Display(Name = "Trung Bình")]
    TrungBinh,

    [Display(Name = "Chậm")]
    Cham,
}
