module Types

    open System
    open Microsoft.Extensions.Logging

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