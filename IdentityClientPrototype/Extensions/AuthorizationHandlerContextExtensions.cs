using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityClientPrototype.Extensions
{
    public static class AuthorizationHandlerContextExtensions
    {
        public static bool HasRole(this AuthorizationHandlerContext context, string role)
        {
            var sw = new Stopwatch();
            sw.Start();

            if (!context.User.Identity?.IsAuthenticated ?? false)
                return false;

            if (context.Resource is not DefaultHttpContext httpContext)
                return false;

            var auth = httpContext.Request.Headers.Authorization.First();
            var jwt = auth.Substring(7);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            var result = token.Claims.Any(x => x.Type == Claims.Role && x.Value == role);

            sw.Stop();
            Trace.WriteLine(sw.Elapsed);

            return result;
        }
    }
}
