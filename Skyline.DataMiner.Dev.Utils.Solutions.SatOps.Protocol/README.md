# Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Protocol

A NuGet package providing models and helper classes for interacting with the DataMiner SatOps (Satellite Operations) solution from within connector QActions.

## Getting Started

> **Prerequisites:** A running DataMiner system with the [SatOps](https://catalog.dataminer.services/details/08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1) solution installed.

Obtain an `ISatOpsApi` from the `SLProtocol` instance inside a QAction and use it to read and manage satellites, beams, transponders, transponder plans, plan rows, slots and range reservations.

**Get the API and verify the solution is installed:**

```csharp
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Solutions.SatOps.Protocol.Extensions;

public static class QAction
{
    public static void Run(SLProtocol protocol)
    {
        try
        {
            var api = protocol.GetSatOpsApi();

            if (!api.IsInstalled(out var version))
            {
                protocol.Log("QA" + protocol.QActionID + "|Run|SatOps solution is not installed.", LogType.Error, LogLevel.NoLogging);
                return;
            }

            protocol.Log("QA" + protocol.QActionID + $"|Run|SatOps {version} detected.", LogType.Information, LogLevel.NoLogging);
        }
        catch (Exception ex)
        {
            protocol.Log("QA" + protocol.QActionID + "|Run|Exception thrown: " + ex, LogType.Error, LogLevel.NoLogging);
        }
    }
}
```

**Fill a table with transponders of a satellite:**

```csharp
var api = protocol.GetSatOpsApi();
var transponders = api.Transponders.ReadBySatellite(satelliteId).ToList();

var keys = transponders.Select(t => t.Id.ToString()).ToArray();
var names = transponders.Select(t => t.Name).Cast<object>().ToArray();
var bandwidths = transponders.Select(t => (object)(t.Bandwidth ?? 0d)).ToArray();

protocol.FillArray(Parameter.Transponders.tablePid, new List<object[]> { keys, names, bandwidths });
```

**Filter with exposers:**

Pushing the filter down to the API avoids retrieving entities the connector does not need.

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;

var kuBandTransponders = api.Transponders.Read(
    TransponderExposers.TransponderBand.Equal(TransponderBandType.Ku));
```

**Navigate the entity hierarchy:**

```csharp
var beams = api.Beams.Read();
var plans = api.TransponderPlans.ReadByTransponder(transponderId);
var slots = api.TransponderSlots.ReadByTransponderPlan(transponderPlanId);
```

**Look up range reservations in a time window:**

```csharp
var reservations = api.TransponderRangeReservations.ReadByTransponderAndTimeWindow(
    transponderId,
    DateTime.UtcNow,
    DateTime.UtcNow.AddDays(1));
```

The full API surface (repositories, domain models, exposers) is provided by the `Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Common` package, which this DevPack depends on.

## About

Contains the models and helper classes to interact with the SatOps solution from connector QActions.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
