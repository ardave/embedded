module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open Types    

    let private queue = Queue<TimeSpan>()
    queue.Enqueue(TimeSpan.FromMinutes 0.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    queue.Enqueue(TimeSpan.FromMinutes 1.)
    
    let nextPictureIn() = 
        NextOperation.TakeNextPictureIn (TimeSpan.FromHours 1.)
        // if queue.Count = 0 then
        //     NextOperation.ExitLoop
        // else
        //     NextOperation.TakeNextPictureIn (queue.Dequeue())