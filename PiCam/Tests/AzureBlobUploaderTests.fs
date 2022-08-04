module AzureBlobUploaderTests

open System
open System.IO
open Xunit

[<Fact>]
let ``Azure Storabe Blob Upload Integration Test`` (): unit =
    let uri = Uri ""
    let blobContainerName = "picam"
    
    "test-image.jpg"
    |> File.OpenRead
    //|> BinaryData.FromStream
    |> AzureBlobUploader.uploadPicture uri blobContainerName

