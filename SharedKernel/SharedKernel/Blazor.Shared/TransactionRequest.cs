using SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Blazor.Shared
{
    public class TransactionRequest
    {
        public long? Id { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string Code { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Comment { get; set; }
        public decimal Amount { get; set; }
        public long? WalletId { get; set; }
        public long UserId { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
