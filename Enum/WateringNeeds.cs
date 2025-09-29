using System.ComponentModel.DataAnnotations;

namespace Plantpedia.Enum;

public enum WateringNeeds
{
    [Display(Name = "Cao - Phun sương cho lá thường xuyên")]
    Cao,

    [Display(Name = "Trung Bình")]
    TrungBinh,

    [Display(Name = "Thấp")]
    Thap,
}
