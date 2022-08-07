namespace PiCamCommon

open System
open System.Threading
open Microsoft.Extensions.Logging
open Newtonsoft.Json

//[<CLIMutable>]
//type NextPictureModel = {
//    NextPictureIn: TimeSpan option
//}

//module NextPictureModel =
//    let serialize (model: NextPictureModel): string = JsonConvert.SerializeObject model
//    let deserialize (json: string): NextPictureModel = JsonConvert.DeserializeObject<NextPictureModel> json

type NextOperation =
| ExitLoop // mainly used for testing.
| TakeNextPictureIn of TimeSpan

module NextOperation =
    let serialize (model: NextOperation): string = JsonConvert.SerializeObject model
    let deserialize (json: string): NextOperation = JsonConvert.DeserializeObject<NextOperation> json

type MyLogger = {
    Info : string -> unit
}

module MyLogger =
    let fromILogger (i: ILogger) = {
            Info = i.LogInformation
    }

module Misc =
    // Stackoverflow on "Sleeping with a cancellation token" seemed fraught,
    // so I'm doing this my way
    let sleepInOneSecondIncrements (ct: CancellationToken) (ts: TimeSpan) =
        for _ in 1 .. ts.Seconds do
            if not ct.IsCancellationRequested then
                Thread.Sleep (TimeSpan.FromSeconds 1.)
