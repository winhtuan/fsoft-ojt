using System.Collections.Generic;
using System.Threading.Tasks;
using Plantpedia.DTO;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public interface IPlantRepository
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
        Task<PlantDto> GetPlantDetailAsync(string plantId);
        Task<PlantInfo?> GetByIdForUpdateAsync(string plantId);
        Task CreateAsync(PlantInfo plant);
        Task UpdateAsync(PlantInfo plant);
        Task<PlantInfo?> GetByIdAsync(string id);
        Task DeleteAsync(PlantInfo plant);
    }
}
