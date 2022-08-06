module AzureBlobUploaderTests

open System
open System.IO
open Xunit
open Microsoft.Extensions.Logging
open Types

let myLogger: MyLogger = {
    Info = fun msg -> ()
}
    

[<Fact>]
let ``Azure Storabe Blob Upload Integration Test`` (): unit =
    let uri = Uri ""
    let blobContainerName = "picam"
    
    "test-image.jpg"
    |> File.OpenRead
    |> AzureBlobUploader.uploadPicture myLogger uri blobContainerName
