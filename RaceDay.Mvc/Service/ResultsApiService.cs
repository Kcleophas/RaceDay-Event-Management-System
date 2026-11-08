
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class ResultsApiService
    {
        private readonly HttpClient _httpClient;

        public ResultsApiService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: Retrieve detailed race results
        public async Task<List<ResultDetailsViewModel>>
            GetResultsAsync()
        {
            var results =
                await _httpClient.GetFromJsonAsync<
                    List<ResultDetailsViewModel>>(
                    "api/Results/details");

            return results ?? new List<ResultDetailsViewModel>();
        }

        // GET: Retrieve all participant enrolments
        public async Task<List<EnrolmentDetailsViewModel>>
            GetEnrolmentsAsync()
        {
            var enrolments =
                await _httpClient.GetFromJsonAsync<
                    List<EnrolmentDetailsViewModel>>(
                    "api/Enrolments");

            return enrolments ?? new List<EnrolmentDetailsViewModel>();
        }

        // POST: Create a new race result
        public async Task<bool> CreateResultAsync(
            ResultCreateViewModel model)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Results",
                    model);

            return response.IsSuccessStatusCode;
        }
    }
}
