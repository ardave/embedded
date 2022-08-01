open System
open System.IO
open System.Threading
open MMALSharp
open MMALSharp.Common
open MMALSharp.Common.Utility
open MMALSharp.Handlers

let nextPictureIn() = TimeSpan.FromSeconds 15.
let uploadPicture (bytes: byte[]) = ()

let microsecondsPerSecond = 1000000.

MMALCameraConfig.StillResolution <- new Resolution(4056, 3040)
MMALCameraConfig.ShutterSpeed <- int (1. / 60. * microsecondsPerSecond)
MMALCameraConfig.ISO <- 100

let cam: MMALCamera = MMALCamera.Instance;

let streamToBytes (inputStream: Stream) : byte[] =
    use memoryStream = new MemoryStream()
    inputStream.CopyTo(memoryStream)
    memoryStream.ToArray()

let takePicture() : byte[] =
    use msch = new MemoryStreamCaptureHandler()
    
    cam.TakePicture(msch, MMALEncoding.JPEG, MMALEncoding.I420)
    |> Async.AwaitTask
    |> Async.RunSynchronously

    msch.CurrentStream
    |> streamToBytes


let rec loop tp np up =
    let nextPictureIn: TimeSpan = np()
    Thread.Sleep nextPictureIn

    tp() |> up   
        
    loop tp np up

loop takePicture nextPictureIn uploadPicture

printfn "Done."
