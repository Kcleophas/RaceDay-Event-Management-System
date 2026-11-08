
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class OrganiserEventApiService
    {
        private readonly HttpClient _httpClient;

        public OrganiserEventApiService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RaceDayApi");
        }

        // Retrieve all events
        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var events = await _httpClient
                .GetFromJsonAsync<List<EventViewModel>>(
                    "api/Events");

            return events ?? new List<EventViewModel>();
        }

        // Create a new event
        public async Task<bool> CreateEventAsync(
            EventViewModel eventModel)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Events", eventModel);

            return response.IsSuccessStatusCode;
        }

        // Update an existing event
        public async Task<bool> UpdateEventAsync(
            int id,
            EventViewModel eventModel)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/Events/{id}", eventModel);

            return response.IsSuccessStatusCode;
        }

        // Delete an event
        public async Task<bool> DeleteEventAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/Events/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
