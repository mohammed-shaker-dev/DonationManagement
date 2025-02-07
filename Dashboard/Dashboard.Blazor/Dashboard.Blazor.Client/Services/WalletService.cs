using Dashboard.Core.DTOs;
using Dashboard.Core.WalletAggregate;

namespace Dashboard.Blazor.Client.Services
{
    public class WalletService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<WalletService> _logger;

        public WalletService(HttpService httpService, ILogger<WalletService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }
        public async Task<TransactionDTO> CreateAsync(TransactionDTO transaction)
        {
            return (await _httpService.HttpPostAsync<TransactionDTO>("Wallets", transaction));
        }
        public async Task<List<Wallet>> ListPagedAsync(int pageSize)
        {
            return (await _httpService.HttpGetAsync<List<Wallet>>($"Wallets"));
        }
    }
}
