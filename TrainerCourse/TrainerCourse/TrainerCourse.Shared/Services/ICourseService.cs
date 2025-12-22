using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;

namespace TrainerCourse.Shared.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetCourseAsync();
        Task<Course> GetCourseByIdAsync(int id);
        Task<Course> AddCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
        Task<List<Course>> SearchCoursesAsync(string searchTerm);
        Task<ImageUploadResponse> UploadImageAsync(IBrowserFile file);
        Task<string> GetImageUrl(string fileName);
        Task<ImageUploadResponse> UploadCameraImageAsync(string imageDataUrl, string fileName = "camera_capture.jpg");
    }
}
