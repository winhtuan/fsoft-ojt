using Plantpedia.Models;

namespace Plantpedia.Repository;

public interface IUserFavoriteRepository
{
    Task<UserFavorite?> GetByUserAndPlantAsync(int userId, string plantId);
    Task AddAsync(UserFavorite favorite);
    void Remove(UserFavorite favorite);
    Task<List<UserFavorite>> GetAllByUserAsync(int userId);
    Task SaveChangesAsync();
}
