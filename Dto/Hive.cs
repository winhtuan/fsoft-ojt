using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Plantpedia.DTO.Hive.V3
{
    public class HiveV3RequestInput
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class HiveV3RequestBody
    {
        [JsonPropertyName("input")]
        public List<HiveV3RequestInput> Input { get; set; } = new();
    }

    // --- CÁC LỚP CHO RESPONSE BODY ---
    public class HiveV3ApiResponse
    {
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; } = string.Empty;

        [JsonPropertyName("output")]
        public List<HiveV3OutputItem> Output { get; set; } = new();
    }

    public class HiveV3OutputItem
    {
        [JsonPropertyName("classes")]
        public List<HiveV3ClassScore> Classes { get; set; } = new();
    }

    public class HiveV3ClassScore
    {
        [JsonPropertyName("class")]
        public string ClassName { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
