using Microsoft.JSInterop;
using TrainerCourse.Shared.Services;

namespace TrainerCourse.Web.Services
{
    public class WebConnectivityService : IConnectivityService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isConnected = true;
        public bool IsConnected => _isConnected;
        public event EventHandler<bool> ConnectivityChanged;

        public WebConnectivityService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                // Check initial connectivity
                _isConnected = await CheckConnectionAsync();

                // Listen to online/offline events
                await _jsRuntime.InvokeVoidAsync(
                    "TrainerCourse.connectivity.init",
                    DotNetObjectReference.Create(this)
                );
            }
            catch
            {
                // Fallback to navigator.onLine
                _isConnected = await _jsRuntime.InvokeAsync<bool>("eval", "navigator.onLine");
            }
        }

        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                // Try to ping a reliable endpoint
                return await _jsRuntime.InvokeAsync<bool>("TrainerCourse.connectivity.checkConnection");
            }
            catch
            {
                // Fallback to navigator.onLine
                return await _jsRuntime.InvokeAsync<bool>("eval", "navigator.onLine");
            }
        }

        [JSInvokable]
        public void UpdateConnectionStatus(bool isOnline)
        {
            var oldStatus = _isConnected;
            _isConnected = isOnline;

            if (oldStatus != _isConnected)
            {
                ConnectivityChanged?.Invoke(this, _isConnected);
            }
        }

    }
}
