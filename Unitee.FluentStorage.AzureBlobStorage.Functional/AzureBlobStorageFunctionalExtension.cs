using Azure.Storage.Blobs.Models;
using CSharpFunctionalExtensions;

namespace Unitee.FluentStorage.AzureBlobStorage.Functional;

public static class AzureBlobStorageFunctionalExtensions
{
    public static async Task<Result<(Uri, BlobContentInfo)>> FUploadAsync(this IAzureBlobStorageProvider storage, Stream s)
    {
        return await Result.Try(async () =>
        {
            var (uri, blob) = await storage.UploadAsync(s);

            if (blob.Value is null)
            {
                return Result.Failure<(Uri, BlobContentInfo)>("Upload failed");
            }

            if (uri is null)
            {
                return Result.Failure<(Uri, BlobContentInfo)>("Upload failed");
            }

            return Result.Success((uri, blob.Value));

        }).Bind(x => x);
    }
}
 
