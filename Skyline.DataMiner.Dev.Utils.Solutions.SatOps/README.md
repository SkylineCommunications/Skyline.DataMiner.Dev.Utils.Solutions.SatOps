# Skyline.DataMiner.Dev.Utils.Solutions.SatOps

## About

DevPack for the SatOps Solution

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

## Getting Started

`SatOpsApi` is the single entry point of this library. Give it a DataMiner `IConnection` and it exposes one repository per satellite-operations entity (`Satellites`, `Beams`, `Transponders`, `TransponderPlans`, `TransponderPlanRows`, `TransponderSlots`), each created lazily on first use:

```csharp
var api = new SatOpsApi(connection);
if (api.IsInstalled())
{
    var satellites = api.Satellites.Read();
}
```

### Architecture

The `SDM.SatOps.GQI` and `SDM.SatOps.Automation` packages are thin DevPacks that build on top of the core library rather than talking to DOM directly: automation scripts consume `SDM.SatOps.Automation`, while low-code apps/dashboards consume `SDM.SatOps.GQI` as a data source. `SDM.SatOps.CommonTests` provides MSTest coverage for the core library.

![SatOps package layout](../docs/architecture.svg)

Every repository is built as a small pipeline: the public interface (e.g. `IBeamRepository`) is implemented by a repository-specific **decorator middleware** (e.g. `BeamRepositoryMiddleware`), which optionally forwards the call to a **validation middleware** (e.g. `BeamValidationMiddleware`) before reaching the actual **repository** (e.g. `BeamRepository`) that talks to DataMiner DOM instances through `DomHelper`. This keeps validation/business rules decoupled from persistence, and lets new cross-cutting behavior be added without touching the storage code.

The domain entities also form a hierarchy: a `Satellite` owns `Beam`s and `Transponder`s, a `Transponder` owns `TransponderPlan`s, and each `TransponderPlan` owns `TransponderPlanRow`s and `TransponderSlot`s. Validation middleware enforces that these parent references exist before a child entity can be created or updated.

<!-- Uncomment below and add more info to provide more information about how to use this package. -->
<!-- ## Getting Started -->
