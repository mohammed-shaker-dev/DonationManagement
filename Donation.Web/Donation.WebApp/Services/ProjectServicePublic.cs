using Dashboard.Core.ProjectAggregate;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using SharedKernel.Enums;

namespace Donation.Web.Services
{
    public class ProjectServicePublic
    {
        private readonly HttpService _httpService;
        private readonly ILogger<ProjectServicePublic> _logger;
        private readonly IMemoryCache _cache;

        private const string PROJECTS_CACHE_KEY = "all_projects";
        private const int CACHE_DURATION_MINUTES = 15;

        public ProjectServicePublic(HttpService httpService, ILogger<ProjectServicePublic> logger)
        {
            _httpService = httpService;
            _logger = logger;
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<List<ProjectDTO>> GetProjectsAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(PROJECTS_CACHE_KEY, out List<ProjectDTO> cachedProjects))
            {
                _logger.LogInformation("Retrieved projects from cache");
                return cachedProjects;
            }

            try
            {
                // Get from API
                var projects = await _httpService.HttpGetAsync<List<ProjectDTO>>("projects");

                if (projects != null)
                {
                    // Store in cache
                    _cache.Set(PROJECTS_CACHE_KEY, projects, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                    return projects;
                }

                return new List<ProjectDTO>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}");
                return new List<ProjectDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting projects: {ex.Message}");
                return new List<ProjectDTO>();
            }
        }

        public async Task<List<ProjectDTO>> GetProjectsByTypeAsync(ProjectType projectType)
        {
            try
            {
                // First try to get all projects from cache
                var allProjects = await GetProjectsAsync();

                // Filter by type
                return allProjects.Where(p => p.ProjectType == projectType).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting projects by type: {ex.Message}");
                return new List<ProjectDTO>();
            }
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(long id)
        {
            // Check if we have the projects in cache
            if (_cache.TryGetValue(PROJECTS_CACHE_KEY, out List<ProjectDTO> cachedProjects))
            {
                var cachedProject = cachedProjects.FirstOrDefault(p => p.Id == id);
                if (cachedProject != null)
                {
                    _logger.LogInformation($"Retrieved project {id} from cache");
                    return cachedProject;
                }
            }

            try
            {
                // Get from API
                var project = await _httpService.HttpGetAsync<ProjectDTO>($"projects/{id}");
                return project;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting project: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ProjectDTO>> GetCompletedProjectsAsync()
        {
            var allProjects = await GetProjectsAsync();
            return allProjects.Where(p => p.Status == ProjectStatus.Completed).ToList();
        }

        public async Task<List<ProjectDTO>> GetInProgressProjectsAsync()
        {
            var allProjects = await GetProjectsAsync();
            return allProjects.Where(p => p.Status == ProjectStatus.InProgress).ToList();
        }

        public async Task<List<ProjectDTO>> GetPlannedProjectsAsync()
        {
            var allProjects = await GetProjectsAsync();
            return allProjects.Where(p => p.Status == ProjectStatus.Planned).ToList();
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