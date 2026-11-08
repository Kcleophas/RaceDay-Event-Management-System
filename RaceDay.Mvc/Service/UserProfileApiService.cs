using System.Net.Http.Headers;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class UserProfileApiService
    {
        private readonly HttpClient _httpClient;

        public UserProfileApiService(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RaceDayApi");
        }

        // Retrieve a participant's profile
        public async Task<UserProfileViewModel?> GetProfileAsync(
            int userId,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Users/profile/{userId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<UserProfileViewModel>();
        }

        // Update a participant's profile
        public async Task<bool> UpdateProfileAsync(
            int userId,
            UserProfileViewModel model,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/Users/profile/{userId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = JsonContent.Create(model);

            using var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        // Upload a participant's profile picture
        public async Task<bool> UploadProfilePictureAsync(
            int userId,
            IFormFile file,
            string accessToken)
        {
            using var content = new MultipartFormDataContent();

            using var fileStream = file.OpenReadStream();

            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(
                streamContent,
                "file",
                file.FileName);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"api/ProfilePictures/upload/{userId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = content;

            using var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}