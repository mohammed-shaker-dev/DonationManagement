using Dashboard.Api.Helpers;
using Dashboard.Core.DTOs;
using Dashboard.Core.ProjectAggregate;
using Dashboard.Core.ProjectAggregate.Specifications;
using Dashboard.Core.ValueObjects;
using Dashboard.Core.WalletAggregate;
using Dashboard.Core.WalletAggregate.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Blazor.Shared;
using SharedKernel.Enums;
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
        private readonly IRepository<Wallet> _walletRepository;
        public ProjectsController(
            IRepository<Project> projectRepository,
            IRepository<Wallet> walletRepository)
        {
            _projectRepository = projectRepository;
            _walletRepository = walletRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDTO>>> GetAll()
        {
            try
            {
                var spec = new ProjectsWithExpensesSpec();
                var projects = await _projectRepository.ListAsync(spec);

                if (projects == null)
                {
                    return NotFound("No projects found");
                }

                return Ok(projects.Select(p => p.ToDto()).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        [HttpGet("type/{projectType}")]
        public async Task<ActionResult<List<ProjectDTO>>> GetByType(ProjectType projectType)
        {
            try
            {
                var spec = new ProjectsByTypeSpec(projectType);
                var projects = await _projectRepository.ListAsync(spec);

                if (projects == null)
                {
                    return NotFound($"No projects found with type: {projectType}");
                }

                return Ok(projects.Select(p => p.ToDto()).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving projects of type {projectType}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetById(long id)
        {
            var spec = new ProjectByIdWithExpensesSpec(id);
            var project = await _projectRepository.GetBySpecAsync(spec);

            if (project == null)
            {
                return NotFound();
            }
            return Ok(project.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProjectRequest request)
        {
            // Create the project
            var project = new Project(
                request.Name,
                request.Description,
                request.AdditionalText,
                request.StartedDate,
                request.CompletedDate,
                request.ProjectType); // Include ProjectType in constructor

            // Add images and videos if present
            if (request.Images != null)
            {
                foreach (var image in request.Images)
                {
                    project.AddImage(image);
                }
            }

            if (request.Videos != null)
            {
                foreach (var video in request.Videos)
                {
                    project.AddVideo(video);
                }
            }

            // Add expenses and create corresponding transactions
            if (request.Expenses != null && request.Expenses.Any())
            {
                // Get the default wallet (usually SYP wallet)
                var wallet = await _walletRepository.GetBySpecAsync(
                    new WalletByNameWithTransactionssSpec(request.WalletName ?? "SYP"));

                if (wallet == null)
                {
                    return BadRequest("Default wallet not found");
                }

                foreach (var expenseRequest in request.Expenses)
                {
                    // Add expense to project
                    var expense = new Expense(
                        expenseRequest.Name,
                        expenseRequest.Date,
                        new Money(expenseRequest.Value, new Currency(wallet.Currency.Code)),
                        expenseRequest.Code);

                    project.AddExpense(expense);

                    // Create a transaction for this expense (OUT type)
                    var transaction = new Transaction(
                        expenseRequest.Code,
                        $"Project expense: {request.Name} - {expenseRequest.Name}",
                        "System", // FullName
                        "system@donation.app", // Email
                        expenseRequest.Value,
                        wallet.Id,
                        1, // Default userId (admin)
                        TransactionType.OUT);

                    wallet.AddNewTransaction(transaction);
                }

                try
                {
                    // Save wallet with transactions
                    await _walletRepository.UpdateAsync(wallet);
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            // Save the project
            await _projectRepository.AddAsync(project);

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateProjectRequest request)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Update basic properties

            // Update status if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                project.UpdateStatus(request.Status);
            }

            // Update project type if different from current value
            if (request.ProjectType != project.ProjectType)
            {
                project.UpdateProjectType(request.ProjectType??ProjectType.Donation);
            }

            // Handle images update
            if (request.Images != null)
            {
                // Add new images
                foreach (var image in request.Images)
                {
                    if (!project.Images.Contains(image))
                    {
                        project.AddImage(image);
                    }
                }

                // Remove images that are not in the updated list
                foreach (var existingImage in project.Images.ToList())
                {
                    if (!request.Images.Contains(existingImage))
                    {
                        project.RemoveImage(existingImage);
                    }
                }
            }

            // Handle videos update
            if (request.Videos != null)
            {
                // Add new videos
                foreach (var video in request.Videos)
                {
                    if (!project.Videos.Contains(video))
                    {
                        project.AddVideo(video);
                    }
                }

                // Remove videos that are not in the updated list
                foreach (var existingVideo in project.Videos.ToList())
                {
                    if (!request.Videos.Contains(existingVideo))
                    {
                        project.RemoveVideo(existingVideo);
                    }
                }
            }

            await _projectRepository.UpdateAsync(project);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                // Get the project with expenses
                var project = await _projectRepository.GetBySpecAsync(new ProjectWithExpensesSpec(id));

                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found");
                }

                // Get all expense codes to find related transactions
                var expenseCodes = project.Expenses.Select(e => e.Code).ToList();

                if (expenseCodes.Any())
                {
                    // Get all wallets with their transactions
                    var wallets = await _walletRepository.ListAsync(new WalletsWithTransactionsSpec());

                    foreach (var wallet in wallets)
                    {
                        // Find transactions matching expense codes
                        var transactionsToDelete = wallet.Transactions
                            .Where(t => expenseCodes.Contains(t.Code))
                            .ToList();

                        // Delete each transaction
                        foreach (var transaction in transactionsToDelete)
                        {
                            wallet.DeleteTransaction(transaction.Id);
                        }

                        // Update wallet
                        if (transactionsToDelete.Any())
                        {
                            await _walletRepository.UpdateAsync(wallet);
                        }
                    }
                }

                // Now delete the project
                await _projectRepository.DeleteAsync(project);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the project");
            }
        }

        [HttpPost("{projectId}/expenses")]
        public async Task<ActionResult> AddExpense(long projectId, [FromBody] CreateExpenseRequest request)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            // Create expense
            var expense = new Expense(
                request.Name,
                request.Date,
               new Money(request.Value, new Currency(request.WalletName)),
                request.Code);

            project.AddExpense(expense);

            // Create transaction for this expense
            if (request.CreateTransaction)
            {
                var wallet = await _walletRepository.GetBySpecAsync(
                    new WalletByNameWithTransactionssSpec(request.WalletName ?? "SYP"));

                if (wallet != null)
                {
                    var transaction = new Transaction(
                        request.Code,
                        $"Project expense: {project.Name} - {request.Name}",
                        "System",
                        "system@donation.app",
                        request.Value,
                        wallet.Id,
                        1, // Default userId
                        TransactionType.OUT);

                    wallet.AddNewTransaction(transaction);
                    await _walletRepository.UpdateAsync(wallet);
                }
            }

            await _projectRepository.UpdateAsync(project);
            return Ok();
        }
    }
}