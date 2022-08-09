module Orchestrator

    open PiCamCommon

    open System
    open System.Threading

    let rec loop takePicture nextPictureIn uploadPicture =
        match nextPictureIn() with
        | NextOperation.SleepForAWhile -> Thread.Sleep (TimeSpan.FromMinutes 15.)
        | NextOperation.TakeNextPictureIn timeSpan ->
            Thread.Sleep timeSpan
            takePicture() |> uploadPicture           
            loop takePicture nextPictureIn uploadPicture