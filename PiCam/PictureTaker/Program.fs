
let uploadPicture = AzureBlobUploader.uploadPicture (AzureBlobUploader.blobUri()) (AzureBlobUploader.azureBlobContainerName())

Orchestrator.loop PictureTaker.takePicture AzureFunctionInteractor.nextPictureIn uploadPicture

printfn "Exiting ..."
