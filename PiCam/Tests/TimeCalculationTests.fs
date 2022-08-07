module TimeCalculationTests

open System
open Xunit
open embedded

[<Fact>]
let ``Next picture should be in 15 minutes if OUTOFTOWN``() : unit =
    let getEnvVar _ = "true"

    let sleepFor = incursions.calculateNextPicture getEnvVar DateTime.UtcNow

    Assert.Equal(TimeSpan.FromMinutes 15., sleepFor)
