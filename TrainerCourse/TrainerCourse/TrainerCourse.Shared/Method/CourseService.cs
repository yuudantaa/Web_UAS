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
                    var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
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

        public async Task<string> GetImageUrl(string fileName)
        {
            return await Task.FromResult($"https://localhost:7285/uploads/{fileName}");
        }

        public async Task<ImageUploadResponse> UploadCameraImageAsync(string imageDataUrl, string fileName = "camera_capture.jpg")
        {
            try
            {
                // Validate the data URL format
                if (string.IsNullOrEmpty(imageDataUrl) || !imageDataUrl.StartsWith("data:image/"))
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "Invalid image data format"
                    };
                }

                // 1. Extract Base64 data from data URL
                string base64Data;
                if (imageDataUrl.Contains(','))
                {
                    base64Data = imageDataUrl.Split(',')[1];
                }
                else
                {
                    base64Data = imageDataUrl;
                }

                // 2. Convert Base64 to byte array
                var imageBytes = Convert.FromBase64String(base64Data);

                // 3. Validate file size (5MB max)
                if (imageBytes.Length > 5 * 1024 * 1024)
                {
                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "Image size too large. Maximum size is 5MB."
                    };
                }

                // 4. Create multipart form data
                using var content = new MultipartFormDataContent();
                using var byteArrayContent = new ByteArrayContent(imageBytes);

                // Set content type - try to detect from data URL or default to jpeg
                string contentType = "image/jpeg";
                if (imageDataUrl.StartsWith("data:image/png"))
                    contentType = "image/png";
                else if (imageDataUrl.StartsWith("data:image/jpeg"))
                    contentType = "image/jpeg";

                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(byteArrayContent, "file", fileName);

                // 5. Send request to upload endpoint
                var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();

                    // Ensure success flag is properly set
                    if (result != null)
                    {
                        result.Success = true;
                        return result;
                    }

                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = "Invalid server response"
                    };
                }
                else
                {
                    string errorMessage = "Upload failed";
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
                        errorMessage = errorResult?.Message ?? await response.Content.ReadAsStringAsync();
                    }
                    catch { }

                    return new ImageUploadResponse
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }
            }
            catch (FormatException)
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = "Invalid Base64 image data"
                };
            }
            catch (Exception ex)
            {
                return new ImageUploadResponse
                {
                    Success = false,
                    Message = $"Error uploading camera image: {ex.Message}"
                };
            }
        }

        public async Task<string> GetImageUrlAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            // Gunakan BaseApiUrl yang sudah didefinisikan
            return $"{BaseApiUrl}/uploads/{fileName}";
        }
    }
}
