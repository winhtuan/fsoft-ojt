using Plantpedia.DTO;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface ICommentRepository
    {
        Task<List<PlantComment>> GetByPlantAsync(string plantId);
        Task<PlantComment?> GetByIdAsync(int id);
        Task AddAsync(PlantComment comment);
        void Delete(PlantComment comment);
        Task UpdateAsync(PlantCommentUpdateRequest request, int userId);
        Task DeleteAsync(int commentId, int userId);
        Task DeleteReactionAsync(int commentId, int userId);
        Task UpsertReactionAsync(int commentId, int userId, bool reactionType);
        Task<PlantCommentReaction?> GetReactionAsync(int commentId, int userId);
        Task<int> CountReactionsAsync(int commentId);
        Task SaveChangesAsync();
    }
}
