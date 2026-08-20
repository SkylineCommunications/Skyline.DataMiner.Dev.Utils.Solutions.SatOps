# Skyline.DataMiner.Dev.Utils.Solutions.SatOps.Common

A NuGet package providing the models, repositories and helper classes for interacting with the DataMiner SatOps (Satellite Operations) solution.

## Getting Started

> **Prerequisites:** A running DataMiner system with the [SatOps](https://catalog.dataminer.services/details/08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1) solution installed.

`SatOpsApi` is the single entry point of this library. Give it a DataMiner `IConnection` — or use the `GetSatOpsApi()` extension method — and it exposes one repository per satellite-operations entity (`Satellites`, `Beams`, `Transponders`, `TransponderPlans`, `TransponderPlanRows`, `TransponderRangeReservations`, `TransponderSlots`), each created lazily on first use.

**Create the API and verify the solution is installed:**

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API;
using Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions;

var api = connection.GetSatOpsApi(); // equivalent to new SatOpsApi(connection)

if (!api.IsInstalled(out var version))
{
    throw new InvalidOperationException("The SatOps solution is not installed on this DMS.");
}
```

**Read entities:**

```csharp
var allSatellites = api.Satellites.Read();
var satellite = api.Satellites.Read(satelliteId);
var count = api.Transponders.Count();
```

**Filter with exposers:**

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;

var kuBandTransponders = api.Transponders.Read(
    TransponderExposers.TransponderBand.Equal(TransponderBandType.Ku));

var wideSlots = api.TransponderSlots.Read(
    TransponderSlotExposers.Bandwidth.GreaterThan(36.0));
```

Every exposer class also exposes the entity's own id (`SatelliteId`, `BeamId`, `TransponderId`, `PlanId`,
`PlanRowId`, `SlotId` and `ReservationId`), which is the only way to read a single transponder slot
(`ITransponderSlotRepository` has no `Read(Guid)`).

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot;

var slot = api.TransponderSlots
    .Read(TransponderSlotExposers.SlotId.Equal(slotId))
    .FirstOrDefault();
```

**Create an entity:**

Every repository exposes `Initialize()` to obtain a new, correctly backed instance. Fill in the properties and pass it to `Create`.

```csharp
var satellite = api.Satellites.Initialize();
satellite.Name = "Astra 1N";
satellite.Abbreviation = "A1N";
satellite.Orbit = OrbitType.GEO;
satellite.LongitudeForGEODegrees = 28.2;
satellite.Hemisphere = HemisphereType.Eastern;

satellite = api.Satellites.Create(satellite);
```

**Update, activate/deprecate and delete:**

```csharp
satellite.Operator = "SES";
api.Satellites.Update(satellite);

api.Satellites.Activate(satellite);    // Draft   -> Active
api.Satellites.Deprecate(satellite);   // Active  -> Deprecated
api.Satellites.Reactivate(satellite);  // Deprecated -> Active

api.Satellites.Delete(satellite.Id);
```

**Navigate the entity hierarchy:**

A `Satellite` owns `Beam`s and `Transponder`s, a `Transponder` owns `TransponderPlan`s, and a `TransponderPlan` owns `TransponderPlanRow`s and `TransponderSlot`s.

```csharp
var transponders = api.Transponders.ReadBySatellite(satellite.Id);
var plans = api.TransponderPlans.ReadByTransponder(transponder.Id);
var slots = api.TransponderSlots.ReadByTransponderPlan(plan.Id);
```

**Generate slots from a transponder plan (there is no `Update` for slots):**

Slots are never edited individually: they are fully derived from the plan rows and the transponder of the plan,
so any change to those inputs simply means the slots have to be recalculated. Because of that,
`ITransponderSlotRepository` intentionally has no update method — it only exposes create, delete, read and count —
and every write goes through `Create`.

Pass the id of the transponder plan, or submit a slot that only has its `TransponderPlan` filled in, and the API
replaces it by the slots calculated from the plan rows. The slots that already exist for the plan are removed as
part of the same call, and the calculated slots pass the same validation as manually created slots. Regenerating
the slots of a plan is therefore simply calling `Create` again.

```csharp
var createdSlots = api.TransponderSlots.Create(plan.Id);

