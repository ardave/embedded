module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open System.Net.Http
    open Newtonsoft.Json
    open PiCamCommon    

    let private client = new HttpClient()

    let private queue = Queue<TimeSpan>()
    queue.Enqueue(TimeSpan.FromMinutes 0.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)
    queue.Enqueue(TimeSpan.FromMinutes 15.)

    let getNext =
        seq { yield NextOperation.TakeNextPictureIn (TimeSpan.FromSeconds 0.)
              while true do
                yield NextOperation.TakeNextPictureIn (TimeSpan.FromMinutes 15.) }

    
    let nextPictureIn(): NextOperation = getNext |> Seq.head

    let nextPictureInHttp(): NextOperation =
        let response = client.GetAsync("uri") |> Async.AwaitTask |> Async.RunSynchronously
        let content: string = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously

        if response.IsSuccessStatusCode then
            failwith "not implemented"
            //content |> JsonConvert.DeserializeObject<NextPictureModel> |> NextOperation.TakeNextPictureIn
        else
            failwith $"Unsuccessful status code {response.StatusCode} getting next picture TimeSpan, with content: '{content}'."           
    