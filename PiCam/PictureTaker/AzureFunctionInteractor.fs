module AzureFunctionInteractor

    open System
    open Types    
    
    let nextPictureIn() = NextOperation.TakeNextPictureIn (TimeSpan.FromMinutes 15.)