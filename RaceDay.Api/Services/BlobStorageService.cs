
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            string containerName = "profile-pictures";

            var blobServiceClient =
                new BlobServiceClient(connectionString);

            _containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);
        }

        // Upload a profile picture to private Azure Blob Storage
        public async Task<string> UploadProfilePictureAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            int userId)
        {
            string extension = Path.GetExtension(fileName);

            string blobName =
                $"{userId}/{Guid.NewGuid()}{extension}";

            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            return blobName;
        }

        // Retrieve a profile picture from private Azure Blob Storage
        public async Task<(Stream? Content, string? ContentType)>
            GetProfilePictureAsync(string blobName)
        {
            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return (null, null);
            }

            var response = await blobClient.DownloadStreamingAsync();

            return (
                response.Value.Content,
                response.Value.Details.ContentType
            );
        }

        // Delete a profile picture from Azure Blob Storage
        public async Task DeleteProfilePictureAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                return;
            }

            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
