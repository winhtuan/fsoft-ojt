using Plantpedia.Models;

namespace Plantpedia.Repository;

public interface ILookupRepository
{
    Task<IEnumerable<PlantType>> GetAllPlantTypesAsync();
    Task<IEnumerable<Region>> GetAllRegionsAsync();
    Task<IEnumerable<SoilType>> GetAllSoilTypesAsync();
    Task<IEnumerable<Climate>> GetAllClimatesAsync();
    Task<IEnumerable<Usage>> GetAllUsagesAsync();
}
