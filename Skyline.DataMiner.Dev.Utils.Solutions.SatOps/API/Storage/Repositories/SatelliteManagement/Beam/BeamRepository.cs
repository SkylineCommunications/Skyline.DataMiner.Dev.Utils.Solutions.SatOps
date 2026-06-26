using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal class BeamRepository : Repository, IBeamRepository
    {
        public BeamRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

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

        public IReadOnlyCollection<Beam> Create(IEnumerable<Beam> oToCreate)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            return oToCreate.Select(Create).ToList();
        }

        public Beam Create(Beam oToCreate)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingBeam);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Beam> CreateOrUpdate(IEnumerable<Beam> oToCreateOrUpdate)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            var results = new List<Beam>();
            foreach (var beam in oToCreateOrUpdate)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToCreateOrUpdate));

                var existing = Read(beam.Id);
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
            if (beam != null)
                beam.ToOriginalInstance().Delete(DomHelper);
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
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            foreach (var beam in oToDelete)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));

                Delete(beam);
            }
        }

        public void Delete(Beam oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

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

        public Beam Update(Beam oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingBeam);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<Beam> Update(IEnumerable<Beam> oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            foreach (var beam in oToUpdate)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToUpdate));
            }

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

            foreach (var beam in beams)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));
            }

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

            foreach (var beam in beams)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));
            }

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

            foreach (var beam in beams)
            {
                if (beam == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beams));
            }

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

        #region Not Implemented Methods
        public long Count(FilterElement<Beam> filter)
        {
            throw new NotImplementedException();
        }

        public long Count(IQuery<Beam> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Beam> Read(FilterElement<Beam> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Beam> Read(IQuery<Beam> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query, int pageSize)
        {
            throw new NotImplementedException();
        }
        #endregion

        private Beam DoTransition(Guid id, string transitionId)
        {
            var beam = Read(id);
            if (beam == null)
                throw new ArgumentException(string.Format(ExceptionMessages.BeamWithIdWasNotFound, id), nameof(id));

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
    }
}
