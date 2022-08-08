namespace SystemdWorker

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open PiCamCommon

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let myLogger = MyLogger.fromILogger logger
    let uploadPicture = AzureBlobUploader.uploadPicture myLogger (AzureBlobUploader.sasUri()) (AzureBlobUploader.azureBlobContainerName())

    override _.ExecuteAsync(ct: CancellationToken) =
        let rec loop() = 
            if not ct.IsCancellationRequested then                
                match AzureFunctionInteractor.nextPictureIn() with
                | NextOperation.ExitLoop -> ()
                | NextOperation.TakeNextPictureIn timeSpan ->
                    logger.LogInformation $"Sleeping for {timeSpan}"
                    Misc.sleepInOneSecondIncrements ct timeSpan
                    PictureTaker.takePicture myLogger
                    |> uploadPicture
                    loop()
        logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
        loop()
        
        async { return () }
        |> Async.StartAsTask
        :> Task // need to convert into the parameter-less task