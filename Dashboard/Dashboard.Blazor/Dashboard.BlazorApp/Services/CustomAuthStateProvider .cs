using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Dashboard.Core.SyncedAggregates;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity()); // Default: Not authenticated
    private const string AuthStorageKey = "authUser";
    private readonly IConfiguration _configuration;
    public CustomAuthStateProvider(IJSRuntime jsRuntime, IConfiguration configuration)
    {
        _jsRuntime = jsRuntime;
        _configuration = configuration;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AuthStorageKey);

        if (!string.IsNullOrEmpty(userJson))
        {
            try
            {
                _currentUser = DeserializeClaims(userJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing claims: " + ex.Message);
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        return new AuthenticationState(_currentUser);
    }
    public async Task<bool> LoginAsync(string username, string password)
    {
        // Get users from appsettings.json
        var users = _configuration.GetSection("Authentication:Users").Get<List<User>>();

        // Find the user
        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
             }, "fake-auth");

            _currentUser = new ClaimsPrincipal(identity);

            // Save to localStorage
            var jsonClaims = SerializeClaims(identity);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AuthStorageKey, jsonClaims);

            // Notify authentication state change
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));

            return true;
        }

        return false;
    }
 
    public static string SerializeClaims(ClaimsIdentity user)
    {
        var claimsList = user.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList();
        return JsonSerializer.Serialize(claimsList);
    }
    public static ClaimsPrincipal DeserializeClaims(string json)
    {
        var claimsList = JsonSerializer.Deserialize<List<ClaimDto>>(json);
        var claims = claimsList.Select(c => new Claim(c.Type, c.Value)).ToList();
        var identity = new ClaimsIdentity(claims, "fake-auth");
        return new ClaimsPrincipal(identity);
    }

    public async Task Logout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AuthStorageKey);
    }
    public class ClaimDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
