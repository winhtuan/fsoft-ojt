using System.Threading.Tasks;
using Plantpedia.DTO;

namespace Plantpedia.Service
{
    public interface IHiveModerationService
    {
        Task<HarmAnalysisResult> AnalyzeTextAsync(string text);
    }
}