// Equivalent, using a slot that only has its transponder plan filled in:
var sameSlots = api.TransponderSlots.Create(new TransponderSlot { TransponderPlan = plan.Id });
```

**Work with range reservations:**

```csharp
var reservations = api.TransponderRangeReservations.ReadByTransponderAndTimeWindow(
    transponder.Id,
    DateTime.UtcNow,
    DateTime.UtcNow.AddDays(7));

api.TransponderRangeReservations.ReserveRange(reservation.Id, startFrequency: 10.0, endFrequency: 12.0);
```

**Enable logging (optional):**

Provide an `ILogger` implementation to surface diagnostics from the middleware pipeline. By default the API
uses a `NullLogger`, so logging is a no-op until you set one.

Derive from `LoggerBase` and implement the single `Log(string, LogType)` method; the `Debug`, `Information`,
`Warning` and `Error` helpers are routed through it for you.

```csharp
using Skyline.DataMiner.Solutions.SatOps.Common.Logging;

public class ConsoleLogger : LoggerBase
{
    public override void Log(string message, LogType type = LogType.Information)
    {
        Console.WriteLine($"{FormatDateTimeNow()}|{GetLogTypeAbbreviation(type)}|{message}");
    }
}

api.SetLogger(new ConsoleLogger());
```

Ready made loggers are shipped with the host specific DevPacks:

| Package | Logger | Writes to |
| --- | --- | --- |
| `...SatOps.Automation` | `EngineLogger(IEngine)` | Automation script log |
| `...SatOps.Protocol` | `ProtocolLogger(SLProtocol)` | Protocol (connector) log |

```csharp
api.SetLogger(new EngineLogger(engine));
```

> `LogType` is defined by this package (`Skyline.DataMiner.Solutions.SatOps.Common.Logging.LogType`) rather than
> reusing `Skyline.DataMiner.Automation.LogType`. This keeps the Common assembly free of any dependency on
> `SLManagedAutomation`, which is not resolvable in hosts such as the GQI DxM process.

## About

Core API library of the SatOps DevPack solution. It contains the domain models, repositories, querying exposers and validation middleware used by the SatOps Automation, GQI and Protocol DevPacks.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

## Architecture

The `SDM.SatOps.GQI`, `SDM.SatOps.Automation` and `SatOps.Protocol` packages are thin DevPacks that build on top of this core library rather than talking to DOM directly: automation scripts consume `SDM.SatOps.Automation`, low-code apps/dashboards consume `SDM.SatOps.GQI` as a data source, and connectors consume `SatOps.Protocol`. `SDM.SatOps.CommonTests` provides MSTest coverage for the core library.

![SatOps package layout](../docs/architecture.svg)

Every repository is built as a small pipeline: the public interface (e.g. `IBeamRepository`) is implemented by a repository-specific **decorator middleware** (e.g. `BeamRepositoryMiddleware`), which optionally forwards the call to a **validation middleware** (e.g. `BeamValidationMiddleware`) before reaching the actual **repository** (e.g. `BeamRepository`) that talks to DataMiner DOM instances through `DomHelper`. This keeps validation/business rules decoupled from persistence, and lets new cross-cutting behavior be added without touching the storage code.

The domain entities also form a hierarchy: a `Satellite` owns `Beam`s and `Transponder`s, a `Transponder` owns `TransponderPlan`s, and each `TransponderPlan` owns `TransponderPlanRow`s and `TransponderSlot`s. Validation middleware enforces that these parent references exist before a child entity can be created or updated.

`TransponderSlot`s are the one exception to the full CRUD surface: they are derived data, calculated from the plan rows and the transponder of their `TransponderPlan`. `ITransponderSlotRepository` therefore only inherits the creatable, deletable, pageable and countable repository interfaces — there is no update method, and regenerating the slots of a plan is done by calling `Create` with the transponder plan id, which recalculates the slots and replaces the ones currently stored for that plan.

The core library also depends on `Skyline.DataMiner.Dev.Utils.Solutions.MediaOps.Plan`: a SatOps `Transponder` is also represented as a `Resource` in the MediaOps data model, so `TransponderResourceCreationMiddleware` keeps a matching MediaOps resource in sync whenever a transponder is created or updated.
