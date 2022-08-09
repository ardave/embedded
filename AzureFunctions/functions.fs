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
open PiCamCommon

module incursions =
    // Define a nullable container to deserialize into.
    [<AllowNullLiteral>]
    type NameContainer() =
        member val Name = "" with get, set

    let calculateNextPicture (getEnvironmentVariable: string->string) (utcNow: DateTime): NextOperation =
        if utcNow.Kind <> DateTimeKind.Utc then
            failwith $"DateTime.Kind must be Utc, and not {utcNow.Kind}"

        if Convert.ToBoolean(getEnvironmentVariable "DISABLECAMERA") then
            SleepForAWhile
        else if Convert.ToBoolean(getEnvironmentVariable "OUTOFTOWN") then
            TakeNextPictureIn (TimeSpan.FromMinutes 15.)
        else
            let mstZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")
            let mstTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, mstZone)

            if mstTime.TimeOfDay < TimeSpan.Parse("05:45:00") then
                let x = mstTime
                let nextStart = DateTime(x.Year, x.Month, x.Day, 5, 45, x.Second, x.Millisecond)
                let sleepForTimeSpan = nextStart - mstTime
                TakeNextPictureIn sleepForTimeSpan
            else if mstTime.TimeOfDay > TimeSpan.Parse("08:45:00") then
                let x = mstTime.AddDays 1.
                let nextStart = DateTime(x.Year, x.Month, x.Day, 5, 45, x.Second, x.Millisecond)
                let sleepForTimeSpan = nextStart - mstTime
                        
                TakeNextPictureIn sleepForTimeSpan
            else
                TakeNextPictureIn (TimeSpan.FromMinutes 15.)


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

            return ContentResult(Content=htmlBody, ContentType="text/html") :> IActionResult
        } |> Async.StartAsTask

    [<FunctionName("nextpicture")>]
    let nextPicture ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = null)>]req: HttpRequest) (log: ILogger): Task<IActionResult> =
        async {
            let envVars = Environment.GetEnvironmentVariables()
            
            let model =
                if Convert.ToBoolean(Environment.GetEnvironmentVariable "OUTOFTOWN") then
                    NextOperation.TakeNextPictureIn (TimeSpan.FromMinutes 15.)
                else
                    let mstZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time")
                    let mstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, mstZone)
                    if mstTime.Hour >= 6 && mstTime.Hour <= 9 then
                        NextOperation.TakeNextPictureIn (TimeSpan.FromMinutes 15.)
                    else
                        let x = mstTime.AddDays 1.
                        let nextStart = DateTime(x.Year, x.Month, x.Day, 6, x.Minute, x.Second, x.Millisecond)
                        let sleepForTimeSpan = nextStart - mstTime
                        
                        NextOperation.TakeNextPictureIn sleepForTimeSpan

            
            let json = model |> NextOperation.serialize
          
            return ContentResult(Content=htmlBody, ContentType="application/json") :> IActionResult
        } |> Async.StartAsTask

    [<FunctionName("latestpicture")>]
    let latestPicture ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>]req: HttpRequest) (log: ILogger): Task<IActionResult> =
        async {
                let addMeHtml: string =
                    html [] [
                        body[][
                            p[][str "add me"]
                        ]
                    ] |> RenderView.AsString.htmlDocument

            
            return ContentResult(Content=addMeHtml, ContentType="text/html") :> IActionResult
        } |> Async.StartAsTask
