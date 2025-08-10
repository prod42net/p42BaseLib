using System.Text.Json;
//using Azure.Storage.Blobs;
using p42BaseLib.Interfaces;

namespace p42BaseLib;

public class ObjectStore
{
    // readonly BlobContainerClient _containerClient;
    // readonly IP42Logger _logger;
    // readonly string _containerName;
    // readonly SemaphoreSlim _initLock = new(1, 1);
    //
    //
    // public ObjectStore(string connectionString, string containerName)
    // {
    //     _logger = new P42Logger();
    //     _containerName = containerName;
    //     _containerClient = new BlobContainerClient(connectionString, containerName);
    //     _containerClient.CreateIfNotExists();
    //     _logger.Log($"ObjectStore [{_containerName}] created ");
    // }
    //
    // public async Task<bool> ObjectExistsAsync(string objectName)
    // {
    //     try
    //     {
    //         if (string.IsNullOrWhiteSpace(objectName))
    //         {
    //             _logger.Error("Object name cannot be null or empty.");
    //             return false;
    //         }
    //
    //         BlobClient client = _containerClient.GetBlobClient(objectName);
    //         return await client.ExistsAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.Error(
    //             $"Error while checking if object [{objectName}] exists in container [{_containerName}] Exception [{e}]");
    //         return false;
    //     }
    // }
    //
    // public async Task<T> LoadObjectAsync<T>(string objectName, bool createIfNotExists) where T : new()
    // {
    //     try
    //     {
    //         BlobClient client = _containerClient.GetBlobClient(objectName);
    //         if (!await client.ExistsAsync() && createIfNotExists)
    //         {
    //             T obj = new T();
    //             await SaveObjectAsync(objectName, obj);
    //             return obj;
    //         }
    //         else
    //         {
    //             await using var stream = await client.OpenReadAsync();
    //             T obj = await JsonSerializer.DeserializeAsync<T>(stream) ?? new T();
    //             return obj;
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.Error(
    //             $"Error while loading Object [{objectName}] from container [{_containerName}] Exception [{e}]");
    //     }
    //
    //     return new T();
    // }
    //
    //
    // public async Task SaveObjectAsync<T>(string objectName, T obj, bool overWrite = true, bool prettyPrint = false)
    // {
    //     try
    //     {
    //         BlobClient client = _containerClient.GetBlobClient(objectName);
    //         await using var stream = new MemoryStream();
    //         JsonSerializerOptions opt = JsonSerializerOptions.Default;
    //         if (prettyPrint)
    //         {
    //             opt.WriteIndented = true;
    //         }
    //         await JsonSerializer.SerializeAsync(stream, obj);
    //         stream.Position = 0;
    //         await client.UploadAsync(stream,overWrite);
    //         _logger.Debug($"Object [{objectName}] saved to store [{_containerName}]");
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.Error("Error saving object [{objectName}] to store: {e.Message}");
    //     }
    // }
}