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

        public async Task<IActionResult> OnGetAsync()
        {
            // Nếu chưa đăng nhập: không gọi service, hiển thị popup
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                var req = HttpContext.Request;

                // Tạo login URL có ReturnUrl
                ViewData["LoginUrl"] = Url.Page(
                    "/Auth/Login",
                    null,
                    new { ReturnUrl = req.Path + req.QueryString },
                    req.Scheme
                );

                ViewData["RequireLogin"] = true;
                FavoritePlants = new();
                Paging = new PagingViewModel();
                return Page();
            }

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

        public async Task<IActionResult> OnGetPreviewAsync()
        {
            try
            {
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return StatusCode(
                        StatusCodes.Status401Unauthorized,
                        "Bạn cần đăng nhập để xem danh sách yêu thích."
                    );
                }

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

                return Partial("_CardPatial", FavoritePlants);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "OnGetPreviewAsync: Lỗi khi tải partial favorite.");
                return StatusCode(500, "Đã xảy ra lỗi máy chủ khi tải dữ liệu.");
            }
        }
    }
}
