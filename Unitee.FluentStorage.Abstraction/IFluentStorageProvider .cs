using Microsoft.AspNetCore.Http;

namespace Unitee.FluentStorage.Abstraction;

public interface IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload>
{
    public IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload> WithConnectionString(string connectionString);
    public IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload> WithContainerName(string containerName);
    public IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload> WithCreateIfNotExist(bool createIfNotExist = true);
    public IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload> WithFileName(string fileName);
    public IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload> WithContentType(string contentType);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(IFormFile f);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(Stream s);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(string b64);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(byte[] b);
    public (Stream, TNativeResponseDownload) OpenRead();
}
