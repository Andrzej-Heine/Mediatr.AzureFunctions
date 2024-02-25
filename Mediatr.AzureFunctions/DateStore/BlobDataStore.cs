using Azure.Storage.Blobs;
using IsolatedMediatr.Functions;
using IsolatedMediatr.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IsolatedMediatr.DateStore
{
    public class BlobDataStore
    {
        private readonly ILogger<GetListBlobFunction> _logger;
        private readonly string? _blobContainerName;
        private readonly string? _connectionString;

        public BlobDataStore(ILogger<GetListBlobFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _blobContainerName = configuration.GetValue<string>("BlobContainerName");
            _connectionString = configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");
        }

        public async Task<List<BlobObject>> GetAllBlobs()
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);

                var filesList = new List<BlobObject>();
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    filesList.Add(new BlobObject(
                        GetPathWithoutFilename(blobItem.Name),
                        GetFileNameFromPath(blobItem.Name),
                        blobItem.Properties.ContentLength));
                }

                return filesList;
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation($"[GetJsonFilesList]: {ex}");
                throw;
            }
        }

        private static string GetFileNameFromPath(string path)
        {
            return path[(path.LastIndexOf('/') + 1)..];
        }

        private static string GetPathWithoutFilename(string path)
        {
            return path.Remove(path.LastIndexOf('/'));
        }
    }
}