using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Plantpedia.DTO;
using Plantpedia.DTO.Hive.V3;
using Plantpedia.Helper;

namespace Plantpedia.Service
{
    public class HiveModerationService : IHiveModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly Dictionary<string, string> _classNameMapping;

        public HiveModerationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration
        )
        {
            _httpClient = httpClientFactory.CreateClient();

            string apiUrl = configuration.GetValue<string>("HiveApi:ApiUrl")!;
            _secretKey = configuration.GetValue<string>("HiveApi:ApiSecretKey")!;

            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _secretKey
            );

            // Load mapping từ file JSON
            _classNameMapping = LoadClassNameMapping();
        }

        /// <summary>
        /// Load mapping từ Config/hive_class_to_vi.json
        /// </summary>
        private Dictionary<string, string> LoadClassNameMapping()
        {
            try
            {
                var configPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Config",
                    "hive_class_to_vi.json"
                );

                if (!File.Exists(configPath))
                {
                    LoggerHelper.Warn(
                        $"Mapping file not found at: {configPath}. Using default English class names."
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

                LoggerHelper.Info($"Loaded {mapping.Count} class name mappings from config.");
                return mapping;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"Failed to load class name mapping: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Chuyển đổi class name từ tiếng Anh sang tiếng Việt
        /// </summary>
        private string GetVietnameseClassName(string englishClassName)
        {
            if (_classNameMapping.TryGetValue(englishClassName, out var vietnameseName))
            {
                return vietnameseName;
            }

            return englishClassName;
        }

        public async Task<HarmAnalysisResult> AnalyzeTextAsync(string text)
        {
            var requestBody = new HiveV3RequestBody
            {
                Input = new List<HiveV3RequestInput> { new HiveV3RequestInput { Text = text } },
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync("hive/text-moderation", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    LoggerHelper.Error(
                        $"Hive API call failed. Status Code: {response.StatusCode}. Response: {errorContent}"
                    );
                    throw new HttpRequestException(
                        $"Hive API returned an error: {response.StatusCode}. Details: {errorContent}"
                    );
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                LoggerHelper.Info($"Hive API Response: {jsonResponse}");

                var hiveResponse = JsonSerializer.Deserialize<HiveV3ApiResponse>(jsonResponse);
                return ParseV3Response(hiveResponse);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex);
                throw;
            }
        }

        private HarmAnalysisResult ParseV3Response(HiveV3ApiResponse? response)
        {
            if (response?.Output == null || !response.Output.Any())
            {
                LoggerHelper.Warn("Hive response output is empty or null.");
                return new HarmAnalysisResult { IsHarmful = false };
            }

            var detectedViolations = new List<string>();

            // Danh sách các loại vi phạm mà chúng ta quan tâm
            var harmfulClasses = new HashSet<string>
            {
                "sexual",
                "violence",
                "hate",
                "profanity",
                "spam",
                "illegal_drugs",
                "drugs",
                "bullying",
                "self_harm",
                "self_harm_intent",
                "child_exploitation",
                "child_safety",
                "weapons",
                "violent_description",
                "sexual_description",
                "minor_implicitly_mentioned",
                "minor_explicitly_mentioned",
            };

            var classScores = response.Output.FirstOrDefault()?.Classes;

            if (classScores != null)
            {
                foreach (var score in classScores)
                {
                    if (harmfulClasses.Contains(score.ClassName) && score.Value > 0)
                    {
                        var vietnameseName = GetVietnameseClassName(score.ClassName);
                        detectedViolations.Add(vietnameseName);

                        LoggerHelper.Warn(
                            $"Detected violation: {score.ClassName} ({vietnameseName}) - Score: {score.Value}"
                        );
                    }
                }
            }

            if (detectedViolations.Any())
            {
                var violationList = string.Join(", ", detectedViolations);
                LoggerHelper.Warn($"Harmful content detected: {violationList}");

                return new HarmAnalysisResult
                {
                    IsHarmful = true,
                    Reason = $"Nội dung vi phạm: {violationList}",
                };
            }

            LoggerHelper.Info("No harmful content detected.");
            return new HarmAnalysisResult { IsHarmful = false };
        }
    }
}
