module AzureBlobUploader

    open System
    open System.IO
    open Azure.Storage.Blobs

    let private envVarOrFail varName =
        if String.IsNullOrWhiteSpace (Environment.GetEnvironmentVariable varName) then
            failwith $"Missing environment variable: '{varName}'"
        else
            Environment.GetEnvironmentVariable varName      

    let sasUri() = "SASURI" |> envVarOrFail |> Uri

    let azureBlobContainerName() = envVarOrFail "BLOBCONTAINERNAME"
        
    let uploadPicture (blobUri: Uri) azureBlobContainerName (pictureStream: Stream) = 
        printfn $"Uploading picture at {DateTime.Now}"
        let sw = System.Diagnostics.Stopwatch.StartNew()

        let blobServiceClient = BlobServiceClient(blobUri, null)
        let _blobContainerClient = blobServiceClient.GetBlobContainerClient azureBlobContainerName
        let n = DateTime.UtcNow
        let fileName = $"{n.Year}-{n.Month}-{n.Day} {n.Hour}:{n.Minute}:{n.Second}Z.jpg"
        pictureStream.Position <- 0
        _blobContainerClient.UploadBlob(fileName, pictureStream)
        |> ignore // response

        printfn $"Upload took {sw.ElapsedMilliseconds} ms."