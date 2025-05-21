using System.Text.Json;

namespace FileAnalysisService.Clients;

public class WordCloudApiClient : IWordCloudApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WordCloudApiClient> _logger;

        public WordCloudApiClient(HttpClient httpClient, ILogger<WordCloudApiClient> logger)
        {
            _httpClient = httpClient; 
            _logger = logger;

            if (_httpClient.BaseAddress == null)
            {
                _logger.LogError("HttpClient BaseAddress is not configured for WordCloudApiClient.");
                throw new InvalidOperationException("HttpClient BaseAddress is not configured for WordCloudApiClient.");
            }
        }

        public async Task<Stream?> GenerateWordCloudAsync(string text, WordCloudParameters? parameters = null)
        {
            _logger.LogInformation("Requesting word cloud generation from QuickChart.io API for text starting with: '{TextStart}...'", text.Substring(0, Math.Min(text.Length, 50)));

            parameters ??= new WordCloudParameters(); 
            var requestBody = new
            {
                text = text,
                format = parameters.Format,
                width = parameters.Width,
                height = parameters.Height,
                backgroundColor = parameters.BackgroundColor,
                fontColor = parameters.FontColor,
                fontScale = parameters.FontScale,
                removeStopwords = parameters.RemoveStopwords,
                language = parameters.Language,
                useWordList = parameters.UseWordList
            };

            try
            {
                _logger.LogDebug("Sending POST request to WordCloud API. BaseAddress: {BaseAddress}, RequestBody preview (format): {Format}",
                    _httpClient.BaseAddress, requestBody.format);

                var options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("", requestBody, options); 

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully generated word cloud image from API. Content-Type: {ContentType}", response.Content.Headers.ContentType);
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to generate word cloud image. Status: {StatusCode}, Error: {ErrorContent}, URL: {RequestUrl}",
                        response.StatusCode, errorContent, response.RequestMessage?.RequestUri);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while requesting word cloud API.");
                return null;
            }
        }
    }