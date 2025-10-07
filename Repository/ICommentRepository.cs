using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface ICommentRepository
    {
        Task<List<PlantComment>> GetByPlantAsync(string plantId);
        Task<PlantComment?> GetByIdAsync(int id);
        Task AddAsync(PlantComment comment);
        Task UpsertReactionAsync(int commentId, int userId, char reactionType);
        Task SaveChangesAsync();
    }
}
