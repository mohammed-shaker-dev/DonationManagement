using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Dashboard.BlazorApp.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly ILogger<HttpService> _logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
        {
            _httpClient = httpClient;
            _apiUrl = _httpClient.BaseAddress?.ToString() ?? "";
            _logger = logger;
        }

        public async Task<T> HttpGetAsync<T>(string uri)
            where T : class
        {
            try
            {
                _logger.LogInformation($"Making GET request to: {uri}");

                // First, check if the URI is properly formatted
                var requestUri = uri;
                if (!uri.StartsWith("http") && !uri.StartsWith("/"))
                {
                    requestUri = $"{_apiUrl}{uri}";
                }

                _logger.LogInformation($"Full request URL: {requestUri}");

                // Log headers for debugging
                _logger.LogInformation($"Request headers: {string.Join(", ", _httpClient.DefaultRequestHeaders.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");

                var result = await _httpClient.GetAsync(requestUri);

                // Log response details
                _logger.LogInformation($"Response status: {(int)result.StatusCode} {result.ReasonPhrase}");

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"Request failed with status code {result.StatusCode}. Response content: {errorContent}");

                    if (result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        _logger.LogError("Server returned 500 Internal Server Error. Check the API logs for more details.");
                    }

                    return null;
                }

                var responseContent = await result.Content.ReadAsStringAsync();

                // Log first part of response for debugging (truncated if too long)
                var logContent = responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent;
                _logger.LogInformation($"Response content: {logContent}");

                try
                {
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError($"JSON deserialization error: {jsonEx.Message}. Content: {responseContent.Substring(0, Math.Min(responseContent.Length, 1000))}");
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}, StatusCode: {ex.StatusCode}");
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error in HttpGetAsync: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<string> HttpGetAsync(string uri)
        {
            try
            {
                _logger.LogInformation($"Making GET request for string content to: {uri}");

                var requestUri = uri.StartsWith("http") ? uri : $"{_apiUrl}{uri}";
                var result = await _httpClient.GetAsync(requestUri);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"Request failed with status code {result.StatusCode}. Response content: {errorContent}");
                    return null;
                }

                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in string HttpGetAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> HttpDeleteAsync(string uri)
        {
            try
            {
                _logger.LogInformation($"Making DELETE request to: {uri}");

                var requestUri = uri.StartsWith("http") ? uri : $"{_apiUrl}{uri}";
                var result = await _httpClient.DeleteAsync(requestUri);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"DELETE request failed with status code {result.StatusCode}. Response content: {errorContent}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in HttpDeleteAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<T> HttpPostAsync<T>(string uri, object dataToSend)
            where T : class
        {
            try
            {
                _logger.LogInformation($"Making POST request to: {uri}");

                string json = JsonSerializer.Serialize(dataToSend);
                _logger.LogInformation($"POST data: {json}");

                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var requestUri = uri.StartsWith("http") ? uri : uri;
                var result = await _httpClient.PostAsync(requestUri, httpContent);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"POST request failed with status code {result.StatusCode}. Response content: {errorContent}");
                    return null;
                }

                var responseContent = await result.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError($"JSON deserialization error in POST: {jsonEx.Message}. Content: {responseContent.Substring(0, Math.Min(responseContent.Length, 1000))}");
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed for POST: {ex.Message}, StatusCode: {ex.StatusCode}");
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error in HttpPostAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<T> HttpPutAsync<T>(string uri, object dataToSend)
            where T : class
        {
            try
            {
                _logger.LogInformation($"Making PUT request to: {uri}");

                var content = ToJson(dataToSend);
                var requestUri = uri.StartsWith("http") ? uri : $"{_apiUrl}{uri}";

                var result = await _httpClient.PutAsync(requestUri, content);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogError($"PUT request failed with status code {result.StatusCode}. Response content: {errorContent}");
                    return null;
                }

                return await FromHttpResponseMessageAsync<T>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in HttpPutAsync: {ex.Message}");
                return null;
            }
        }

        private StringContent ToJson(object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            _logger.LogInformation($"Serialized object to JSON: {json}");
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<T> FromHttpResponseMessageAsync<T>(HttpResponseMessage result)
        {
            var content = await result.Content.ReadAsStringAsync();
            _logger.LogInformation($"Deserializing response content: {(content.Length > 200 ? content.Substring(0, 200) + "..." : content)}");

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing response: {ex.Message}");
                _logger.LogError($"Content that failed to deserialize: {content}");
                throw;
            }
        }
    }
}