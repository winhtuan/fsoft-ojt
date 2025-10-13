using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

        public PlantDiagnosisService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory
        )
        {
            _apiKey = configuration["PlantIdApi:ApiKey"]!;
            _apiUrl = configuration["PlantIdApi:ApiUrl"]!;
            _httpClient = httpClientFactory.CreateClient();
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

                var jsonDoc = JsonDocument.Parse(responseContent);
                return jsonDoc.RootElement.Clone();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex);
                return null;
            }
        }
    }
}
