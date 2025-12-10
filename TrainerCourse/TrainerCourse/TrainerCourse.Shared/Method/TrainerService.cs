using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrainerCourse.Shared.Model;
using TrainerCourse.Shared.Services;

namespace TrainerCourse.Shared.Method
{
    public class TrainerService : ITrainerService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7285/trainers";

        public TrainerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Trainer> AddTrainerAsync(Trainer trainer)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, trainer);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Trainer>();
        }

        public async Task<Trainer> UpdateTrainerAsync(Trainer trainer)
        {
            var response = await _httpClient.PutAsJsonAsync(BaseUrl, trainer);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Trainer>();
        }

        public async Task DeleteTrainerAsync(int id)
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
                    throw new HttpRequestException($"Trainer with ID {id} not found");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(errorMessage);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error deleting trainer: {response.StatusCode} - {errorMessage}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting trainer: {ex.Message}", ex);
            }
        }

        public async Task<Trainer> GetTrainerByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Trainer>($"{BaseUrl}/{id}");
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Trainer>>(BaseUrl);
                return response ?? new List<Trainer>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching trainers: {ex.Message}");
                return new List<Trainer>();
            }
        }

        public async Task<List<Trainer>> SearchTrainersAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Trainer>>($"{BaseUrl}/search/{searchTerm}");
                return response ?? new List<Trainer>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching trainers: {ex.Message}");
                return new List<Trainer>();
            }
        }
    }
}
