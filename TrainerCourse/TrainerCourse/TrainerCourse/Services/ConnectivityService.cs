using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Services;

namespace TrainerCourse.Services
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly IConnectivity _mauiConnectivity;
        public bool IsConnected => _mauiConnectivity.NetworkAccess == NetworkAccess.Internet;

        public event EventHandler<bool> ConnectivityChanged;

        public ConnectivityService()
        {
            _mauiConnectivity = Microsoft.Maui.Networking.Connectivity.Current;
            _mauiConnectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        public Task<bool> CheckConnectionAsync()
        {
            return Task.FromResult(IsConnected);
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var isConnected = e.NetworkAccess == NetworkAccess.Internet;
            ConnectivityChanged?.Invoke(this, isConnected);
        }

        ~ConnectivityService()
        {
            _mauiConnectivity.ConnectivityChanged -= OnConnectivityChanged;
        }
    }
}
