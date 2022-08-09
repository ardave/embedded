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
let ``Should sleep for 15 minutes if in the active window.``() : unit =
    let getEnvVar _ = null

    let midnight = DateTime(2022, 8, 9, 0, 0, 0, DateTimeKind.Utc)

    for i in int(TimeSpan.Parse("11:45:00").TotalMinutes) .. int(TimeSpan.Parse("14:45:00").TotalMinutes) do
        let now = midnight.AddMinutes i
        let actual = incursions.calculateNextPicture getEnvVar now
        let expected = TakeNextPictureIn(TimeSpan.FromMinutes 15.)
        if expected <> actual then
            failwith $"Expected {expected} at {now} ({now.Kind}), but was {actual}."
