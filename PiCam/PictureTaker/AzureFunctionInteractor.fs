module AzureFunctionInteractor

    open System
    open System.Collections.Generic
    open System.Net.Http
    open Newtonsoft.Json
    open PiCamCommon    

    let private client = new HttpClient()

    let mutable isFirst = None

    let wait a = a |> Async.AwaitTask |> Async.RunSynchronously
   
    let nextPictureIn() =
        match isFirst with
        | Some _ -> 60.
        | None -> 
            isFirst <- Some()
            0.
        |> TimeSpan.FromMinutes
        |> NextOperation.TakeNextPictureIn    


    let nextPictureInHttp (uri: Uri): NextOperation =
        let response = client.GetAsync uri |> wait
        let content: string = response.Content.ReadAsStringAsync() |> wait

        if response.IsSuccessStatusCode then
            content |> JsonConvert.DeserializeObject<NextOperation>
        else
            failwith $"Unsuccessful status code {response.StatusCode} getting next picture TimeSpan, with content: '{content}'."           
    