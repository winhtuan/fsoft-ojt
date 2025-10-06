using System.ComponentModel.DataAnnotations;

namespace Plantpedia.ViewModel;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
