using SharedKernel;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.SyncedAggregates
{
    public class User :BaseEntity<long>,IAggregateRoot
    {
        public User(string fullName,string emailAddress)
        {
            FullName = fullName;
            EmailAddress = emailAddress;
        }
        public string FullName { get; private set; }
        public string EmailAddress { get; private set; }
        public override string ToString()
        {
            return FullName.ToString();
        }
    }
}
