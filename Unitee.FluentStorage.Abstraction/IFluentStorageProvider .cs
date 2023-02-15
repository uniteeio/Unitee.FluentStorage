using Microsoft.AspNetCore.Http;

namespace Unitee.FluentStorage.Abstraction;

public interface IFluentStorageProvider<TNativeResponseUpload, TNativeResponseDownload, TSelf>
{
    public TSelf FromUrl(Uri url);
    public TSelf WithConnectionString(string connectionString);
    public TSelf WithContainerName(string containerName);
    public TSelf WithCreateIfNotExist(bool createIfNotExist = true);
    public TSelf WithFileName(string fileName);
    public TSelf WithContentType(string contentType);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(IFormFile f);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(Stream s);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(string b64);
    public Task<(Uri, TNativeResponseUpload)> UploadAsync(byte[] b);
    public (Stream, TNativeResponseDownload) OpenRead();
}
