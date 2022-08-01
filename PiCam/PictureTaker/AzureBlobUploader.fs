module AzureBlobUploader

    open System
    open System.IO
    open Azure.Storage.Blobs

    let private envVarOrFail varName =
        if String.IsNullOrWhiteSpace (Environment.GetEnvironmentVariable varName) then
            failwith $"Missing environment variable: '{varName}'"
        else
            Environment.GetEnvironmentVariable varName      

    let sasUri() = "SasUri" |> envVarOrFail |> Uri

    let azureBlobContainerName() = envVarOrFail "BlobContainerName"
        
    let uploadPicture (blobUri: Uri) azureBlobContainerName (stream: Stream) = 
        let blobServiceClient = BlobServiceClient(blobUri, null)
        let _blobContainerClient = blobServiceClient.GetBlobContainerClient azureBlobContainerName
        let n = DateTime.UtcNow
        let fileName = $"{n.Year}-{n.Month}-{n.Day} {n.Hour}:{n.Minute}:{n.Second}Z.jpg"
        _blobContainerClient.UploadBlob(fileName, stream)
        |> ignore // response