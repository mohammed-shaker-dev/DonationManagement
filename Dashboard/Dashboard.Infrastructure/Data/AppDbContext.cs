using Microsoft.EntityFrameworkCore;
using Dashboard.Core.SyncedAggregates;
using Dashboard.Core.WalletAggregate;
using MediatR;
using System.Reflection;
using SharedKernel;

namespace Dashboard.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public AppDbContext(DbContextOptions<AppDbContext> options,IMediator mediator) :base(options)
        {
            _mediator = mediator;
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasSequence<long>("TransactionCodeSequence", "dbo")
          .StartsAt(1000000001) // Ensure an 8-digit starting value
          .IncrementsBy(1);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            int result= await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            if (_mediator == null) return result;

            var entitieswithEvents= ChangeTracker.Entries().Select(e=>e.Entity as BaseEntity<long>)
                .Where(e=>e?.Events!=null&&e.Events.Any()).ToArray();

            foreach (var entity in entitieswithEvents)
            {
                var events=entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false);
                }
            }
            return result;
        }
        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
