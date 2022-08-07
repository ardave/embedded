module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open System.Net.Http
    open PiCamCommon    

    let private client = new HttpClient()

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

    let nextPictureInHttp() =
        let response = client.GetAsync("uri") |> Async.AwaitTask |> Async.RunSynchronously
        ()