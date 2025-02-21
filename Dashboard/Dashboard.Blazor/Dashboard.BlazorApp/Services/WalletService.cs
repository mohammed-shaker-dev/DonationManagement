using Dashboard.Core.DTOs;
using Dashboard.Core.WalletAggregate;
using SharedKernel.Blazor.Shared;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace Dashboard.BlazorApp.Services
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
        public async Task<TransactionDTO> CreateAsync(TransactionRequest transaction)
        {
            try
            {
                return (await _httpService.HttpPostAsync<TransactionDTO>("wallets", transaction));
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
        public async Task<List<WalletDTO>> GetWalletByNameAsync(string walletName)
        {
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
        public async Task<List<WalletDTO>> GetTransactionsBetweenDateRange(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return (await _httpService.HttpGetAsync<List<WalletDTO>>($"wallets/from-to?fromDate={fromDate}&toDate={toDate}"));
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
        public async Task<List<WalletDTO>> GetAllAsync()
        {
            try
            {
                return (await _httpService.HttpGetAsync<List<WalletDTO>>("wallets"));
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
        public async Task<List<Wallet>> ListPagedAsync(int pageSize)
        {
            try
            {
                return (await _httpService.HttpGetAsync<List<Wallet>>($"Wallets"));
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
        public async Task<bool> UpdateTransactionAsync(long transactionId, TransactionRequest transaction)
        {
            try
            {
                await _httpService.HttpPutAsync<TransactionDTO>($"wallets/{transactionId}", transaction);
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTransactionAsync(long transactionId,long walletID)
        {
            try
            {
               var result= await _httpService.HttpDeleteAsync($"wallets/{transactionId}?walletId={walletID}");
                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }
}
