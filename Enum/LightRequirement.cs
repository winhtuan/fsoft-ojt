using System.ComponentModel.DataAnnotations;

namespace Plantpedia.Enum;

public enum LightRequirement
{
    [Display(Name = "Nắng toàn phần")]
    NangToanPhan,

    [Display(Name = "Bán râm")]
    BanRam,

    [Display(Name = "Bóng râm")]
    BongRam,
}
