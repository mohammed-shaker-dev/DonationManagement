using SharedKernel;
using SharedKernel.Interfaces;


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
