using Dashboard.Core.SyncedAggregates;
using Dashboard.Core.WalletAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure.Data
{
    public class AppDbContextSeed
    {
        private Wallet USWallet => new Wallet("US", Core.ValueObjects.Currency.USD);
        private Wallet SYPWallet => new Wallet("SYP", Core.ValueObjects.Currency.SYP);
        private User user => new User("admin", "admin@admin.com", "L@tt@kiaP@ssw0rd");

        private readonly AppDbContext _context;
        private readonly ILogger<AppDbContextSeed> _logger;

        public AppDbContextSeed(AppDbContext context,
      ILogger<AppDbContextSeed> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task SeedAsync(DateTimeOffset testDate, int? retry = 0)
        {
            _logger.LogInformation($"Seeding data - testDate: {testDate}");
            _logger.LogInformation($"DbContext Type: {_context.Database.ProviderName}");
            try
            {
                if (_context.IsRealDatabase())
                {
                    // apply migrations if connecting to a SQL database
                    _context.Database.Migrate();
                }
                if (!await _context.Wallets.AnyAsync())
                {
                    await _context.Wallets.AddAsync(USWallet);
                    await _context.Wallets.AddAsync(SYPWallet);
                    await _context.SaveChangesAsync();
                }
                if (!await _context.Users.AnyAsync())
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
