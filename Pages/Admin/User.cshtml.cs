using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Service;

namespace Plantpedia.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class UserModel : PageModel
    {
        private readonly IUserService _userService;

        public UserModel(IUserService userService)
        {
            _userService = userService;
        }
    }
}
