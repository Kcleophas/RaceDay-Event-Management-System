using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Mvc.Services
{
    public class SessionBearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionBearerTokenHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            string? accessToken = context?.Session
                .GetString("AccessToken");

            if (request.Headers.Authorization == null &&
                !string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
