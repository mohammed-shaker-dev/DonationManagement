using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Dashboard.Core.SyncedAggregates;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity()); // Default: Not authenticated
    private const string AuthStorageKey = "authUser";

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
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
    public async Task<bool> LoginAsync(string username,string password)
    {
        // Hardcoded authentication
        bool result = false;
        ClaimsIdentity identity = new ClaimsIdentity();

        if (username == "admin" && password== "L@tt@kiaP@ssw0rd")
        {
              identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "fake-auth");

            _currentUser = new ClaimsPrincipal(identity);
            result= true;
        }
        else if (username == "user")
        {
              identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "User")
            }, "fake-auth");

            _currentUser = new ClaimsPrincipal(identity);
            result= true;
        } 
        _currentUser = new ClaimsPrincipal(identity);
 

        if (result)
        {
            var jsonClaims = SerializeClaims(identity);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AuthStorageKey, jsonClaims);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        return result;
    
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
}
