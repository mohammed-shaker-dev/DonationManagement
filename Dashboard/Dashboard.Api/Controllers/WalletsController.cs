﻿using Ardalis.GuardClauses;
using Dashboard.Core.DTOs;
using Dashboard.Core.WalletAggregate;
using Dashboard.Core.WalletAggregate.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SharedKernel.Blazor.Shared;
using SharedKernel.Interfaces;


namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IRepository<Wallet> _walletRepository;
        public WalletsController(IRepository<Wallet> walletRepository)
        {
            _walletRepository = walletRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletDTO>>> GetWallets()
        {
            var wallets = await _walletRepository.ListAsync(new WalletsWithTransactionsSpec());
            return Ok(wallets.Select(w=>w.ToDto()).ToList());
        }

        [HttpGet("Deposit")]
        public async Task<ActionResult<IEnumerable<WalletDTO>>> GetAllDepositAsync()
        {
            var wallets = await _walletRepository.ListAsync(new WalletsDepositOnlySpec());
            return Ok(wallets.Select(w => w.ToDto()).ToList());
        }
        [HttpGet("by-name")]
        public async Task<ActionResult<IEnumerable<WalletDTO>>> GetWalletByName([FromQuery] string walletName)
        {
            var wallets = await _walletRepository.ListAsync(new WalletByNameWithTransactionssSpec(walletName));
            return Ok(wallets.Select(w => w.ToDto()).ToList());
        }    

        [EnableRateLimiting("fixed")]
        [HttpGet("trx-code")]
        public async Task<ActionResult<WalletDTO>> GetTransactionByCode([FromQuery] string code)
        {
            var wallets = await _walletRepository.ListAsync(new TransactionByCodeSpec(code));
            var walletDto = wallets?.FirstOrDefault(w => w?.Transactions?.Any() == true);
            if (walletDto == null)
                return NotFound();
            return Ok(walletDto?.ToDto());
        }
        [HttpGet("from-to")]
        public async Task<IActionResult> GetTransactions([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var spec = new TransactionsBetweenDateRangeSpec(fromDate, toDate);
            var wallets = await _walletRepository.ListAsync(spec);
            return Ok(wallets.Select(w=>w.ToDto()));
        }
        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> InsertTransaction([FromBody] TransactionRequest transaction)
        {
            var userId = 1;
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }
            var wallet = await _walletRepository.GetBySpecAsync(new WalletByIdWithTransactionssSpec((long)transaction.WalletId));
           if (wallet == null) throw new NotFoundException("", "wallet not found");
            var createdTransaction = new Transaction(transaction.Code,transaction.Comment,transaction?.FullName, transaction?.Email, transaction.Amount, (long)transaction.WalletId, userId,transaction.TransactionType);
            wallet.AddNewTransaction(createdTransaction);
            await _walletRepository.UpdateAsync(wallet);
            return CreatedAtAction(nameof(GetWallets), new { id = transaction.Code }, transaction);
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> UpdateTransaction(long transactionId, [FromBody] TransactionRequest transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }

            var wallet = await _walletRepository.GetBySpecAsync(new WalletByIdWithTransactionssSpec((long)transaction.WalletId));
            if (wallet == null) throw new NotFoundException("", "wallet not found");

            wallet.UpdateTransaction(transactionId, transaction.Amount, transaction.Code, transaction?.Comment, transaction?.FullName);
            await _walletRepository.UpdateAsync(wallet);

            return NoContent();
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransaction(long transactionId, [FromQuery] long walletId)
        {
            var wallet = await _walletRepository.GetBySpecAsync(new WalletByIdWithTransactionssSpec(walletId));
            if (wallet == null) throw new NotFoundException("", "wallet not found");

            wallet.DeleteTransaction(transactionId);
            await _walletRepository.UpdateAsync(wallet);

            return NoContent();
        }
        //[HttpGet("search")]
        //public ActionResult<IEnumerable<Transaction>> SearchTransactions([FromQuery] string searchTerm)
        //{
        //    if (string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        return Ok(Transactions);
        //    }

        //    var filteredTransactions = Transactions
        //        .Where(t => t.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        //                    t.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        //        .ToList();

        //    return Ok(filteredTransactions);
        //}
    }
}
