using Plantpedia.DTO;
using Plantpedia.Models;

namespace Plantpedia.Service;

public interface IUserFavoriteService
{
    Task<bool> ToggleFavoriteAsync(int userId, string plantId);
    Task<bool> IsFavoriteAsync(int userId, string plantId);
    Task<List<PlantDto>> GetFavoritePlantDtosAsync(int userId);
}
