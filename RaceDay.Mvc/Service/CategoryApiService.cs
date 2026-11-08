
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: Retrieve all categories
        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _httpClient
                .GetFromJsonAsync<List<CategoryViewModel>>("api/Categories");

            return categories ?? new List<CategoryViewModel>();
        }

        // GET: Retrieve one category
        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<CategoryViewModel>($"api/Categories/{id}");
        }

        // POST: Create a category
        public async Task<bool> CreateCategoryAsync(CategoryViewModel category)
        {
            var response = await _httpClient
                .PostAsJsonAsync("api/Categories", category);

            return response.IsSuccessStatusCode;
        }

        // PUT: Update a category
        public async Task<bool> UpdateCategoryAsync(
            int id, CategoryViewModel category)
        {
            var response = await _httpClient
                .PutAsJsonAsync($"api/Categories/{id}", category);

            return response.IsSuccessStatusCode;
        }

        // DELETE: Delete a category
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient
                .DeleteAsync($"api/Categories/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
