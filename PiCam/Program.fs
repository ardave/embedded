open System
open System.Threading
open MMALSharp
open MMALSharp.Common
open MMALSharp.Common.Utility
open MMALSharp.Handlers

let takePicture() = ()
let nextPictureIn() = TimeSpan.FromSeconds 15.
let uploadPicture() = ()

MMALCameraConfig.StillResolution <- new Resolution(4056, 3040)
//MMALCameraConfig.Framerate <- new MMAL_RATIONAL_T(20, 1) // Set to 20fps. Default is 30fps.
MMALCameraConfig.ShutterSpeed <- 2000000 // Set to 2s exposure time. Default is 0 (auto).
MMALCameraConfig.ISO <- 400 // Set ISO to 400. Default is 0 (auto).

let cam: MMALCamera = MMALCamera.Instance;

let doIt tp np up =
    printfn "Gonna take a picture."
    use imgCaptureHandler = new ImageStreamCaptureHandler("/home/dave/Pictures", "jpg")

    cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420)
    |> Async.AwaitTask
    |> Async.RunSynchronously

    printfn "Took a picture."    

let rec loop() =
    doIt () () ()
    let timeSpan = nextPictureIn()
    printfn "Sleeping for %O" timeSpan
    Thread.Sleep timeSpan
    loop()

loop()
printfn "Done."
