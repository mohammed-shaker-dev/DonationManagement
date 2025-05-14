
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dashboard.BlazorApp.Services
{
    public class FileUploadService
    {
        private readonly HttpClient _httpClient;

        public FileUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadImageAsync(IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Read the file content
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync("FileUpload/images", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<UploadResult>();
                return result.Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }
        }

        public async Task<DocumentUploadResult> UploadDocumentAsync(IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Read the file content
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024)); // 50MB max
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync("FileUpload/documents", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<DocumentUploadResult>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading document: {ex.Message}");
                throw;
            }
        }
    }

    public class UploadResult
    {
        public string Url { get; set; }
    }

    public class DocumentUploadResult
    {
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}
