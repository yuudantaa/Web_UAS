using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;
using TrainerCourse.Shared.Services;

namespace TrainerCourse.Services
{
    public class CameraService : ICameraService
    {
        public async Task<bool> CheckAndRequestPermissionAsync()
        {
            try
            {
#if ANDROID
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                }
                return status == PermissionStatus.Granted;
#else
                return false;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Permission error: {ex.Message}");
                return false;
            }
        }

        public async Task<ImageUploadResponse> TakePhotoAsync()
        {
            try
            {
#if ANDROID
                // Check camera permission
                if (!await CheckAndRequestPermissionAsync())
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "Camera permission denied"
                    };
                }

                // Take photo
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null)
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "No photo captured"
                    };
                }

                // Convert to base64 for display
                using var stream = await photo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                var dataUrl = $"data:image/jpeg;base64,{base64}";

                // Generate file name
                var fileName = $"{Guid.NewGuid()}.jpg";

                return new ImageUploadResponse
                {
                    Success = true,
                    FileName = fileName,
                    ImageUrl = dataUrl,
                    Message = "Photo captured successfully"
                };
#else
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = "Camera feature is only available on Android"
                };
#endif
            }
            catch (Exception ex)
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = $"Error capturing photo: {ex.Message}"
                };
            }
        }
    }
}