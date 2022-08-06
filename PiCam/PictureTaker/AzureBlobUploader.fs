module AzureBlobUploader

    open System
    open System.Diagnostics
    open System.IO
    open Azure.Storage.Blobs
    open Microsoft.Extensions.Logging
    open Types

    let private envVarOrFail varName =
        if String.IsNullOrWhiteSpace (Environment.GetEnvironmentVariable varName) then
            failwith $"Missing environment variable: '{varName}'"
        else
            Environment.GetEnvironmentVariable varName      

    let sasUri() = "SASURI" |> envVarOrFail |> Uri

    let azureBlobContainerName() = envVarOrFail "BLOBCONTAINERNAME"

    let zeroPad i = if i < 10 then "0" + string i else string i

    let private bytesToStream ary =
        let stream = new MemoryStream()
        stream.Write(ary, 0, ary.Length)
        stream
        
    let uploadPicture (log: MyLogger) (blobUri: Uri) azureBlobContainerName (pictureStream: Stream) = 
        log.Info $"Uploading picture at {DateTime.Now}"
        let sw = Stopwatch.StartNew()
        let blobServiceClient = BlobServiceClient(blobUri, null)
        let _blobContainerClient = blobServiceClient.GetBlobContainerClient azureBlobContainerName
        let n = DateTime.UtcNow
        let fileName = $"{n.Year}-{zeroPad n.Month}-{zeroPad n.Day} {zeroPad n.Hour}:{zeroPad n.Minute}:{zeroPad n.Second}Z.jpg"
        pictureStream.Position <- 0
        _blobContainerClient.UploadBlob(fileName, pictureStream) 
        |> ignore // response
        log.Info $"Uploaded {fileName} in {sw.ElapsedMilliseconds} ms."
