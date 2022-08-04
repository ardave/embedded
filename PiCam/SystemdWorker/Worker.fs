namespace SystemdWorker

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Types

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let myLogger = MyLogger.fromILogger logger
    let uploadPicture = AzureBlobUploader.uploadPicture myLogger (AzureBlobUploader.sasUri()) (AzureBlobUploader.azureBlobContainerName())

    override _.ExecuteAsync(ct: CancellationToken) =
        let mutable clientSaysToKeepGoing = true
        async {
            while not ct.IsCancellationRequested && clientSaysToKeepGoing do
                match AzureFunctionInteractor.nextPictureIn() with
                | NextOperation.ExitLoop -> clientSaysToKeepGoing <- false
                | NextOperation.TakeNextPictureIn timeSpan ->
                    do! Async.Sleep timeSpan
                    PictureTaker.takePicture logger
                    |> uploadPicture           
                
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
        }
        |> Async.StartAsTask
        :> Task // need to convert into the parameter-less task