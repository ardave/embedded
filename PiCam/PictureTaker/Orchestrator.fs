module Orchestrator

    open Types

    open System
    open System.Threading

    let rec loop takePicture nextPictureIn uploadPicture =
        match nextPictureIn() with
        | NextOperation.ExitLoop -> ()
        | NextOperation.TakeNextPictureIn timeSpan ->
            Thread.Sleep timeSpan
            takePicture() |> uploadPicture           
            loop takePicture nextPictureIn uploadPicture