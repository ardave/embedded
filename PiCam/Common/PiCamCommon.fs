namespace PiCamCommon

open System
open System.Threading
open Microsoft.Extensions.Logging
open Newtonsoft.Json


type NextOperation =
| SleepForAWhile
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
        for _ in 1 .. int ts.TotalSeconds do
            if not ct.IsCancellationRequested then
                Thread.Sleep (TimeSpan.FromSeconds 1.)

    let envVarOrFail varName =
        if String.IsNullOrWhiteSpace (Environment.GetEnvironmentVariable varName) then
            failwith $"Missing environment variable: '{varName}'"
        else
            Environment.GetEnvironmentVariable varName   
              
