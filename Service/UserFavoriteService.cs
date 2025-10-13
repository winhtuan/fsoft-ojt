using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;

namespace Plantpedia.Service;

public class UserFavoriteService : IUserFavoriteService
{
    private readonly IUserFavoriteRepository _repo;

    public UserFavoriteService(IUserFavoriteRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> ToggleFavoriteAsync(int userId, string plantId)
    {
        LoggerHelper.Info($"ToggleFavorite userId={userId}, plantId={plantId}");
        var favorite = await _repo.GetByUserAndPlantAsync(userId, plantId);
        if (favorite != null)
        {
            _repo.Remove(favorite);
            await _repo.SaveChangesAsync();
            LoggerHelper.Info($"Removed from favorite userId={userId}, plantId={plantId}");
            return false;
        }
        else
        {
            await _repo.AddAsync(
                new UserFavorite
                {
                    UserId = userId,
                    PlantId = plantId,
                    CreatedAt = DateTime.UtcNow,
                }
            );
            await _repo.SaveChangesAsync();
            LoggerHelper.Info($"Added to favorite userId={userId}, plantId={plantId}");
            return true;
        }
    }

    public async Task<bool> IsFavoriteAsync(int userId, string plantId)
    {
        LoggerHelper.Info($"IsFavorite userId={userId}, plantId={plantId}");
        var favorite = await _repo.GetByUserAndPlantAsync(userId, plantId);
        LoggerHelper.Info(
            $"IsFavorite result={favorite != null} userId={userId}, plantId={plantId}"
        );
        return favorite != null;
    }

    public async Task<List<PlantDto>> GetFavoritePlantDtosAsync(int userId)
    {
        var favorites = await _repo.GetAllByUserAsync(userId);
        var plantDtos = favorites
            .Where(f => f.Plant != null)
            .Select(f => new PlantDto
            {
                PlantId = f.Plant.PlantId,
                CommonName = f.Plant.CommonName,
                ScientificName = f.Plant.ScientificName,
                Description = f.Plant.Description,
                ImageUrl = f.Plant.PlantImages?.FirstOrDefault()?.ImageUrl ?? "",
                ImageUrls =
                    f.Plant.PlantImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
            })
            .ToList();

        return plantDtos;
    }
}
