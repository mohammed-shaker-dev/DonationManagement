using System.Text.Json;
using System.Text;
using System.Net;

namespace Donation.Web.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiUrl = _httpClient.BaseAddress.ToString();
        }
        public async Task<T> HttpGetAsync<T>(string uri)
            where T : class
        {
            try
            {
                var result = await _httpClient.GetAsync(uri);
                if (result.StatusCode == (HttpStatusCode)429) // Too Many Requests
                {
                    // Handle rate limit exceeded
                    throw new HttpRequestException("Rate limit exceeded. Please try again later.");
                }
                if (!result.IsSuccessStatusCode)
                {
                    return null;
                }

                return await FromHttpResponseMessageAsync<T>(result);
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> HttpGetAsync(string uri)
        {
            var result = await _httpClient.GetAsync($"{_apiUrl}{uri}");
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await result.Content.ReadAsStringAsync();
        }

        public async Task<T> HttpDeleteAsync<T>(string uri, object id)
            where T : class
        {
            var result = await _httpClient.DeleteAsync($"{_apiUrl}{uri}/{id}");
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
        }

        public async Task<T> HttpPostAsync<T>(string uri, object dataToSend)
            where T : class
        {
            try
            {
                string json = JsonSerializer.Serialize(dataToSend);  
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync( uri, httpContent);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T> HttpPutAsync<T>(string uri, object dataToSend)
            where T : class
        {
            var content = ToJson(dataToSend);

            var result = await _httpClient.PutAsync($"{_apiUrl}{uri}", content);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
        }


        private StringContent ToJson(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        private async Task<T> FromHttpResponseMessageAsync<T>(HttpResponseMessage result)
        {
            return JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
