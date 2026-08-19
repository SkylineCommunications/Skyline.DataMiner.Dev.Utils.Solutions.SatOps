namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Beam
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class BeamRepository : Repository, IBeamRepository
    {
        public BeamRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private readonly BeamFilterTranslator filterTranslator = new BeamFilterTranslator();

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsBeamInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.Beams);
        }

        public Beam Initialize()
        {
            return Beam.CreateNewBeam();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsBeamInstance);
        }

        public long Count(FilterElement<Beam> filter)
        {
            if (filter.isEmpty())
            {
                return 0;
            }

            var domFilter = filterTranslator.Translate(filter);
            return SatOpsApi.SlcSatelliteManagementHelper.CountSatelliteManagementInstances(domFilter);
        }

        public long Count(IQuery<Beam> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Count(query.Filter);
        }

        public IReadOnlyCollection<Beam> Create(IEnumerable<Beam> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public Beam Create(Beam oToCreate)
        {
            if (oToCreate.Id != Guid.Empty && Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingBeam);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Beam> CreateOrUpdate(IEnumerable<Beam> oToCreateOrUpdate)
        {
            var results = new List<Beam>();
            foreach (var beam in oToCreateOrUpdate)
            {
                var existing = beam.Id != Guid.Empty ? Read(beam.Id) : null;
                var result = existing == null ? CreateInternal(beam) : UpdateInternal(beam);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var beam = Read(apiObjectId);

            beam?.ToOriginalInstance().Delete(DomHelper);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            if (apiObjectIds == null)
                throw new ArgumentNullException(nameof(apiObjectIds));

            foreach (var id in apiObjectIds)
            {
                if (id == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(apiObjectIds));

                Delete(id);
            }
        }

        public void Delete(IEnumerable<Beam> oToDelete)
        {
            foreach (var beam in oToDelete)
            {
                Delete(beam);
            }
        }

        public void Delete(Beam oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<Beam> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsBeamInstance)
                .Select(di => Beam.FromInstance(new BeamsInstance(di)));
        }

        public Beam Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsBeamInstance(domInstance))
                return null;

            return Beam.FromInstance(new BeamsInstance(domInstance));
        }

        public IEnumerable<Beam> Read(IEnumerable<Guid> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var idSet = new HashSet<Guid>();
            foreach (var id in ids)
            {
                if (id == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(ids));

                idSet.Add(id);
            }

            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsBeamInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => Beam.FromInstance(new BeamsInstance(di)));
        }

        public IEnumerable<Beam> Read(FilterElement<Beam> filter)
        {
            if (filter.isEmpty())
            {
                return Enumerable.Empty<Beam>();
            }

            var domFilter = filterTranslator.Translate(filter);
            return Beam.InstantiateBeams(SatOpsApi.SlcSatelliteManagementHelper.GetBeams(domFilter));
        }

        public IEnumerable<Beam> Read(IQuery<Beam> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query.Filter);
        }

        public Beam Update(Beam oToUpdate)
        {
            if (oToUpdate.Id == Guid.Empty || Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingBeam);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<Beam> Update(IEnumerable<Beam> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public Beam Activate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Transitions.Draft_To_Active);
        }

        public Beam Activate(Beam beam)
        {
            if (beam == null)
                throw new ArgumentNullException(nameof(beam));

            return Activate(beam.Id);
        }

        public IReadOnlyCollection<Beam> Activate(IEnumerable<Beam> beams)
        {
            if (beams == null)
                throw new ArgumentNullException(nameof(beams));

            if(beams.Any(beam => beam == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));

            return beams.Select(Activate).ToList();
        }

        public IReadOnlyCollection<Beam> Activate(IEnumerable<Guid> beamIds)
        {
            if (beamIds == null)
                throw new ArgumentNullException(nameof(beamIds));

            foreach (var beamId in beamIds)
            {
                if (beamId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(beamIds));
            }

            return beamIds.Select(Activate).ToList();
        }

        public Beam Deprecate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Transitions.Active_To_Deprecated);
        }

        public Beam Deprecate(Beam beam)
        {
            if (beam == null)
                throw new ArgumentNullException(nameof(beam));

            return Deprecate(beam.Id);
        }

        public IReadOnlyCollection<Beam> Deprecate(IEnumerable<Beam> beams)
        {
            if (beams == null)
                throw new ArgumentNullException(nameof(beams));

            if (beams.Any(beam => beam == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));

            return beams.Select(Deprecate).ToList();
        }

        public IReadOnlyCollection<Beam> Deprecate(IEnumerable<Guid> beamIds)
        {
            if (beamIds == null)
                throw new ArgumentNullException(nameof(beamIds));

            foreach (var beamId in beamIds)
            {
                if (beamId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(beamIds));
            }

            return beamIds.Select(Deprecate).ToList();
        }

        public Beam Reactivate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Transitions.Deprecated_To_Active);
        }

        public Beam Reactivate(Beam beam)
        {
            if (beam == null)
                throw new ArgumentNullException(nameof(beam));

            return Reactivate(beam.Id);
        }

        public IReadOnlyCollection<Beam> Reactivate(IEnumerable<Beam> beams)
        {
            if (beams == null)
                throw new ArgumentNullException(nameof(beams));

            if (beams.Any(beam => beam == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));

            return beams.Select(Reactivate).ToList();
        }

        public IReadOnlyCollection<Beam> Reactivate(IEnumerable<Guid> beamIds)
        {
            if (beamIds == null)
                throw new ArgumentNullException(nameof(beamIds));

            foreach (var beamId in beamIds)
            {
                if (beamId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(beamIds));
            }

            return beamIds.Select(Reactivate).ToList();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<Beam>());
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<Beam>(), pageSize);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter, pageSize);
        }

        private Beam DoTransition(Guid id, string transitionId)
        {
            var beam = Read(id) ?? throw new ArgumentException(string.Format(ExceptionMessages.BeamWithIdWasNotFound, id), nameof(id));

            var domInstanceId = beam.ToOriginalInstance().ID;
            var transitionedDomInstance = DomHelper.DomInstances.DoStatusTransition(domInstanceId, transitionId);
            return Beam.FromInstance(new BeamsInstance(transitionedDomInstance));
        }

        private Beam CreateInternal(Beam beam)
        {
            var updatedInstance = beam.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return Beam.FromInstance(new BeamsInstance(createdDomInstance));
        }

        private Beam UpdateInternal(Beam beam)
        {
            var updatedInstance = beam.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return Beam.FromInstance(new BeamsInstance(updatedDomInstance));
        }

        private IEnumerable<IPagedResult<Beam>> ReadPagedIterator(FilterElement<Beam> filter, int pageSize)
        {
            var pageNumber = 0;
            var paramFilter = filterTranslator.Translate(filter);

            var items = SatOpsApi.SlcSatelliteManagementHelper.GetBeamsPaged(paramFilter, pageSize);

            var enumerator = items.GetEnumerator();
            var hasNext = enumerator.MoveNext();

            while (hasNext)
            {
                var page = enumerator.Current;
                hasNext = enumerator.MoveNext();
                yield return new PagedResult<Beam>(Beam.InstantiateBeams(page), pageNumber++, pageSize, hasNext);
            }

        }
    }
}
