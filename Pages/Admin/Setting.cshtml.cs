using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Service;

namespace Plantpedia.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class SettingModel : PageModel
    {
        public SettingModel(IUserService userService) { }
    }
}
