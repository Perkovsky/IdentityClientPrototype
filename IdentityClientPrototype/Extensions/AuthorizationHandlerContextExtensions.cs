using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace IdentityClientPrototype.Extensions
{
    public static class AuthorizationHandlerContextExtensions
    {
        private const string RolesKey = "roles";

        public static bool HasRole(this AuthorizationHandlerContext context, string role)
        {
            var sw = new Stopwatch();
            sw.Start();

            if (context.HasFailed)
                return false;

            if (!context.User.Identity?.IsAuthenticated ?? false)
                return false;

            var vRoles = context.User.Claims.FirstOrDefault(x => x.Type == RolesKey)?.Value;
            if (string.IsNullOrWhiteSpace(vRoles))
                return false;

            var roles = JsonSerializer.Deserialize<IEnumerable<string>>(vRoles);
            var result = roles.Any(r => r == role);

            sw.Stop();
            Trace.WriteLine(sw.Elapsed);

            return result;
        }
    }
}
