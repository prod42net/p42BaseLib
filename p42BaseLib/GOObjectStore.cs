using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using p42ReportingLib;
using p42ReportingLib.Models;

namespace p42ReportingAPI.Reports;

public class GOObjectStore : BaseStore
{
    P42Logger _logger = new();
    string _accessKey;
    string _secretKey;
    string _serviceUrl;
    string _bucketName;
    AmazonS3Client _client;
    string _extension = "json";


    public GOObjectStore(string accessKey, string secretKey, string serviceUrl, string bucketName)
    {
        _accessKey = accessKey;
        _secretKey = secretKey;
        _serviceUrl = serviceUrl;
        _bucketName = bucketName;
        createClient();
    }

    void createClient()
    {
        _client = new AmazonS3Client(_accessKey, _secretKey, new AmazonS3Config
        {
            ServiceURL = _serviceUrl
        });
        if (_client == null)
        {
            _logger.Info("GOObjectStore creation failed");
        }
        else
        {
            _logger.Info("GOObjectStore created");
        }
    }

    public override int TotalNumber(string? prefix = null)
    {
        if (_client == null) return 0;

        try
        {
            int total = 0;
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName, 
                Prefix = prefix,
                
            };

            ListObjectsV2Response response;
            do
            {
                response = _client.ListObjectsV2Async(request).GetAwaiter().GetResult();
                if (response?.S3Objects != null)
                {
                    total += response.S3Objects.Count;
                }

                request.ContinuationToken = response?.NextContinuationToken;
            } while ((bool)response.IsTruncated!);

            return total;
        }
        catch (Exception ex)
        {
            _logger.Info($"GOObjectStore.TotalNumber failed: {ex.Message}");
            return 0;
        }
    }

    public override async Task<List<ReportModel>> GetAll(string id = "", string? prefix = null)
{
    var results = new List<ReportModel>();

    try
    {
        if (_client == null) return results;

        // Build listing prefix (treating 'id' as a subfolder or key prefix under the optional 'prefix')
        string listPrefix;
        if (string.IsNullOrWhiteSpace(prefix))
        {
            listPrefix = string.IsNullOrWhiteSpace(id) ? string.Empty : id;
        }
        else
        {
            listPrefix = string.IsNullOrWhiteSpace(id)
                ? $"{prefix.TrimEnd('/')}/"
                : $"{prefix.TrimEnd('/')}/{id}";
        }

        var request = new Amazon.S3.Model.ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = listPrefix
        };

        Amazon.S3.Model.ListObjectsV2Response response;
        do
        {
            response = await _client.ListObjectsV2Async(request);

            if (response?.S3Objects != null)
            {
                foreach (var s3Obj in response.S3Objects)
                {
                    // Only process expected extension (e.g., .json)
                    if (!s3Obj.Key.EndsWith($".{_extension}", StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        using var getResp = await _client.GetObjectAsync(new Amazon.S3.Model.GetObjectRequest
                        {
                            BucketName = _bucketName,
                            Key = s3Obj.Key
                        });
                        var model = await System.Text.Json.JsonSerializer.DeserializeAsync<ReportModel>(getResp.ResponseStream);
                        if (model != null)
                            results.Add(model);
                    }
                    catch (Exception exObj)
                    {
                        _logger.Info($"GOObjectStore.GetAll skipped key '{s3Obj.Key}': {exObj.Message}");
                    }
                }
            }

            request.ContinuationToken = response?.NextContinuationToken;
        } while (response != null && (bool)response.IsTruncated);

        return results;
    }
    catch (Exception ex)
    {
        _logger.Info($"GOObjectStore.GetAll failed: {ex.Message}");
        return results;
    }
}
    public override async Task<ReportModel?> Get(string id,string? prefix = null)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = getPath(id,_extension,prefix)
        };
        using var getResponse = await _client.GetObjectAsync(request);
        using var reader = new StreamReader(getResponse.ResponseStream);
        //string content = reader.ReadToEnd();
        ReportModel? model = JsonSerializer.Deserialize<ReportModel>(reader.ReadToEnd());
        if (model == null) return null;
        return model;
    }

    public override async Task<ReportModel?> Add(ReportModel model,string? prefix = null)
    {
        if (model == null) return null;
        model.Id = Guid.NewGuid().ToString();
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        string fn = getPath(model.Id,_extension,prefix);
        PutObjectRequest request = new PutObjectRequest()
        {
            BucketName = _bucketName,
            Key = fn,
            ContentBody = JsonSerializer.Serialize(model, jsonOptions)
        };
        PutObjectResponse response = await _client.PutObjectAsync(request);
        if ((int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300)
        {
            return model;
        }

        return null;
    }

    public override bool Delete(string id,string? prefix = null)
    {
        if (string.IsNullOrWhiteSpace(id) || _client == null)
            return false;

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = getPath(id,_extension,prefix)
            };

            var response = _client.DeleteObjectAsync(request).GetAwaiter().GetResult();
            return (int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300;
        }
        catch (Exception ex)
        {
            _logger.Info($"GOObjectStore.Delete failed: {ex.Message}");
            return false;
        }
    }

    public override bool Update(string id, ReportModel model,string? prefix = null)
    {
        if (string.IsNullOrWhiteSpace(id) || model == null || _client == null)
            return false;

        try
        {
            // Ensure the object exists before updating (to mirror InMemoryObjectStore behavior)
            try
            {
                var metaRequest = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = getPath(id,_extension,prefix)
                };
                _client.GetObjectMetadataAsync(metaRequest).GetAwaiter().GetResult();
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            // Ensure the model's Id matches the key being updated
            model.Id = id;

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = getPath(id,_extension,prefix),
                ContentBody = JsonSerializer.Serialize(model, jsonOptions)
            };

            var response = _client.PutObjectAsync(putRequest).GetAwaiter().GetResult();
            return (int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300;
        }
        catch (Exception ex)
        {
            _logger.Info($"GOObjectStore.Update failed: {ex.Message}");
            return false;
        }
    }
}