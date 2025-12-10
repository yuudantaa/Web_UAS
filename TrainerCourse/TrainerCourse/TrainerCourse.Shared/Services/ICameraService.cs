using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;

namespace TrainerCourse.Shared.Services
{
    public interface ICameraService
    {
        Task<ImageUploadResponse> TakePhotoAsync();
        Task<bool> CheckAndRequestPermissionAsync();
    }
}
