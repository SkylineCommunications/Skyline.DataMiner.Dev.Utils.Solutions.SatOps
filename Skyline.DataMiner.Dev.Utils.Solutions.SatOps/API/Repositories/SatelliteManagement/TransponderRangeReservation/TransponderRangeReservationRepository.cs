namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderRangeReservation;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal class TransponderRangeReservationRepository : Repository, ITransponderRangeReservationRepository
    {
        private readonly TransponderRangeReservationFilterTranslator filterTranslator = new TransponderRangeReservationFilterTranslator();

        public TransponderRangeReservationRepository(SatOpsApi satOpsApi)
            : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderRangeReservationInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderReservations);
        }

        public TransponderRangeReservation Initialize()
        {
            return TransponderRangeReservation.CreateNewTransponderRangeReservation();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderRangeReservationInstance);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Create(IEnumerable<TransponderRangeReservation> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderRangeReservation Create(TransponderRangeReservation oToCreate)
        {
            if (Read(oToCreate.Id) != null)
            {
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderRangeReservation);
            }

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> CreateOrUpdate(IEnumerable<TransponderRangeReservation> oToCreateOrUpdate)
        {
            var results = new List<TransponderRangeReservation>();
            foreach (var reservation in oToCreateOrUpdate)
            {
                var existing = Read(reservation.Id);
                var result = existing == null ? CreateInternal(reservation) : UpdateInternal(reservation);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));
            }

            var reservation = Read(apiObjectId);
            if (reservation != null)
            {
                reservation.ToOriginalInstance().Delete(DomHelper);
            }
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            if (apiObjectIds == null)
            {
                throw new ArgumentNullException(nameof(apiObjectIds));
            }

            foreach (var id in apiObjectIds)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(apiObjectIds));
                }

                Delete(id);
            }
        }

        public void Delete(IEnumerable<TransponderRangeReservation> oToDelete)
        {
            if (oToDelete == null)
            {
                throw new ArgumentNullException(nameof(oToDelete));
            }

            foreach (var reservation in oToDelete)
            {
                if (reservation == null)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));
                }

                Delete(reservation);
            }
        }

        public void Delete(TransponderRangeReservation oToDelete)
        {
            if (oToDelete == null)
            {
                throw new ArgumentNullException(nameof(oToDelete));
            }

            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<TransponderRangeReservation> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderRangeReservationInstance)
                .Select(di => TransponderRangeReservation.FromInstance(new TransponderReservationsInstance(di)));
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponder(Guid transponderId)
        {
            if (transponderId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderId));
            }

            return Read(TransponderRangeReservationExposers.Transponder.Equal(transponderId));
        }

        public IEnumerable<TransponderRangeReservation> ReadByTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            ValidateTimeWindow(startTimeUtc, endTimeUtc);

            var filter = new ANDFilterElement<TransponderRangeReservation>(
                TransponderRangeReservationExposers.StartTime.LessThan(endTimeUtc),
                TransponderRangeReservationExposers.EndTime.GreaterThan(startTimeUtc));

            return Read(filter);
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponderAndTimeWindow(Guid transponderId, DateTime startTimeUtc, DateTime endTimeUtc)
        {
            if (transponderId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderId));
            }

            ValidateTimeWindow(startTimeUtc, endTimeUtc);

            var filter = new ANDFilterElement<TransponderRangeReservation>(
                TransponderRangeReservationExposers.Transponder.Equal(transponderId),
                TransponderRangeReservationExposers.StartTime.LessThan(endTimeUtc),
                TransponderRangeReservationExposers.EndTime.GreaterThan(startTimeUtc));

            return Read(filter);
        }

        public TransponderRangeReservation Read(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));
            }

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderRangeReservationInstance(domInstance))
            {
                return null;
            }

            return TransponderRangeReservation.FromInstance(new TransponderReservationsInstance(domInstance));
        }

        public IEnumerable<TransponderRangeReservation> Read(IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var idSet = new HashSet<Guid>();
            foreach (var id in ids)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(ids));
                }

                idSet.Add(id);
            }

            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsTransponderRangeReservationInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderRangeReservation.FromInstance(new TransponderReservationsInstance(di)));
        }

        public TransponderRangeReservation Update(TransponderRangeReservation oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
            {
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderRangeReservation);
            }

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Update(IEnumerable<TransponderRangeReservation> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public long Count(FilterElement<TransponderRangeReservation> filter)
        {
            if (filter.isEmpty())
            {
                return 0;
            }

            var domFilter = filterTranslator.Translate(filter);
            return SatOpsApi.SlcSatelliteManagementHelper.CountSatelliteManagementInstances(domFilter);
        }

        public long Count(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Count(query.Filter);
        }

        public IEnumerable<TransponderRangeReservation> Read(FilterElement<TransponderRangeReservation> filter)
        {
            if (filter.isEmpty())
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            var domFilter = filterTranslator.Translate(filter);
            return TransponderRangeReservation.InstantiateTransponderRangeReservations(SatOpsApi.SlcSatelliteManagementHelper.GetTransponderReservations(domFilter));
        }

        public IEnumerable<TransponderRangeReservation> Read(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<TransponderRangeReservation>());
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<TransponderRangeReservation>(), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter, int pageSize)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query, int pageSize)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ReadPaged(query.Filter, pageSize);
        }

        private IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPagedIterator(FilterElement<TransponderRangeReservation> filter, int pageSize)
        {
            var pageNumber = 0;
            var domFilter = filterTranslator.Translate(filter);
            var items = SatOpsApi.SlcSatelliteManagementHelper.GetTransponderReservationsPaged(domFilter, pageSize);

            var enumerator = items.GetEnumerator();
            var hasNext = enumerator.MoveNext();

            while (hasNext)
            {
                var page = enumerator.Current;
                hasNext = enumerator.MoveNext();
                yield return new PagedResult<TransponderRangeReservation>(TransponderRangeReservation.InstantiateTransponderRangeReservations(page), pageNumber++, pageSize, hasNext);
            }
        }

        private static void ValidateTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            if (startTimeUtc >= endTimeUtc)
            {
                throw new ArgumentException("The startTimeUtc must be earlier than endTimeUtc.");
            }
        }

        private TransponderRangeReservation CreateInternal(TransponderRangeReservation reservation)
        {
            var updatedInstance = reservation.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return TransponderRangeReservation.FromInstance(new TransponderReservationsInstance(createdDomInstance));
        }

        private TransponderRangeReservation UpdateInternal(TransponderRangeReservation reservation)
        {
            var updatedInstance = reservation.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return TransponderRangeReservation.FromInstance(new TransponderReservationsInstance(updatedDomInstance));
        }
    }
}
