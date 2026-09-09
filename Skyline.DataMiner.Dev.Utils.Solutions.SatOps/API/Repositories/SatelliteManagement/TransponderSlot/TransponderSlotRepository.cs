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
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderSlot);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> Create(Guid transponderPlanId)
        {
            return RegenerateSlots(transponderPlanId);
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

        private IReadOnlyCollection<TransponderSlot> RegenerateSlots(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            var slots = slotGenerator.BuildSlots(transponderPlanId);
            DeleteSlotsByTransponderPlan(transponderPlanId);

            return slots.Select(CreateInternal).ToList();
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

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter, pageSize);
        }

        private IEnumerable<IPagedResult<TransponderSlot>> ReadPagedIterator(FilterElement<TransponderSlot> filter, int pageSize)
        {
            var pageNumber = 0;
            var domFilter = filterTranslator.Translate(filter);
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

        public void DeleteSlotsByTransponderPlan(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            var domFilter = DomInstanceExposers.FieldValues
                .DomInstanceField(SlcSatellite_ManagementIds.Sections.TransponderSlot.TransponderPlan)
                .Equal(transponderPlanId);
            var existingInstances = DomHelper.DomInstances.Read(domFilter).ToList();
            if (existingInstances.Count > 0)
                DomHelper.DomInstances.DeleteInBatches(existingInstances);
        }
    }
}
