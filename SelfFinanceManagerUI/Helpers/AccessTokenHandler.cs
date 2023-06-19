using System.Net.Http;
using System.Net.Http.Headers;

namespace SelfFinanceManagerUI.Helpers
{
    public class AccessTokenHandler : DelegatingHandler
    {
        private readonly TokenProvider _tokenProvider;
        private readonly ILogger<AccessTokenHandler> _logger;

        public AccessTokenHandler(TokenProvider tokenProvider, ILogger<AccessTokenHandler> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var accessToken = _tokenProvider.AccessToken;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    _logger.LogWarning("Access token not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding access token to request");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}