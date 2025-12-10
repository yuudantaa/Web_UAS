using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;
using TrainerCourse.Shared.Services;

namespace TrainerCourse.Shared.Method
{
    public class CourseService : ICourseService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7285/courses";
        private const string BaseApiUrl = "https://localhost:7285";

        public CourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Course> AddCourseAsync(Course course)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, course);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Course>();
        }

        public async Task DeleteCourseAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return; // Success, no content to return
                }

                // Handle specific error cases
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new HttpRequestException($"Course with ID {id} not found");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(errorMessage);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error deleting course: {response.StatusCode} - {errorMessage}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting course: {ex.Message}", ex);
            }
        }

        public async Task<List<Course>> GetCourseAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Course>>(BaseUrl);

                // Debug: Cek apakah data gambar ada
                if (response != null)
                {
                    foreach (var course in response)
                    {
                        Console.WriteLine($"Course: {course.CourseName}, ImageUrl: {course.ImageUrl}");
                    }
                }

                return response ?? new List<Course>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Course: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Course>($"{BaseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching course by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseUrl, course);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Course>();
        }

        public async Task<List<Course>> SearchCoursesAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Course>>($"{BaseUrl}/search/{searchTerm}");
                return response ?? new List<Course>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching courses: {ex.Message}");
                return new List<Course>();
            }
        }

        public async Task<ImageUploadResponse> UploadImageAsync(IBrowserFile file)
        {
            try
            {
                // Validate file
                var validExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var extension = Path.GetExtension(file.Name);

                if (!validExtensions.Contains(extension.ToLower()))
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = $"Invalid extension. Valid extensions: {string.Join(", ", validExtensions)}"
                    };
                }

                if (file.Size > 5 * 1024 * 1024) // 5MB
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "File size too large. Maximum size is 5MB."
                    };
                }

                // Create multipart form data
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                // Send request to correct endpoint
                var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UploadResult>();
                    return new ImageUploadResponse
                    {
                        Success = true,
                        FileName = result.FileName,
                        ImageUrl = result.ImageUrl
                    };
                }
                else
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ErrorResult>();
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = errorResult?.Message ?? "Upload failed"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = $"Error uploading image: {ex.Message}"
                };
            }
        }

        public async Task<string> GetImageUrlAsync(string fileName)
        {
            return await Task.FromResult($"https://localhost:7285/uploads/{fileName}");
        }
    }
}
