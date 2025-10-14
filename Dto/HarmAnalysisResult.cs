namespace Plantpedia.DTO
{
    public class HarmAnalysisResult
    {
        public bool IsHarmful { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
