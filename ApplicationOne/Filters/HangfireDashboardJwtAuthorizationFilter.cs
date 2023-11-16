using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationOne.Filters;

public class HangfireDashboardJwtAuthorizationFilter : IDashboardAuthorizationFilter
{
    private static readonly string HangfireCookieName = "hangfire-auth";
    private static readonly int CookieExpirtionMinutes = 60;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ILogger<HangfireDashboardJwtAuthorizationFilter> _logger;
    public HangfireDashboardJwtAuthorizationFilter(TokenValidationParameters tokenValidationParameters, ILogger<HangfireDashboardJwtAuthorizationFilter> logger)
    {
        _tokenValidationParameters = tokenValidationParameters;
        _logger = logger;
    }
    public bool Authorize([NotNull] DashboardContext context)
    {
        try
        {
            var httpContext = context.GetHttpContext();

            var accees_token = string.Empty;

            var setCookie = false;

            if (httpContext.Request.Query.ContainsKey("access_token"))
            {
                accees_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else
            {
                accees_token = httpContext.Request.Cookies[HangfireCookieName];
            }

            if (string.IsNullOrEmpty(accees_token))
                return false;

            if (setCookie)
            {
                httpContext.Response.Cookies.Append(HangfireCookieName, accees_token);
                _ = new CookieOptions()
                {
                    Expires = DateTime.UtcNow.AddMinutes(CookieExpirtionMinutes),
                };
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("HangfireDashboardJwtAuthorizationFilter error: {0}", ex.Message);
            return false;
        }
    }
}
