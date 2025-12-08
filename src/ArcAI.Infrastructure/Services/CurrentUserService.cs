using ArcAI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ArcAI.Infrastructure.Services;

/// <summary>
/// Implementation of current user service that extracts user information from HTTP context.
/// Provides access to authenticated user details throughout the application.
/// </summary>
public class CurrentUserService : ICurrentUserService, IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClaimsPrincipal? _user;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _user = _httpContextAccessor.HttpContext?.User;
    }

    /// <inheritdoc/>
    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? _user?.FindFirstValue("sub")
                            ?? _user?.FindFirstValue("userId");

    /// <inheritdoc/>
    public string? UserName => _user?.FindFirstValue(ClaimTypes.Name)
                              ?? _user?.FindFirstValue("name")
                              ?? _user?.FindFirstValue("preferred_username");

    /// <inheritdoc/>
    public string? Email => _user?.FindFirstValue(ClaimTypes.Email)
                           ?? _user?.FindFirstValue("email");

    /// <inheritdoc/>
    public IEnumerable<string> Roles
    {
        get
        {
            if (_user == null)
                return Enumerable.Empty<string>();

            return _user.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .Union(_user.FindAll("role").Select(c => c.Value))
                .Distinct();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> Claims
    {
        get
        {
            if (_user == null)
                return Enumerable.Empty<KeyValuePair<string, string>>();

            return _user.Claims
                .Select(c => new KeyValuePair<string, string>(c.Type, c.Value))
                .ToList();
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return _user?.IsInRole(role) ?? false;
    }

    /// <inheritdoc/>
    public bool HasClaim(string claimType, string? claimValue = null)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return false;

        if (_user == null)
            return false;

        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return _user.HasClaim(c => c.Type == claimType);
        }

        return _user.HasClaim(claimType, claimValue);
    }
}