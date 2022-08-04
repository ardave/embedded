module PictureTaker

    open System
    open System.Diagnostics
    open System.IO
    open MMALSharp
    open MMALSharp.Common
    open MMALSharp.Common.Utility
    open MMALSharp.Handlers
    open Microsoft.Extensions.Logging


    let private microsecondsPerSecond = 1000000.

    MMALCameraConfig.StillResolution <- new Resolution(4056, 3040)
    MMALCameraConfig.ShutterSpeed <- int (1. / 60. * microsecondsPerSecond)
    MMALCameraConfig.ISO <- 100
    
    let private cam: MMALCamera = MMALCamera.Instance;
    
    let streamToBytes (inputStream: Stream) : byte[] =
        use memoryStream = new MemoryStream()
        inputStream.CopyTo(memoryStream)
        memoryStream.ToArray()

    let takePicture (logger: ILogger) : byte[] =
        use msch = new MemoryStreamCaptureHandler()
        let sw = Stopwatch.StartNew()
    
        cam.TakePicture(msch, MMALEncoding.JPEG, MMALEncoding.I420)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        
        let bytes = streamToBytes msch.CurrentStream
        logger.LogInformation("Camera pictures is {picturesSize} bytes and took {milliseconds} ms.", bytes.Length, sw.ElapsedMilliseconds)
        bytes
