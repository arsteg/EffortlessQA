using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace EffortlessQA.Api.Services.Implementation
{
    public class AzureBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _containerUrl;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerUrl =
                $"https://{blobServiceClient.AccountName}.blob.core.windows.net/{containerName}";
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadImageAsync(
            Stream fileStream,
            string fileName,
            string entityId,
            string fieldName
        )
        {
            // Validate file
            if (fileStream.Length > 5 * 1024 * 1024) // 5MB limit
                throw new Exception("Image size exceeds 5MB.");
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                throw new Exception("Invalid image format.");

            // Generate unique file name: {entityId}/{fieldName}/{guid}_{originalName}
            var blobName = $"{entityId}/{fieldName}/{Guid.NewGuid()}_{fileName}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Upload file
            await blobClient.UploadAsync(
                fileStream,
                new BlobHttpHeaders { ContentType = GetContentType(fileName) }
            );

            // Return SAS URL for secure access
            return await GenerateSasUrlAsync(blobName);
        }

        public async Task<List<string>> ExtractAndSecureImageUrlsAsync(
            string html,
            string entityId,
            string fieldName
        )
        {
            var regex = new Regex(@"src=""(https://[^\s""]+?)""", RegexOptions.IgnoreCase);
            var matches = regex.Matches(html);
            var validUrls = new List<string>();

            foreach (Match match in matches)
            {
                var url = match.Groups[1].Value;
                if (url.StartsWith(_containerUrl) && url.Contains($"{entityId}/{fieldName}/"))
                {
                    var blobName = url.Replace(_containerUrl + "/", "");
                    var sasUrl = await GenerateSasUrlAsync(blobName);
                    html = html.Replace(url, sasUrl);
                    validUrls.Add(url);
                }
            }

            return validUrls;
        }

        public async Task DeleteUnusedImagesAsync(string html, string entityId, string fieldName)
        {
            var regex = new Regex(@"src=""(https://[^\s""]+?)""", RegexOptions.IgnoreCase);
            var usedUrls = new HashSet<string>(
                regex.Matches(html).Cast<Match>().Select(m => m.Groups[1].Value)
            );
            var blobPrefix = $"{entityId}/{fieldName}/";
            var blobs = _containerClient.GetBlobsAsync(prefix: blobPrefix);

            await foreach (var blob in blobs)
            {
                var blobUrl = $"{_containerUrl}/{blob.Name}";
                if (!usedUrls.Contains(blobUrl))
                {
                    var blobClient = _containerClient.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync();
                }
            }
        }

        public async Task DeleteAllImagesForEntityAsync(string entityId, string fieldName)
        {
            var blobPrefix = $"{entityId}/{fieldName}/";
            var blobs = _containerClient.GetBlobsAsync(prefix: blobPrefix);

            await foreach (var blob in blobs)
            {
                var blobClient = _containerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        private async Task<string> GenerateSasUrlAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                //Permissions = BlobSasPermissions.Read
            };
			sasBuilder.SetPermissions(BlobSasPermissions.Read); // Use SetPermissions for Write permission
			var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
