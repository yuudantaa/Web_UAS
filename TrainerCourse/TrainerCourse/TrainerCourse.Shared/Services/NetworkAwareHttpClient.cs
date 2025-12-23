using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerCourse.Shared.Services
{
    public class NetworkAwareHttpClient : DelegatingHandler
    {
        private readonly IConnectivityService _connectivityService;

        public NetworkAwareHttpClient(IConnectivityService connectivityService, HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _connectivityService = connectivityService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Check connectivity before making request
            if (!_connectivityService.IsConnected)
            {
                throw new HttpRequestException("No internet connection available.");
            }

            int maxRetries = 3;
            int retryCount = 0;
            TimeSpan delay = TimeSpan.FromSeconds(1);

            while (retryCount < maxRetries)
            {
                try
                {
                    // Check connectivity before each attempt
                    if (!await _connectivityService.CheckConnectionAsync())
                    {
                        throw new HttpRequestException("Lost internet connection.");
                    }

                    var response = await base.SendAsync(request, cancellationToken);

                    // Check for network errors
                    if (IsNetworkError(response.StatusCode))
                    {
                        throw new HttpRequestException($"Network error: {response.StatusCode}");
                    }

                    return response;
                }
                catch (HttpRequestException ex) when (retryCount < maxRetries - 1)
                {
                    retryCount++;

                    // Exponential backoff
                    await Task.Delay(delay * retryCount, cancellationToken);

                    // Re-check connectivity
                    if (!await _connectivityService.CheckConnectionAsync())
                    {
                        throw new HttpRequestException("No internet connection available.", ex);
                    }
                }
            }

            throw new HttpRequestException($"Request failed after {maxRetries} attempts.");
        }

        private bool IsNetworkError(System.Net.HttpStatusCode statusCode)
        {
            return statusCode == System.Net.HttpStatusCode.RequestTimeout ||
                   statusCode == System.Net.HttpStatusCode.GatewayTimeout ||
                   statusCode == System.Net.HttpStatusCode.BadGateway ||
                   statusCode == System.Net.HttpStatusCode.ServiceUnavailable;
        }
    }
}
