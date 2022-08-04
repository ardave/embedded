module Types

    open System
    open Microsoft.Extensions.Logging

    type NextOperation =
    | ExitLoop // mainly used for testing.
    | TakeNextPictureIn of TimeSpan

    type MyLogger = {
        Info : (string * obj[]) -> unit
    }

    module MyLogger =
        let fromILogger (i: ILogger) = {
             Info = fun (msg, args) -> i.LogInformation(msg, args)
        }