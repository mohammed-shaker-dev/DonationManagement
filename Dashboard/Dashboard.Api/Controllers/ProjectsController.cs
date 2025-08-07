using Dashboard.Api.Helpers;
using Dashboard.Core.DTOs;
using Dashboard.Core.ProjectAggregate;
using Dashboard.Core.ProjectAggregate.Specifications;
using Dashboard.Core.ValueObjects;
using Dashboard.Core.WalletAggregate;
using Dashboard.Core.WalletAggregate.Specifications;
using Dashboard.Infrastructure.Migrations;
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

        [HttpGet("type/{projectTypeNumber}")]
        public async Task<ActionResult<List<ProjectDTO>>> GetByType(int projectTypeNumber)
        {
            try
            {
                var projectType = (ProjectType)projectTypeNumber;
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
                return StatusCode(500, $"An error occurred while retrieving projects of type {projectTypeNumber}");
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
                foreach (var expenseRequest in request.Expenses)
                {
                    // Fetch wallet for this specific expense's currency
                    var expenseWallet = await _walletRepository.GetBySpecAsync(
                        new WalletByNameWithTransactionssSpec(expenseRequest.CurrencyCode));

                    if (expenseWallet == null)
                    {
                        return BadRequest($"Wallet for currency {expenseRequest.CurrencyCode} not found for expense '{expenseRequest.Name}'.");
                    }

                    // Add expense to project
                    var money = new Money(expenseRequest.Value, new Currency(expenseRequest.CurrencyCode));
                    var expense = new Expense(
                        expenseRequest.Name,
                        expenseRequest.Date,
                        money,
                        expenseRequest.Code);

                    project.AddExpense(expense);

                    // Create a transaction for this expense (OUT type)
                    var transaction = new Transaction(
                        expenseRequest.Code,
                        $"Project expense: {request.Name} - {expenseRequest.Name}",
                        "System", // FullName
                        "system@donation.app", // Email
                        expenseRequest.Value,
                        expenseWallet.Id, // Use the ID of the wallet matching the expense's currency
                        1, // Default userId (admin)
                        TransactionType.OUT);

                    expenseWallet.AddNewTransaction(transaction);
                    await _walletRepository.UpdateAsync(expenseWallet); // Save this specific wallet
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
            if (!string.IsNullOrEmpty(request.Name) || !string.IsNullOrEmpty(request.Description) || !string.IsNullOrEmpty(request.AdditionalText))
            {
                project.UpdateBasicInfo(
                    request.Name ?? project.Name,
                    request.Description ?? project.Description,
                    request.AdditionalText ?? project.AdditionalText
                );
            }

            // Update dates if provided
            if (request.StartedDate.HasValue || request.CompletedDate.HasValue)
            {
                project.UpdateDates(request.StartedDate, request.CompletedDate);
            }

            // Update status
            project.UpdateStatus(request.Status);

            // Update project type if different from current value
            if (request.ProjectType.HasValue && request.ProjectType != project.ProjectType)
            {
                project.UpdateProjectType(request.ProjectType.Value);
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
               new Money(request.Value, new Currency(request.CurrencyCode)), // Use CurrencyCode
                request.Code);

            project.AddExpense(expense);

            // Create transaction for this expense
            if (request.CreateTransaction)
            {
                // Fetch wallet using CurrencyCode
                var wallet = await _walletRepository.GetBySpecAsync(
                    new WalletByNameWithTransactionssSpec(request.CurrencyCode)); 

                if (wallet == null)
                {
                     return BadRequest($"Wallet for currency {request.CurrencyCode} not found.");
                }
                
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

            await _projectRepository.UpdateAsync(project);
            return Ok();
        }

        [HttpPut("{projectId}/expenses/{expenseId}")]
        public async Task<IActionResult> UpdateExpense(long projectId, long expenseId, [FromBody] UpdateExpenseRequest request)
        {
            var projectSpec = new ProjectByIdWithExpensesSpec(projectId); // Ensure this spec loads expenses
            var project = await _projectRepository.GetBySpecAsync(projectSpec);
            if (project == null) return NotFound("Project not found.");

            var expenseToUpdate = project.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expenseToUpdate == null) return NotFound("Expense not found.");

            // Crucial Check: Prevent currency change during update
            if (!string.Equals(request.CurrencyCode, expenseToUpdate.Amount.Currency.Code, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Currency cannot be changed during an update. Please delete the expense and add it again with the new currency.");
            }

            // If currencies are the same, proceed with update
            var newMoney = new Money(request.Value, new Currency(request.CurrencyCode));
            project.UpdateExpense(expenseId, request.Name, request.Date, newMoney, request.Code);

            // Transaction Update Logic
            var walletSpec = new WalletByNameWithTransactionssSpec(request.CurrencyCode);
            var wallet = await _walletRepository.GetBySpecAsync(walletSpec);
            if (wallet == null)
            {
                return BadRequest($"Wallet for currency {request.CurrencyCode} not found. Cannot update transaction.");
            }

            var transactionToUpdate = wallet.Transactions.FirstOrDefault(t => t.Code == expenseToUpdate.Code && t.TransactionType == TransactionType.OUT); 
            // Assuming expense code is unique enough for this transaction, or add more specific identifiers if needed.
            
            if (transactionToUpdate != null)
            {
                // Update transaction amount
                // Note: Transaction class might need an UpdateAmount method or properties need to be settable.
                // For this example, assuming direct property update or a method exists.
                // If Transaction.Amount is private set, you'll need a method in Transaction class.
                // Let's assume a method UpdateAmount exists for now or properties are public set.
                // transactionToUpdate.UpdateAmount(request.Value); // Ideal
                
                // If direct property access (less ideal but for example):
                // We need to remove the old and add a new one if amount is immutable in Transaction
                // For simplicity, let's assume we can modify it or replace it.
                // This part highly depends on the Transaction entity's design.
                // A safe way: delete old, create new.
                wallet.DeleteTransaction(transactionToUpdate.Id);
                var newTransaction = new Transaction(
                        request.Code, // Use new code from request if it can change, or old code if it's an identifier
                        $"Project expense (updated): {project.Name} - {request.Name}",
                        "System",
                        "system@donation.app",
                        request.Value,
                        wallet.Id,
                        1, // Default userId
                        TransactionType.OUT);
                wallet.AddNewTransaction(newTransaction);
                
                await _walletRepository.UpdateAsync(wallet);
            }
            else
            {
                // Log or handle if the original transaction was not found, though it should exist.
                // Potentially, create a new transaction if it's missing for some reason.
            }
            
            await _projectRepository.UpdateAsync(project);
            return NoContent();
        }

        [HttpDelete("{projectId}/expenses/{expenseId}")]
        public async Task<ActionResult> DeleteExpense(long projectId, long expenseId)
        {
            try
            {
                var projectSpec = new ProjectByIdWithExpensesSpec(projectId);
                var project = await _projectRepository.GetBySpecAsync(projectSpec);
                if (project == null) return NotFound("Project not found.");

                // Find the expense
                var expenseToDelete = project.Expenses.FirstOrDefault(e => e.Id == expenseId);
                if (expenseToDelete == null) return NotFound("Expense not found.");
                var wallet = await _walletRepository.GetBySpecAsync(
                    new WalletByNameWithTransactionssSpec(expenseToDelete.Amount.Currency.Code));

                if (wallet != null)
                {
                    var transactionToDelete = wallet.Transactions
                        .FirstOrDefault(t => t.Code == expenseToDelete.Code && t.TransactionType == TransactionType.OUT);

                    if (transactionToDelete != null)
                    {
                        wallet.DeleteTransaction(transactionToDelete.Id);
                        await _walletRepository.UpdateAsync(wallet);
                    }
                }

                // Remove the expense from the project
                project.DeleteExpense(expenseId);
                await _projectRepository.UpdateAsync(project);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the expense.");
            }
        }

    }
}
