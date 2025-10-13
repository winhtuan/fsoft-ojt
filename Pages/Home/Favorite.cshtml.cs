using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Pages.Home
{
    [Authorize]
    public class FavoriteModel : PageModel
    {
        private readonly IUserFavoriteService _favoriteService;

        public List<PlantDto> FavoritePlants { get; set; } = new();
        public PagingViewModel Paging { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "p")]
        public new int Page { get; set; } = 1;

        public FavoriteModel(IUserFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // GET: /Home/Favorite
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var allFavorites = await _favoriteService.GetFavoritePlantDtosAsync(userId);

            var paged = allFavorites.Paginate(
                Page,
                8,
                pageNumber =>
                    Url.Page("/Home/Favorite", null, new { p = pageNumber }) ?? string.Empty
            );

            FavoritePlants = paged.Items;
            Paging = paged.PagingInfo;

            return Page();
        }

        // AJAX Preview Partial
        public async Task<IActionResult> OnGetPreviewAsync()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var allFavorites = await _favoriteService.GetFavoritePlantDtosAsync(userId);

                var paged = allFavorites.Paginate(
                    Page,
                    8,
                    pageNumber =>
                        Url.Page("/Home/Favorite", null, new { p = pageNumber }) ?? string.Empty
                );

                FavoritePlants = paged.Items;
                Paging = paged.PagingInfo;

                // Đảm bảo _CardPatial.cshtml dùng model IEnumerable<PlantDto>
                return Partial("_CardPatial", FavoritePlants);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "OnGetPreviewAsync: Xảy ra lỗi khi tải partial view favorite."
                );
                return StatusCode(500, "Đã xảy ra lỗi máy chủ khi tải dữ liệu.");
            }
        }
    }
}
