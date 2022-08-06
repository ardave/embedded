module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open Types    

    let private queue = Queue<TimeSpan>()
    queue.Enqueue(TimeSpan.FromMinutes 0.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)
    
    let nextPictureIn() = 
        if queue.Count = 0 then
            NextOperation.ExitLoop
        else
            NextOperation.TakeNextPictureIn (queue.Dequeue())