using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository;

public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly AppDbContext _db;

    public UserFavoriteRepository(AppDbContext db) => _db = db;

    public async Task<UserFavorite?> GetByUserAndPlantAsync(int userId, string plantId)
    {
        LoggerHelper.Info($"Get favorite userId={userId}, plantId={plantId}");
        return await _db.UserFavorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.PlantId == plantId
        );
    }

    public async Task AddAsync(UserFavorite favorite)
    {
        LoggerHelper.Info($"Add favorite userId={favorite.UserId}, plantId={favorite.PlantId}");
        await _db.UserFavorites.AddAsync(favorite);
    }

    public void Remove(UserFavorite favorite)
    {
        LoggerHelper.Info($"Remove favorite userId={favorite.UserId}, plantId={favorite.PlantId}");
        _db.UserFavorites.Remove(favorite);
    }

    public async Task<List<UserFavorite>> GetAllByUserAsync(int userId)
    {
        LoggerHelper.Info($"Get all favorites by userId={userId}");
        return await _db
            .UserFavorites.Include(f => f.Plant)
            .ThenInclude(p => p.PlantImages)
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        LoggerHelper.Info("Save changes (UserFavoriteRepository)");
        await _db.SaveChangesAsync();
    }
}
