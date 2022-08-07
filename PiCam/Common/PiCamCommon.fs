namespace PiCamCommon

open System
open Microsoft.Extensions.Logging

open Newtonsoft.Json

[<CLIMutable>]
type NextPictureModel = {
    NextPictureIn: TimeSpan option
}

module NextPictureModel =
    let serialize (model: NextPictureModel): string = JsonConvert.SerializeObject model
    let deserialize (json: string): NextPictureModel = JsonConvert.DeserializeObject<NextPictureModel> json

type NextOperation =
| ExitLoop // mainly used for testing.
| TakeNextPictureIn of TimeSpan

type MyLogger = {
    Info : string -> unit
}

module MyLogger =
    let fromILogger (i: ILogger) = {
            Info = i.LogInformation
    }
