using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Unitee.FluentStorage.Abstraction;

namespace Unitee.FluentStorage.AzureBlobStorage;


public interface IAzureBlobStorageProvider :
    IFluentStorageProvider<Response<BlobContentInfo>, Response<BlobProperties>, Response<bool>, IAzureBlobStorageProvider>
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
    public string? FileName { get; set; }
    public BlobHttpHeaders Headers { get; set; }
    public bool CreateIfNotExist { get; set; }

}
public record AzureBlobStorageProvider : IAzureBlobStorageProvider
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
    public string? FileName { get; set; }
    public BlobHttpHeaders Headers { get; set; } = new BlobHttpHeaders();
    public bool CreateIfNotExist { get; set; } = false;

    private void AssertGuards()
    {
        ArgumentNullException.ThrowIfNull(ConnectionString, nameof(ConnectionString));
        ArgumentNullException.ThrowIfNull(ContainerName, nameof(ContainerName));
        ArgumentNullException.ThrowIfNull(FileName, nameof(FileName));
    }

    private BlobClient GetBlobClient()
    {
        AssertGuards();
        BlobServiceClient blobServiceClient = new(ConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

        if (CreateIfNotExist)
        {
            containerClient.CreateIfNotExists();
        }

        return containerClient.GetBlobClient(FileName);
    }

    public (Stream, Response<BlobProperties>) OpenRead()
    {
        var blobClient = GetBlobClient();
        return (blobClient.OpenRead(), blobClient.GetProperties());
    }

    public async Task<(Uri, Response<BlobContentInfo>)> UploadAsync(IFormFile f)
    {
        if (FileName is null && f.FileName is not null)
        {
            return await WithFileName(f.FileName).UploadAsync(f);
        }

        if (Headers.ContentType is null && f.ContentType is not null)
        {
            return await WithContentType(f.ContentType).UploadAsync(f);
        }

        AssertGuards();

        var blobClient = GetBlobClient();
        var res = await blobClient.UploadAsync(f.OpenReadStream(), Headers);
        return (blobClient.Uri, res);
    }

    public async Task<Response<bool>> DeleteIfExistsAsync()
    {
        AssertGuards();

        var blobClient = GetBlobClient();
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Response<bool>> DeleteIfExistsAsync(Uri url)
    {
        return await FromUrl(url).DeleteIfExistsAsync();
    }

    public async Task<(Uri, Response<BlobContentInfo>)> UploadAsync(Stream s)
    {
        AssertGuards();

        var blobClient = GetBlobClient();
        var res = await blobClient.UploadAsync(s, Headers);
        return (blobClient.Uri, res);
    }

    public async Task<(Uri, Response<BlobContentInfo>)> UploadAsync(string b64)
    {
        AssertGuards();

        var blobClient = GetBlobClient();
        var res = await blobClient.UploadAsync(new MemoryStream(Convert.FromBase64String(b64)), Headers);
        return (blobClient.Uri, res);
    }

    public async Task<(Uri, Response<BlobContentInfo>)> UploadAsync(byte[] b)
    {
        AssertGuards();

        var blobClient = GetBlobClient();
        var res = await blobClient.UploadAsync(new MemoryStream(b), Headers);
        return (blobClient.Uri, res);
    }

    public IAzureBlobStorageProvider WithConnectionString(string connectionString)
    {
        return this with { ConnectionString = connectionString };
    }

    public IAzureBlobStorageProvider WithContainerName(string containerName)
    {
        return this with { ContainerName = containerName };
    }

    public IAzureBlobStorageProvider WithCreateIfNotExist(bool createIfNotExist = true)
    {
        return this with { CreateIfNotExist = createIfNotExist };
    }

    public IAzureBlobStorageProvider WithFileName(string fileName)
    {
        return this with { FileName = fileName };
    }

    public IAzureBlobStorageProvider WithContentType(string contentType)
    {
        return this with
        {
            Headers = new BlobHttpHeaders()
            {
                ContentType = contentType,
                CacheControl = Headers.CacheControl,
                ContentDisposition = Headers.ContentDisposition,
                ContentEncoding = Headers.ContentEncoding,
                ContentLanguage = Headers.ContentLanguage,
            }
        };
    }

    public IAzureBlobStorageProvider FromUrl(Uri url)
    {
        var client = new BlobClient(url);

        return this with
        {
            ContainerName = client.BlobContainerName,
            FileName = client.Name,
        };
    }
}
