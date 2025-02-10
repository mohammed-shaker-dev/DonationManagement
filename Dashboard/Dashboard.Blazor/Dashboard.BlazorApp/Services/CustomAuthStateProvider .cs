﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity()); // Default: Not authenticated

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public bool Login(string username,string password)
    {
        // Hardcoded authentication
        bool result = false;
        if (username == "admin" && password== "L@tt@kiaP@ssw0rd")
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "fake-auth");

            _currentUser = new ClaimsPrincipal(identity);
            result= true;
        }
        else if (username == "user")
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "User")
            }, "fake-auth");

            _currentUser = new ClaimsPrincipal(identity);
            result= true;
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity()); // Unauthenticated
       
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return result;
    }

    public void Logout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}
