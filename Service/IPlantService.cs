using Plantpedia.DTO;
using Plantpedia.Models;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public interface IPlantService
    {
        Task<IEnumerable<PlantDto>> GetAllAsync();
        Task<List<PlantDto>> GetFilteredDtosAsync(
            string? plantTypeId,
            string? regionId,
            string? climateId,
            string? usageId,
            string? soilTypeId,
            string? keywords
        );

        Task<PlantDto> GetPlantById(string plantId);
        Task CreatePlantAsync(UpdatePlantViewModel viewModel);
        Task UpdatePlantAsync(UpdatePlantViewModel viewModel);
        Task<bool> DeleteByIdAsync(string id);
        Task<int> GetPlantCountAsync();
    }
}
