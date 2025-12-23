using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerCourse.Shared.Services
{
    public interface IConnectivityService
    {
        bool IsConnected { get; }
        Task<bool> CheckConnectionAsync();
        event EventHandler<bool> ConnectivityChanged;
    }
}
