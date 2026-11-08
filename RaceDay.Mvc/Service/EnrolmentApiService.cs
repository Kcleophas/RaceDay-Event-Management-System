
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class EnrolmentApiService
    {
        private readonly HttpClient _httpClient;

        public EnrolmentApiService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: Retrieve all enrolments with participant,
        // event and category information.
        public async Task<List<EnrolmentDetailsViewModel>>
            GetAllEnrolmentsAsync()
        {
            var enrolments =
                await _httpClient.GetFromJsonAsync<
                    List<EnrolmentDetailsViewModel>>(
                    "api/Enrolments");

            return enrolments ?? new List<EnrolmentDetailsViewModel>();
        }
    }
}
