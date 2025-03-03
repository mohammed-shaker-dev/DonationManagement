﻿using Ardalis.GuardClauses;
using Dashboard.Core.DTOs;
using Dashboard.Core.Events;
using Dashboard.Core.ValueObjects;
using SharedKernel;
using SharedKernel.Interfaces;

namespace Dashboard.Core.WalletAggregate
{
    public class Wallet: BaseEntity<long>,IAggregateRoot
    {
        private Wallet()
        {
            
        }
        public Wallet(string name, Currency currency)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Currency = currency;
        }
        public string Name { get; private set; }
        public Currency Currency { get; private set; }

        private readonly List<Transaction> _transactions = new List<Transaction>();
        public IEnumerable<Transaction> Transactions => _transactions.AsReadOnly();

        public void AddNewTransaction(Transaction transaction)
        {
            Guard.Against.Null(transaction, nameof(transaction));

            _transactions.Add(transaction);


            var transactionCreated = new TransactionCreatedEvent(transaction);
            Events.Add(transactionCreated);
        }
        public void UpdateTransaction(long transactionId, decimal amount, string code, string comment, string fullName)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
            Guard.Against.Null(transaction, nameof(transaction));
            transaction.Update(amount, code, comment, fullName);
            var transactionUpdated = new TransactionUpdatedEvent(transaction);
            Events.Add(transactionUpdated);
        }

        public void DeleteTransaction(long transactionId)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
            Guard.Against.Null(transaction, nameof(transaction));
            _transactions.Remove(transaction);
            var transactionDeleted = new TransactionDeletedEvent(transaction);
            Events.Add(transactionDeleted);
        }
        public Money GetAmmount()
        {
            var amount = GetDepositAmount().Amount - GetWithdrawalAmount().Amount;  
            return new Money(amount, Currency);
        }
        public Money GetDepositAmount()
        {
            decimal inAmount = 0;
            if (_transactions.Any(t => t.TransactionType == SharedKernel.Enums.TransactionType.IN))
                inAmount = _transactions.Where(t => t.TransactionType == SharedKernel.Enums.TransactionType.IN).Sum(t => t.Amount);
          
            return new Money(inAmount, Currency);
        }  
        public Money GetWithdrawalAmount()
        {
            decimal outAmount = 0;

            if (_transactions.Any(t => t.TransactionType == SharedKernel.Enums.TransactionType.OUT))
                outAmount = _transactions.Where(t => t.TransactionType == SharedKernel.Enums.TransactionType.OUT).Sum(t => t.Amount);
            return new Money(outAmount, Currency);
        }
        public WalletDTO ToDto()
        {
            return new WalletDTO
            {
                Id=Id,
                Name = Name,
                Currency = Currency,
                Transactions = Transactions.Select(t => t.ToDto(Currency.Code)).ToList(),
                TotalAmount = GetAmmount(),
                DepositAmount = GetDepositAmount(),
                WithdrawalAmount = GetWithdrawalAmount()
            };
        }
    }
}
