using Ardalis.GuardClauses;
using Dashboard.Core.DTOs;
using Dashboard.Core.Events;
using Dashboard.Core.ValueObjects;
using SharedKernel;
using SharedKernel.Enums;

namespace Dashboard.Core.WalletAggregate
{
    public class Transaction : BaseEntity<long>
    {
        private Transaction()
        {
                
        }
        public Transaction(
            string code,
            string comment,
            string fullName,
            string email,
            decimal amount,
            long walletId,
            long userId,
            TransactionType transactionType)
        {
             Code=Guard.Against.NullOrWhiteSpace(code,nameof(code));
            WalletId=Guard.Against.Negative(walletId,nameof(walletId));
            userId=Guard.Against.Negative(userId,nameof(userId));
            Email = email;
            Comment = comment;
            TransactionType=transactionType;
            Amount = amount;
            FullName=fullName;
            CreatedDate = DateTime.UtcNow;
        }
        public DateTime UpdatedDate { get; private set; }
        public string? LastUpdatedBy { get; private set; }
        public string Code { get; private set; }
        public string? FullName { get; private set; }
        public string? Comment { get; private set; }
        public string? Email { get; private set; }
        public decimal Amount  { get; private set; }
        public long  WalletId { get; private set; }
        public long UserId { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public void UpdateAmount(decimal amount )
        {
            Amount = amount;
            var transactionUpdatedEvent = new TransactionUpdatedEvent(this);
            Events.Add(transactionUpdatedEvent);
        }
        public TransactionDTO ToDto(string currency)
        {
            return new TransactionDTO
            {
                Id = Id,
                FullName =FullName,
                Amount = Amount,
                Code = Code,
                Comment = Comment,
                Email = Email,
                LastUpdatedBy = LastUpdatedBy,
                TransactionType = TransactionType,
                UpdatedDate = UpdatedDate,
                UserId = UserId,
                WalletId = WalletId,
                CreatedDate = CreatedDate,
                Currency= currency
            };
        }
    }
}
