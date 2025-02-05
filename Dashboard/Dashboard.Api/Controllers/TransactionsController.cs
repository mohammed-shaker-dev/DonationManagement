﻿using Ardalis.GuardClauses;
using Dashboard.Core.DTOs;
using Dashboard.Core.WalletAggregate;
using Dashboard.Core.WalletAggregate.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interfaces;


namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IRepository<Wallet> _walletRepository;
        public TransactionsController(IRepository<Wallet> walletRepository)
        {
            _walletRepository = walletRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetTransactions()
        {
            var wallets = await _walletRepository.ListAsync(new WalletsWithTransactionsSpec());
            return Ok(wallets);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> InsertTransaction([FromBody] TransactionDTO transaction)
        {
            var userId = 1;
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data.");
            }
            var wallet = await _walletRepository.GetBySpecAsync(new WalletByIdWithTransactionssSpec((long)transaction.WalletId));
           if (wallet == null) throw new NotFoundException("", "wallet not found");
            var createdTransaction = new Transaction(transaction?.FullName, transaction?.Email, transaction.Amount, (long)transaction.WalletId, userId,transaction.TransactionType);
            wallet.AddNewTransaction(createdTransaction);
            await _walletRepository.UpdateAsync(wallet);
            return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Code }, transaction);
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
