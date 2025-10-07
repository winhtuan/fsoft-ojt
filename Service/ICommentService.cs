using Plantpedia.DTO;

namespace Plantpedia.Service
{
    public interface ICommentService
    {
        Task<List<PlantCommentDto>> GetCommentsByPlantAsync(string plantId);
        Task<PlantCommentDto> CreateAsync(PlantCommentCreateRequest request);
        Task ToggleReactionAsync(PlantCommentReactionRequest request);
    }
}
