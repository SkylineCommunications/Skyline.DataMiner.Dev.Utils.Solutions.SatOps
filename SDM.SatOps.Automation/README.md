# Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Automation

A NuGet package providing models and helper classes for interacting with the DataMiner SatOps (Satellite Operations) solution from within DataMiner Automation scripts.

## Getting Started

> **Prerequisites:** A running DataMiner system with the [SatOps](https://catalog.dataminer.services/details/08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1) solution installed.

Obtain an `ISatOpsApi` from the `engine` object and use it to read and manage satellites, beams, transponders, transponder plans, plan rows, slots and range reservations.

**Get the API and verify the solution is installed:**

```csharp
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Solutions.SatOps.Automation.Extensions;

public class Script
{
    public void Run(IEngine engine)
    {
        var api = engine.GetSatOpsApi();

        if (!api.IsInstalled(out var version))
        {
            engine.ExitFail("The SatOps solution is not installed on this DMS.");
            return;
        }

        engine.GenerateInformation($"SatOps {version} detected.");
    }
}
```

**Read satellites and their transponders:**

```csharp
var api = engine.GetSatOpsApi();

foreach (var satellite in api.Satellites.Read())
{
    engine.GenerateInformation($"{satellite.Name} ({satellite.Orbit}) @ {satellite.LongitudeForGEODegrees}°");

    foreach (var transponder in api.Transponders.ReadBySatellite(satellite.Id))
        engine.GenerateInformation($"  {transponder.Name} — {transponder.Band}, {transponder.Bandwidth} MHz");
}
```

**Create a satellite:**

Every repository exposes `Initialize()` to obtain a new, correctly backed instance. Fill in the properties and pass it to `Create`.

```csharp
var satellite = api.Satellites.Initialize();
satellite.Name = "Astra 1N";
satellite.Abbreviation = "A1N";
satellite.Orbit = OrbitType.GEO;
satellite.LongitudeForGEODegrees = 28.2;
satellite.Hemisphere = HemisphereType.Eastern;

satellite = api.Satellites.Create(satellite);
api.Satellites.Activate(satellite);
```

**Filter with exposers:**

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;

var kuBandTransponders = api.Transponders.Read(
    TransponderExposers.TransponderBand.Equal(TransponderBandType.Ku));
```

**Generate slots for a transponder plan:**

Slot generation runs through the regular `Create` method. Pass the id of the transponder plan, or submit a slot
that only has its `TransponderPlan` filled in, and the API replaces it by the slots calculated from the plan
rows, removing the existing slots of the plan in the same call. Call `Create` again to regenerate them.

```csharp
var plan = api.TransponderPlans.ReadByTransponder(transponder.Id).First();
var createdSlots = api.TransponderSlots.Create(plan.Id);

engine.GenerateInformation($"Generated {createdSlots.Count} slot(s) for plan '{plan.Name}'.");
```

**Look up range reservations in a time window:**

```csharp
var reservations = api.TransponderRangeReservations.ReadByTransponderAndTimeWindow(
    transponder.Id,
    DateTime.UtcNow,
    DateTime.UtcNow.AddDays(7));
```

**Route API diagnostics to the script log (optional):**

```csharp
api.SetLogger(myLogger); // any Skyline.DataMiner.Solutions.SatOps.Common.Logging.ILogger implementation
```

The full API surface (repositories, domain models, exposers) is provided by the `Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Common` package, which this DevPack depends on.

## About

Contains the models and helper classes to interact with the SatOps solution from Automation scripts.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
