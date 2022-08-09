module TimeCalculationTests

open System
open Xunit
open PiCamCommon
open embedded

[<Fact>]
[<Trait("TestType", "Unit")>]
let ``Next picture should be in 15 minutes if OUTOFTOWN``() : unit =
    let getEnvVar v = 
        if v = "OUTOFTOWN" then "true" else null

    let actual = incursions.calculateNextPicture getEnvVar DateTime.UtcNow

    let expected = TakeNextPictureIn(TimeSpan.FromMinutes 15.)

    Assert.Equal(expected, actual)

[<Fact>]
[<Trait("TestType", "Unit")>]
let ``Should sleep for a while if camera config setting is disabled.``() : unit =
    let getEnvVar v = 
        if v = "DISABLECAMERA" then "true" else null

    let actual = incursions.calculateNextPicture getEnvVar DateTime.UtcNow

    let expected = SleepForAWhile

    Assert.Equal(expected, actual)

[<Fact>]
[<Trait("TestType", "Unit")>]
[<InlineData()>]
let ``Should sleep for 15 minutes until next picture if in the active time window.``() : unit =
    let getEnvVar _ = null

    let midnight = DateTime(2022, 8, 9, 0, 0, 0, DateTimeKind.Utc)

    for i in int(TimeSpan.Parse("11:45:00").TotalMinutes) .. int(TimeSpan.Parse("14:45:00").TotalMinutes) do
        let now = midnight.AddMinutes i
        let actual = incursions.calculateNextPicture getEnvVar now
        let expected = TakeNextPictureIn(TimeSpan.FromMinutes 15.)
        if expected <> actual then
            failwith $"Expected {expected} at {now} ({now.Kind}), but was {actual}."

[<Theory>]
[<Trait("TestType", "Unit")>]
[<InlineData("09:00:00", 2, 45)>] // 3 am mst
[<InlineData("16:00:00", 19, 45)>] // 10 am mst
[<InlineData("21:00:00", 14, 45)>] // 15:00 mst
[<InlineData("05:00:00", 6, 45)>] // 23:00 mst
let ``Should sleep until next active time window if currently outside of the active time window``(utcTime: string, expectedHours: int, expectedMinutes: int): unit =
    let timeSpan = TimeSpan.Parse utcTime
    let utcTime = DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, DateTimeKind.Utc)
    let getEnvVar _ = null
    let expectedTimeSpan = TimeSpan.FromHours(expectedHours).Add(TimeSpan.FromMinutes(expectedMinutes))
    let expectedOperation = TakeNextPictureIn expectedTimeSpan

    let actual = incursions.calculateNextPicture getEnvVar utcTime
    
    Assert.Equal(expectedOperation, actual)