# Skyline.DataMiner.Dev.Utils.Solutions.SatOps.GQI

A NuGet package providing models and helper classes for interacting with the DataMiner SatOps (Satellite Operations) solution from within custom GQI data sources.

## Getting Started

> **Prerequisites:** A running DataMiner system with the [SatOps](https://catalog.dataminer.services/details/08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1) solution installed.

Obtain an `ISatOpsApi` from the `GQIDMS` instance handed to your data source in `OnInit`, then use it to feed satellite-operations data into low-code apps and dashboards.

**A minimal ad hoc data source listing satellites:**

```csharp
using Skyline.DataMiner.Analytics.GenericInterface;
using Skyline.DataMiner.Solutions.SatOps.Common.API;
using Skyline.DataMiner.Solutions.SatOps.GQI.Extensions;

[GQIMetaData(Name = "SatOps Satellites")]
public sealed class SatellitesDataSource : IGQIDataSource, IGQIOnInit
{
    private ISatOpsApi api;

    public OnInitOutputArgs OnInit(OnInitInputArgs args)
    {
        api = args.DMS.GetSatOpsApi();
        return default;
    }

    public GQIColumn[] GetColumns() => new GQIColumn[]
    {
        new GQIStringColumn("Name"),
        new GQIStringColumn("Operator"),
        new GQIDoubleColumn("Longitude (°)"),
    };

    public GQIPage GetNextPage(GetNextPageInputArgs args)
    {
        var rows = api.Satellites.Read()
            .Select(satellite => new GQIRow(new[]
            {
                new GQICell { Value = satellite.Name },
                new GQICell { Value = satellite.Operator },
                new GQICell { Value = satellite.LongitudeForGEODegrees ?? 0d },
            }))
            .ToArray();

        return new GQIPage(rows) { HasNextPage = false };
    }
}
```

**Guard against a missing solution:**

```csharp
public OnInitOutputArgs OnInit(OnInitInputArgs args)
{
    api = args.DMS.GetSatOpsApi();

    if (!api.IsInstalled())
        throw new GenIfException("The SatOps solution is not installed on this DMS.");

    return default;
}
```

**Filter server-side with exposers:**

Pushing the filter down to the API avoids materializing entities you do not need.

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;

var kuBandTransponders = api.Transponders.Read(
    TransponderExposers.TransponderBand.Equal(TransponderBandType.Ku));
```

**Read child entities for a selected parent (e.g. via a GQI argument):**

```csharp
var transponders = api.Transponders.ReadBySatellite(satelliteId);
var plans = api.TransponderPlans.ReadByTransponder(transponderId);
var slots = api.TransponderSlots.ReadByTransponderPlan(transponderPlanId);
```

**Page large result sets:**

```csharp
foreach (var page in api.Transponders.ReadPaged(pageSize: 500))
{
    // map page to GQI rows
}
```

The full API surface (repositories, domain models, exposers) is provided by the `Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Common` package, which this DevPack depends on.

## About

Contains the models and helper classes to interact with the SatOps solution from custom GQI data sources.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
