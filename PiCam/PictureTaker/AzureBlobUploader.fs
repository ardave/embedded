module AzureBlobUploader

    open System
    open System.IO
    open Azure.Storage.Blobs
    open Azure.Storage.Blobs.Models
    open Azure.Storage.Blobs.Specialized

    let sasToken = Environment.GetEnvironmentVariable "SasToken"
    let azureStorageAccountName = Environment.GetEnvironmentVariable "AzureStorageAccountName"
    let azureBlobContainerName = Environment.GetEnvironmentVariable "AzureBlobContainerName"
    let blobUri = Uri $"https://{azureStorageAccountName}.blob.core.windows.net?{sasToken}"

    let uploadPicture (stream: Stream) = 
        let blobServiceClient = BlobServiceClient(blobUri, null)
        let _blobContainerClient = blobServiceClient.GetBlobContainerClient azureBlobContainerName
        let n = DateTime.UtcNow
        let fileName = $"{n.Year}-{n.Month}-{n.Day} {n.Hour}:{n.Minute}:{n.Second}Z.jpg"
        _blobContainerClient.UploadBlob("filename.jpg", stream)
        |> ignore // response