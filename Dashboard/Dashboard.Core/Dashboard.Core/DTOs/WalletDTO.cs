using Dashboard.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.DTOs
{
    public class WalletDTO
    {
        public long Id { get; set; }
        public string Name { get;set; }
        public Currency Currency { get; set; }

        public List<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();

        public Money TotalAmount { get; set; }
    }
}
