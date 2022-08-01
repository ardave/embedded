module AzureBlobUploaderTests

open System
open System.IO
open Xunit

[<Fact>]
let ``Azure Storabe Blob Upload Integration Test`` () =
    let fileStream = File.OpenRead "test-image.jpg"
    
    AzureBlobUploader.uploadPicture fileStream

