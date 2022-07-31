open System
open System.Threading
open Unosquare.RaspberryIO
open Unosquare.WiringPi

let takePicture() = ()
let nextPictureIn() = TimeSpan.FromMinutes 15.
let uploadPicture() = ()

let doIt tp np up =
    let pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480)
    printfn "Picture is %i bytes" pictureBytes.Length
    ()

let rec loop() =
    doIt () () () 

    let timeSpan = nextPictureIn()
    printfn "Sleeping for %O" timeSpan
    Thread.Sleep timeSpan
    loop()

Pi.Init<BootstrapWiringPi>()
loop()
printfn "Done."
