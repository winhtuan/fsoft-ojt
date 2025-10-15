using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Plantpedia.Helper;

namespace Plantpedia.Service
{
    public class PlantDiagnosisService : IPlantDiagnosisService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _diseaseMap;

        public PlantDiagnosisService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            _apiKey = configuration["PlantIdApi:ApiKey"]!;
            _apiUrl = configuration["PlantIdApi:ApiUrl"]!;
            _httpClient = httpClientFactory.CreateClient();

            // Load mapping từ file JSON
            _diseaseMap = LoadDiseaseMapping();
        }

        /// <summary>
        /// Load mapping từ Config/disease_to_vi.json
        /// </summary>
        private Dictionary<string, string> LoadDiseaseMapping()
        {
            try
            {
                var configPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Config",
                    "plant_diseases_vi.json"
                );

                if (!File.Exists(configPath))
                {
                    LoggerHelper.Warn(
                        $"Mapping file not found at: {configPath}. Using default English disease names."
                    );
                    return new Dictionary<string, string>();
                }

                var jsonContent = File.ReadAllText(configPath);
                var mapping = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                if (mapping == null || !mapping.Any())
                {
                    LoggerHelper.Warn("Mapping file is empty or invalid.");
                    return new Dictionary<string, string>();
                }

                LoggerHelper.Info($"Loaded {mapping.Count} disease name mappings from config.");
                return new Dictionary<string, string>(mapping, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"Failed to load disease name mapping: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Chuyển đổi disease name từ tiếng Anh sang tiếng Việt
        /// </summary>
        private string GetVietnameseDiseaseName(string englishDiseaseName)
        {
            var lowerName = englishDiseaseName.ToLowerInvariant();
            if (_diseaseMap.TryGetValue(lowerName, out var vietnameseName))
            {
                return vietnameseName;
            }

            return englishDiseaseName;
        }

        public async Task<JsonElement?> DiagnoseAsync(IFormFile image)
        {
            try
            {
                LoggerHelper.Info($"Bắt đầu chẩn đoán với API: {_apiUrl}");
                LoggerHelper.Info(
                    $"Ảnh: {image.FileName}, Kích thước: {image.Length} bytes, Loại: {image.ContentType}"
                );

                // Chuyển ảnh sang base64
                string base64Image;
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();
                    base64Image = Convert.ToBase64String(imageBytes);
                }

                var requestBody = new
                {
                    images = new[] { $"data:{image.ContentType};base64,{base64Image}" },
                    similar_images = true,
                    health = "all",
                    classification_level = "all",
                    classification_raw = true,
                    symptoms = true,
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                LoggerHelper.Info($"Độ dài nội dung yêu cầu: {jsonContent.Length} ký tự");

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);

                var response = await _httpClient.PostAsync(_apiUrl, httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    LoggerHelper.Error($"Lỗi API {(int)response.StatusCode}: {responseContent}");
                    return null;
                }

                LoggerHelper.Info("Gọi API thành công");
                var previewLen = Math.Min(500, responseContent.Length);
                LoggerHelper.Info(
                    $"Xem nhanh phản hồi: {responseContent.Substring(0, previewLen)}..."
                );

                var rootNode = JsonNode.Parse(responseContent);
                if (rootNode == null)
                {
                    LoggerHelper.Error("Failed to parse API response to JsonNode.");
                    return null;
                }

                // Map tên bệnh sang tiếng Việt nếu có
                if (
                    rootNode is JsonObject rootObj
                    && rootObj["result"] is JsonObject result
                    && result["disease"] is JsonObject disease
                    && disease["suggestions"] is JsonArray suggestions
                )
                {
                    foreach (var suggestion in suggestions.OfType<JsonObject>())
                    {
                        if (
                            suggestion["name"] is JsonValue nameValue
                            && nameValue.TryGetValue<string>(out var name)
                        )
                        {
                            var vietName = GetVietnameseDiseaseName(name);
                            suggestion["name"] = JsonValue.Create(vietName);
                        }

                        // Tùy chọn: Dịch classification nếu có
                        if (suggestion["classification"] is JsonArray classification)
                        {
                            for (int i = 0; i < classification.Count; i++)
                            {
                                if (
                                    classification[i] is JsonValue classValue
                                    && classValue.TryGetValue<string>(out var className)
                                )
                                {
                                    var vietClass = GetVietnameseDiseaseName(className);
                                    classification[i] = JsonValue.Create(vietClass);
                                }
                            }
                        }
                    }
                }

                var modifiedJson = rootNode.ToJsonString();
                var modifiedDoc = JsonDocument.Parse(modifiedJson);
                return modifiedDoc.RootElement.Clone();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex);
                return null;
            }
        }
    }
}
