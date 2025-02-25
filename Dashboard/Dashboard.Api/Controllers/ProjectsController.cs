using Dashboard.Api.Helpers;
using Dashboard.Core.DTOs;
using Dashboard.Core.ProjectAggregate;
using Dashboard.Core.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Blazor.Shared;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IWebHostEnvironment _environment;

        public ProjectsController(IRepository<Project> projectRepository, IWebHostEnvironment environment)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDTO>>> GetAll()
        {
            var projects = await _projectRepository.ListAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetById(long id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project.ToDTO());
        }

        [HttpPost]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] ProjectRequestModel projectRequest)
        {
            var project = new Project(projectRequest.Name, projectRequest.Description, projectRequest.AdditionalText);

            foreach (var image in projectRequest.Images)
            {
                var imagePath = await FileHelper.StoreImageAsync(image);
                project.AddImage(new Image(imagePath, image.FileName));
            }

            foreach (var video in projectRequest.Videos)
            {
                project.AddVideo(new Video(video, video));
            }

            await _projectRepository.AddAsync(project);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project.ToDTO());
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromForm] ProjectRequestModel projectRequest)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.Update(projectRequest.Name, projectRequest.Description, projectRequest.AdditionalText);

            // Update images and videos
            project.ClearImages();
            foreach (var image in projectRequest.Images)
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "images", image.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                project.AddImage(new Image($"/images/{image.FileName}", image.FileName));
            }

            project.ClearVideos();
            foreach (var video in projectRequest.Videos)
            {
                project.AddVideo(new Video(video, video));
            }

            await _projectRepository.UpdateAsync(project);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            //  await _projectRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
