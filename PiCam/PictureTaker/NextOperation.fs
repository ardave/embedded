module Types

    open System

    type NextOperation =
    | ExitLoop // mainly used for testing.
    | TakeNextPictureIn of TimeSpan
