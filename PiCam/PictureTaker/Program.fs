open Types

let myLogger: MyLogger = {
    Info = printfn "%s"
}

let uploadPicture = AzureBlobUploader.uploadPicture myLogger (AzureBlobUploader.sasUri()) (AzureBlobUploader.azureBlobContainerName())

let takePicture() = PictureTaker.takePicture myLogger

Orchestrator.loop takePicture AzureFunctionInteractor.nextPictureIn uploadPicture

printfn "Deprecated ..."
