
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class EventApiService
    {
        private readonly HttpClient _httpClient;

        public EventApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var events = await _httpClient
                .GetFromJsonAsync<List<EventViewModel>>("api/Events");

            return events ?? new List<EventViewModel>();
        }
        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<EventViewModel>($"api/Events/{id}");
        }
    }
}
