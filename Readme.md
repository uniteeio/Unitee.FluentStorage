# Fluent Storage

Abstraction around Storages with Azure Blob Storage in mind.

## Installation

```
dotnet add package Unitee.FluentStorage.AzureBlobStorage
```

## Usage

```csharp
// Setup
var storage = new AzureBlobStorageProvider()
    .WithConnectionString("xxxxxx")
    .WithContainerName("documents")
    .CreateIfNotExist();

// Add to DI
services.AddScoped<IAzureBlobStorageProvider, AzureBlobStorageProvider>(storage);


// Upload a stream
using var fileStream = new FileStream(@"/app/file.txt", FileMode.Open);

var (uri, response) = await storage
    .WithFileName("file.txt")
    .WithContentType("text/plain")
    .UploadAsync(filestream);

// Upload IFormFile
var formFile = req.File;

var (uri, response) = await storage
    .WithFileName("file.txt")
    .WithContentType("text/plain")
    .UploadAsync(formFile);


// Or (if you want to use filename and content-type from the IFormFile)
var (uri, response) = await storage
    .UploadAsync(formFile);

// Download
var (stream, blobProperties) = storage
    .WithFileName("file.txt")
    .OpenRead();

var contentType = blobProperties.ContentType;
return File(stream, contentType);

// Upload b64

var base64Str = "ABCDEF1234=="

var (uri, response) = await storage
    .WithFileName("file.txt")
    .WithContentType("text/plain")
    .UploadAsync(base64Str);

```