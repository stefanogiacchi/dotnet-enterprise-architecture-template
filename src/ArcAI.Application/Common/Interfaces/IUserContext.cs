using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcAI.Application.Common.Interfaces
{
    public interface IUserContext
    {
        // User identification
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }

        // Authorization
        IEnumerable<string> Roles { get; }
        IEnumerable<KeyValuePair<string, string>> Claims { get; }
        bool IsAuthenticated { get; }

        // Helper methods
        bool IsInRole(string role);
        bool HasClaim(string claimType, string? claimValue = null);
    }
}
