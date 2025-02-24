using Dashboard.Core.ProjectAggregate;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Dashboard.BlazorApp.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
        }

        public async Task<Project> GetProjectByIdAsync(long id)
        {
            return await _httpClient.GetFromJsonAsync<Project>($"api/projects/{id}");
        }

        public async Task CreateProjectAsync(Project project)
        {
            await _httpClient.PostAsJsonAsync("api/projects", project);
        }

        public async Task UpdateProjectAsync(Project project)
        {
            await _httpClient.PutAsJsonAsync($"api/projects/{project.Id}", project);
        }

        public async Task DeleteProjectAsync(long id)
        {
            await _httpClient.DeleteAsync($"api/projects/{id}");
        }
    }
}
