using Dashboard.Core.ProjectAggregate;
using SharedKernel.Enums;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharedKernel.Blazor.Shared;

namespace Dashboard.BlazorApp.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpService _httpService;

        public ProjectService(HttpClient httpClient, HttpService httpService)
        {
            _httpClient = httpClient;
            _httpService = httpService;
        }

        public async Task<List<ProjectDTO>> GetProjectsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProjectDTO>>("projects");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return new List<ProjectDTO>();
            }
        }

        public async Task<List<ProjectDTO>> GetProjectsByTypeAsync(ProjectType projectType)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProjectDTO>>($"projects/type/{GetProjectTypeId(projectType)}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return new List<ProjectDTO>();
            }
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(long id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProjectDTO>($"projects/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateProjectAsync(CreateProjectRequest project)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("projects", project);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProjectAsync(long id, UpdateProjectRequest project)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"projects/{id}", project);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(long id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"projects/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddExpenseAsync(long projectId, CreateExpenseRequest expense)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"projects/{projectId}/expenses", expense);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteExpenseAsync(long projectId, long expenseId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"projects/{projectId}/expenses/{expenseId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                return false;
            }
        }
        private int GetProjectTypeId(ProjectType projectType)
        {
            return projectType switch
            {
                ProjectType.Organization => 1,
                ProjectType.Donation => 2,
            };
        }
    }
}