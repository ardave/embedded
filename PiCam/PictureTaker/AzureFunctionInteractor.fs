module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open System.Net.Http
    open Newtonsoft.Json
    open PiCamCommon    

    let private client = new HttpClient()

    let mutable isFirst = None
   
    let nextPictureIn() =
        match isFirst with
        | Some _ -> 15.
        | None -> 
            isFirst <- Some()
            0.
        |> TimeSpan.FromMinutes
        |> NextOperation.TakeNextPictureIn    


    let nextPictureInHttp(): NextOperation =
        let response = client.GetAsync("uri") |> Async.AwaitTask |> Async.RunSynchronously
        let content: string = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously

        if response.IsSuccessStatusCode then
            failwith "not implemented"
            //content |> JsonConvert.DeserializeObject<NextPictureModel> |> NextOperation.TakeNextPictureIn
        else
            failwith $"Unsuccessful status code {response.StatusCode} getting next picture TimeSpan, with content: '{content}'."           
    