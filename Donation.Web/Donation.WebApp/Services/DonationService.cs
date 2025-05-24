using Dashboard.Core.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace Donation.Web.Services
{
    public class DonationService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<DonationService> _logger;
        private readonly IMemoryCache _cache;

        public DonationService(HttpService httpService, ILogger<DonationService> logger)
        {
            _httpService = httpService;
            _logger = logger;
            _cache = new MemoryCache(new MemoryCacheOptions()); // Create in-memory cache
        }
        public async Task<WalletDTO> GetTransactionByCode(string code)
        {
            try
            {
                var wallet = await _httpService.HttpGetAsync<WalletDTO>($"wallets/trx-code?code={code}");
                return (wallet);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw new Exception(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<WalletDTO>> GetWalletByNameAsync(string walletName)
        {
            string cacheKey = $"wallet_{walletName}";
            if (_cache.TryGetValue(cacheKey, out List<WalletDTO> cachedData))
            {
                _logger.LogInformation($"Cache hit: {cacheKey}");
                return cachedData;
            }
            try
            {
                return (await _httpService.HttpGetAsync<List<WalletDTO>>($"wallets/by-name?walletName={walletName}"));
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }
        public async Task<List<WalletDTO>> GetAllDepositAsync()
        {
            string cacheKey = "all_wallets_Deposit";

            if (_cache.TryGetValue(cacheKey, out List<WalletDTO> cachedData))
            {
                _logger.LogInformation($"Cache hit: {cacheKey}");
                return cachedData;
            }

            try
            {
                var result = await _httpService.HttpGetAsync<List<WalletDTO>>("wallets/Deposit");

                if (result != null)
                {
                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message}");
                return null;
            }
        }
        public async Task<List<WalletDTO>> GetAllAsync()
        {
            string cacheKey = "all_wallets";

            if (_cache.TryGetValue(cacheKey, out List<WalletDTO> cachedData))
            {
                _logger.LogInformation($"Cache hit: {cacheKey}");
                return cachedData;
            }

            try
            {
                var result = await _httpService.HttpGetAsync<List<WalletDTO>>("wallets");

                if (result != null)
                {
                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message}");
                return null;
            }
        }
        public async Task<TransactionDTO> GetTransactionByCodeAsync(string trxCode)
        {
            string cacheKey = "AllTransactions";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, TransactionDTO> cachedTransactions))
            {
                _logger.LogInformation($"Cache hit: {cacheKey}");

                if (cachedTransactions.TryGetValue(trxCode, out var transaction))
                {
                    return transaction;
                }
            }

            try
            {
                _logger.LogInformation($"Cache miss: {cacheKey}, fetching from API...");

                var wallets = await _httpService.HttpGetAsync<List<WalletDTO>>("wallets");
                if (wallets == null || wallets.Count == 0)
                {
                    _logger.LogWarning("No wallets found.");
                    return null;
                }
                var transactionCache = new Dictionary<string, TransactionDTO>();
                foreach (var wallet in wallets)
                {
                    if (wallet?.Transactions != null)
                    {
                        foreach (var transaction in wallet.Transactions)
                        {
                            if (!transactionCache.ContainsKey(transaction.Code))
                            {
                                transactionCache[transaction.Code] = transaction;
                            }
                        }
                    }
                }
                _cache.Set(cacheKey, transactionCache, TimeSpan.FromMinutes(10));

                _logger.LogInformation($"Cached {transactionCache.Count} transactions.");
                return transactionCache.TryGetValue(trxCode, out var trx) ? trx : null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message}");
                return null;
            }
        }


        public async Task<List<WalletDTO>> ListPagedAsync(int pageSize)
        {
            try
            {
                return (await _httpService.HttpGetAsync<List<WalletDTO>>($"Wallets"));
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }
    }
}
