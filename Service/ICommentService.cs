using Plantpedia.DTO;

namespace Plantpedia.Service
{
    public interface ICommentService
    {
        Task<List<PlantCommentDto>> GetCommentsByPlantAsync(string plantId);
        Task<PlantCommentDto> CreateAsync(PlantCommentCreateRequest request, int userId);
        Task<int> ToggleReactionAsync(PlantCommentReactionRequest request, int userId);
        Task UpdateAsync(PlantCommentUpdateRequest request, int userId);
        Task DeleteAsync(int commentId, int userId);
    }
}
