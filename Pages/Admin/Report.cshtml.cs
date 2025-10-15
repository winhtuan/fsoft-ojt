using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plantpedia.Pages;

[Authorize(Policy = "AdminOnly")]
public class ReportModel : PageModel
{
    public void OnGet() { }
}
