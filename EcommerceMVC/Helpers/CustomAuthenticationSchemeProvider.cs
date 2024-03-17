using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EcommerceMVC.Helpers
{
    public class CustomAuthenticationSchemeProvider : AuthenticationSchemeProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Request.Path.StartsWithSegments("/Admin"))
            {
                return await GetSchemeAsync("AdminAuth");
            }
            else
            {
                return await GetSchemeAsync("CustomerAuth");
            }
        }
    }
}
