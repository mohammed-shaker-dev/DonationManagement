using Dashboard.Core.DTOs;
using Dashboard.Core.WalletAggregate;
using SharedKernel.Blazor.Shared;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace Dashboard.BlazorApp.Services
{
    public class WalletService
    {
        private readonly HttpClient _httpClient;

        private readonly HttpService _httpService;
        private readonly ILogger<WalletService> _logger;

        //public WalletService(HttpClient httpClient, ILogger<WalletService> logger)
        //{
        //    _httpClient = httpClient;
        //    _logger = logger;
        //}
        //public async Task<List<WalletDTO>> GetDataFromApiAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetFromJsonAsync< List<WalletDTO>>("wallets");

        //        if (response != null)
        //        {
        //            return response;
        //        }
        //        return null;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"HTTP Request failed: {ex.Message}");
        //        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"General error: {ex.Message}");
        //        return null;
        //    }
        //}
        //    // Method to make a POST request to the server-side API
        //    public async Task<bool> PostDataToApiAsync(MyData data)
        //    {
        //        try
        //        {
        //            var response = await _httpClient.PostAsJsonAsync("api/myendpoint", data);
        //            return response.IsSuccessStatusCode;
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle errors
        //            Console.WriteLine($"Error: {ex.Message}");
        //            return false;
        //        }
        //    }
        //}
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
        public async Task<List<WalletDTO>> GetDataFromApiAsync()
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
    }
}
