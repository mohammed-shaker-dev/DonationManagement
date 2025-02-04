using Ardalis.GuardClauses;
using Dashboard.Core.ValueObjects;
using SharedKernel;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.BudgetAggregate
{
    public class Transaction : BaseEntity<long>
    {
        public Transaction(string code,
            string fullName,
            string email,
            Money amount,
            long walletId,
            long userId,
            TransactionType transactionType)
        {
            Code=Guard.Against.NullOrWhiteSpace(code,nameof(code));
            WalletId=Guard.Against.Negative(walletId,nameof(walletId));
            userId=Guard.Against.Negative(userId,nameof(userId));
            Email = email;
            TransactionType=transactionType;
            Amount = amount;
            FullName=fullName;
        }
        public string Code { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public Money Amount  { get; private set; }
        public long? WalletId { get; private set; }
        public long UserId { get; private set; }
        public TransactionType TransactionType { get; private set; }

    }
}
