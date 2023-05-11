using Azure.Storage.Blobs.Models;
using CSharpFunctionalExtensions;

namespace Unitee.FluentStorage.AzureBlobStorage.Functional;

public static class AzureBlobStorageFunctionalExtensions
{

    private static Result AssertGuardsF(this IAzureBlobStorageProvider storage)
    {
        if (storage.ConnectionString is null)
        {
            return Result.Failure("ConnectionString is null");
        }

        if (storage.ContainerName is null)
        {
            return Result.Failure("ContainerName is null");
        }

        if (storage.FileName is null)
        {
            return Result.Failure("FileName is null");
        }

        return Result.Success();
    }


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

    public static async Task<Result<(Uri, BlobContentInfo)>> FUploadAsync(this IAzureBlobStorageProvider storage, string b64) => await storage
            .AssertGuardsF()
            .Bind(async () => await storage.FUploadAsync(new MemoryStream(Convert.FromBase64String(b64))));


    public static Result<(Stream, BlobProperties)> FOpenRead(this IAzureBlobStorageProvider storage)
    {
        return Result.Try(() =>
        {
            var (stream, blob) = storage.OpenRead();

            if (blob.Value is null)
            {
                return Result.Failure<(Stream, BlobProperties)>("OpenRead failed");
            }

            if (stream is null)
            {
                return Result.Failure<(Stream, BlobProperties)>("OpenRead failed");
            }

            return Result.Success((stream, blob.Value));

        }).Bind(x => x);
    }
}

