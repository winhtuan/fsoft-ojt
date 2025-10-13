using System.Text.Json;

namespace Plantpedia.Service
{
    public interface IPlantDiagnosisService
    {
        Task<JsonElement?> DiagnoseAsync(IFormFile image);
    }
}
