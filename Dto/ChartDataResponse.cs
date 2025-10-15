namespace Plantpedia.DTO;

public sealed class ChartDataResponse
{
    public string[] Labels { get; set; } = Array.Empty<string>();
    public int[] Data { get; set; } = Array.Empty<int>();
    public int Total { get; set; }
    public string? TopType { get; set; }
    public double? Growth { get; set; }
    public int? FavoritesNew { get; set; }
}
