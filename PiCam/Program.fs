open System
open System.Threading

let takePicture() = ()
let nextPictureIn() = TimeSpan.FromMinutes 15.
let uploadPicture() = ()

let doIt tp np up =
    ()

let rec loop() =
    let timeSpan = nextPictureIn()
    printfn "Sleeping for %O" timeSpan
    Thread.Sleep timeSpan
    loop()

loop()
printfn "Done."
