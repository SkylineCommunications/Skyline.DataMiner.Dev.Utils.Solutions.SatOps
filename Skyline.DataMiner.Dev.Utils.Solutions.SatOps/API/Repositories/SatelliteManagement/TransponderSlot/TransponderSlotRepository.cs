namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using SLDataGateway.API.Types.Querying;
    using SLDataGateway.API.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;

    internal class TransponderSlotRepository : Repository, ITransponderSlotRepository
    {
        public TransponderSlotRepository(SatOpsApi satOpsApi) : this(satOpsApi, new TransponderSlotGenerator(satOpsApi))
        {
        }

        public TransponderSlotRepository(SatOpsApi satOpsApi, ITransponderSlotGenerator slotGenerator) : base(satOpsApi)
        {
            this.slotGenerator = slotGenerator ?? throw new ArgumentNullException(nameof(slotGenerator));
        }

        private readonly ITransponderSlotGenerator slotGenerator;

        private readonly TransponderSlotFilterTranslator filterTranslator = new TransponderSlotFilterTranslator();

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderSlotInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderSlots);
        }

        public IReadOnlyCollection<TransponderSlot> Create(IEnumerable<TransponderSlot> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderSlot Create(TransponderSlot oToCreate)
        {
            if (oToCreate.Id != Guid.Empty && Read(oToCreate.Id) != null)
                throw new SlotValidationException("The selected slot already exists. Edit it instead of creating it again.", nameof(oToCreate), isConflict: true);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> Create(Guid transponderPlanId)
        {
            return this
                .WithMiddleware(new TransponderSlotValidationMiddleware(SatOpsApi.TransponderPlans), slotGenerator)
                .WithMiddleware(new SlotOverlapValidationMiddleware(this), slotGenerator)
                .Create(transponderPlanId);
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));
            if (oToUpdate.Id == Guid.Empty || Read(oToUpdate.Id) == null)
                throw new SlotNotFoundException(oToUpdate.Id);

            var instance = oToUpdate.ToUpdatedInstance();
            var updated = DomHelper.DomInstances.Update(instance.ToInstance());
            return TransponderSlot.FromInstance(new TransponderSlotsInstance(updated));
        }

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            return oToUpdate.Select(Update).ToList();
        }

        public void Delete(IEnumerable<TransponderSlot> oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            foreach (var transponderSlot in oToDelete)
            {
                if (transponderSlot == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));

                Delete(transponderSlot);
            }
        }

        public void Delete(TransponderSlot oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        private TransponderSlot Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderSlotInstance(domInstance))
                return null;

            return TransponderSlot.FromInstance(new TransponderSlotsInstance(domInstance));
        }

        public long Count(FilterElement<TransponderSlot> filter)
        {
            if (filter.isEmpty())
            {
                return 0;
            }

            var domFilter = filterTranslator.Translate(filter);
            return SatOpsApi.SlcSatelliteManagementHelper.CountSatelliteManagementInstances(domFilter);
        }

        public long Count(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Count(query.Filter);
        }

        public IEnumerable<TransponderSlot> Read(FilterElement<TransponderSlot> filter)
        {
            if (filter.isEmpty())
            {
                return Enumerable.Empty<TransponderSlot>();
            }

            var domFilter = filterTranslator.Translate(filter);
            return TransponderSlot.InstantiateTransponderSlots(SatOpsApi.SlcSatelliteManagementHelper.GetTransponderSlots(domFilter));
        }

        public IEnumerable<TransponderSlot> Read(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var domQuery = filterTranslator.TranslateQuery(query);
            return TransponderSlot.InstantiateTransponderSlots(SatOpsApi.SlcSatelliteManagementHelper.GetTransponderSlots(domQuery));
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query, 100);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return ReadPagedIterator(filter.ToQuery(), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPagedIterator(query, pageSize);
        }

        private IEnumerable<IPagedResult<TransponderSlot>> ReadPagedIterator(IQuery<TransponderSlot> query, int pageSize)
        {
            var pageNumber = 0;
            var domFilter = filterTranslator.TranslateQuery(query);
            var items = SatOpsApi.SlcSatelliteManagementHelper.GetTransponderSlotsPaged(domFilter, pageSize);

            var enumerator = items.GetEnumerator();
            var hasNext = enumerator.MoveNext();

            while (hasNext)
            {
                var page = enumerator.Current;
                hasNext = enumerator.MoveNext();
                yield return new PagedResult<TransponderSlot>(TransponderSlot.InstantiateTransponderSlots(page), pageNumber++, pageSize, hasNext);
            }
        }

        private TransponderSlot CreateInternal(TransponderSlot transponderSlot)
        {
            var updatedInstance = transponderSlot.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return TransponderSlot.FromInstance(new TransponderSlotsInstance(createdDomInstance));
        }

        public IEnumerable<TransponderSlot> ReadByTransponderPlan(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            return Read(TransponderSlotExposers.TransponderPlan.Equal(transponderPlanId));
        }

        public IEnumerable<TransponderSlot> ReadByTransponderPlans(IEnumerable<Guid> transponderPlanIds)
        {
            return RelationshipReadHelper.ReadByIds<TransponderSlot>(
                transponderPlanIds,
                nameof(transponderPlanIds),
                id => TransponderSlotExposers.TransponderPlan.Equal(id),
                filter => Read(filter));
        }

        public void DeleteSlotsByTransponderPlan(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            var domFilter = DomInstanceExposers.FieldValues
                .DomInstanceField(SlcSatellite_ManagementIds.Sections.TransponderSlot.TransponderPlan)
                .Equal(transponderPlanId);
            var existingInstances = DomHelper.DomInstances.Read(domFilter).ToList();
            if (existingInstances.Count > 0)
            {
                var result = DomHelper.DomInstances.DeleteInBatches(existingInstances);
                if (result.UnsuccessfulIds.Count != 0 || result.SuccessfulIds.Count != existingInstances.Count)
                    throw new InvalidOperationException(
                        $"Could not delete all transponder slots. Succeeded: {result.SuccessfulIds.Count}; failed IDs: {string.Join(", ", result.UnsuccessfulIds)}.");
            }
        }
    }
}
