namespace embedded

open System
open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open Giraffe.ViewEngine

module incursions =
    // Define a nullable container to deserialize into.
    [<AllowNullLiteral>]
    type NameContainer() =
        member val Name = "" with get, set

    // For convenience, it's better to have a central place for the literal.
    [<Literal>]
    let Name = "name"

    let htmlBody: string =
        html [] [
            table [ _style "border: 1px solid black;" ] [
                thead [] [
                    td [] [ str "Location" ]
                    td [] [ str "Value" ]
                    td [] [ str "Timestamp" ]
                ]
                tr [] [
                    td [] [ str "Main House" ]
                    td [] [ str "24C / 76F" ]
                    td [] [ str (DateTime.Now.ToString()) ]
                ] 
                tr [] [
                    td [] [ str "Casita" ]
                    td [] [ str "28C / 82F" ]
                    td [] [ str (DateTime.Now.ToString()) ] 
                ]
                tr [] [
                    td [] [ str "Outside" ]
                    td [] [ str "40C / 101F" ]
                    td [] [ str (DateTime.Now.ToString()) ]
                ]
            ]
        ] |> RenderView.AsString.htmlDocument

    [<FunctionName("incursion")>]
    let incursion ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>]req: HttpRequest) (log: ILogger): Task<IActionResult> =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")

            let nameOpt = 
                if req.Query.ContainsKey(Name) then
                    Some(req.Query.[Name].[0])
                else
                    None

            use stream = new StreamReader(req.Body)
            let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask

            let data = JsonConvert.DeserializeObject<NameContainer>(reqBody)

            
            //return OkObjectResult(htmlBody) :> IActionResult

            return ContentResult(Content=htmlBody, ContentType="text/html") :> IActionResult
        } |> Async.StartAsTask

    [<FunctionName("report")>]
    let report ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = null)>]req: HttpRequest) (log: ILogger): Task<IActionResult> =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")

            let nameOpt = 
                if req.Query.ContainsKey(Name) then
                    Some(req.Query.[Name].[0])
                else
                    None

            use stream = new StreamReader(req.Body)
            let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask

            let data = JsonConvert.DeserializeObject<NameContainer>(reqBody)

            
            //return OkObjectResult(htmlBody) :> IActionResult

            return ContentResult(Content=htmlBody, ContentType="text/html") :> IActionResult
        } |> Async.StartAsTask

    [<FunctionName("sensor")>]
    let sensor ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>]req: HttpRequest) (log: ILogger): Task<IActionResult> =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")

            let nameOpt = 
                if req.Query.ContainsKey(Name) then
                    Some(req.Query.[Name].[0])
                else
                    None

            use stream = new StreamReader(req.Body)
            let! reqBody = stream.ReadToEndAsync() |> Async.AwaitTask

            let data = JsonConvert.DeserializeObject<NameContainer>(reqBody)

            
            //return OkObjectResult(htmlBody) :> IActionResult

            return ContentResult(Content=htmlBody, ContentType="text/html") :> IActionResult
        } |> Async.StartAsTask