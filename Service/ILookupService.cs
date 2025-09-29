using Plantpedia.DTO;
using Plantpedia.ViewModel;

namespace Plantpedia.Service
{
    public interface ILookupService
    {
        Task<IEnumerable<LookupItemDto>> GetPlantTypesAsync();
        Task<IEnumerable<LookupItemDto>> GetRegionsAsync();
        Task<IEnumerable<LookupItemDto>> GetSoilTypesAsync();
        Task<IEnumerable<LookupItemDto>> GetClimatesAsync();
        Task<IEnumerable<LookupItemDto>> GetUsagesAsync();
        Task<List<DropdownViewModel>> GetAllFilterDropdownsAsync();
    }
}
