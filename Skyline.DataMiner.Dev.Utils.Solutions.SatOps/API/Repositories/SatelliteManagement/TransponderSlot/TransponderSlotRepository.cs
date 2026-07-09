namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TransponderSlotRepository : Repository, ITransponderSlotRepository
    {
        public TransponderSlotRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private readonly TransponderSlotFilterTranslator filterTranslator = new TransponderSlotFilterTranslator();

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderSlotInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderSlots);
        }

        public TransponderSlot Initialize()
        {
            return TransponderSlot.CreateNewTransponderSlot();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderSlotInstance);
        }

        public IReadOnlyCollection<TransponderSlot> Create(IEnumerable<TransponderSlot> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderSlot Create(TransponderSlot oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderSlot);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> CreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate)
        {
            var results = new List<TransponderSlot>();
            foreach (var transponderSlot in oToCreateOrUpdate)
            {
                var existing = Read(transponderSlot.Id);
                var result = existing == null ? CreateInternal(transponderSlot) : UpdateInternal(transponderSlot);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var transponderSlot = Read(apiObjectId);
            if (transponderSlot != null)
                transponderSlot.ToOriginalInstance().Delete(DomHelper);
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

        public IEnumerable<TransponderSlot> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderSlotInstance)
                .Select(di => TransponderSlot.FromInstance(new TransponderSlotsInstance(di)));
        }

        public TransponderSlot Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderSlotInstance(domInstance))
                return null;

            return TransponderSlot.FromInstance(new TransponderSlotsInstance(domInstance));
        }

        public IEnumerable<TransponderSlot> Read(IEnumerable<Guid> ids)
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
                .Where(di => IsTransponderSlotInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderSlot.FromInstance(new TransponderSlotsInstance(di)));
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderSlot);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
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

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<TransponderSlot>());
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<TransponderSlot>(), pageSize);
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

        private TransponderSlot UpdateInternal(TransponderSlot transponderSlot)
        {
            var updatedInstance = transponderSlot.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return TransponderSlot.FromInstance(new TransponderSlotsInstance(updatedDomInstance));
        }

        public IReadOnlyCollection<TransponderSlot> GenerateSlots(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            var plan = SatOpsApi.TransponderPlans.Read(transponderPlanId);
            if (plan == null)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderPlanWithIdWasNotFound, transponderPlanId), nameof(transponderPlanId));

            if (!plan.Transponder.HasValue || plan.Transponder.Value == Guid.Empty)
                throw new InvalidOperationException(string.Format(ExceptionMessages.TransponderPlanHasNoAssociatedTransponder, transponderPlanId));

            var transponder = SatOpsApi.Transponders.Read(plan.Transponder.Value);
            if (transponder == null)
                throw new InvalidOperationException(string.Format(ExceptionMessages.TransponderForPlanWasNotFound, plan.Transponder.Value, transponderPlanId));

            var planRows = SatOpsApi.TransponderPlanRows
                .Read(TransponderPlanRowExposers.TransponderPlan.Equal(transponderPlanId))
                .ToList();

            DeleteByTransponderPlan(transponderPlanId);

            double transponderBandwidth = transponder.Bandwidth.GetValueOrDefault();
            double transponderStartFrequency = transponder.StartFrequency.GetValueOrDefault();
            double transponderDownlinkStartFrequency = transponder.DownlinkStartFreq.GetValueOrDefault();

            var createdSlots = new List<TransponderSlot>();
            foreach (var row in planRows)
            {
                double offset = row.Offset.GetValueOrDefault();
                double step = row.StepSize.GetValueOrDefault();
                double bandwidth = row.Bandwidth.GetValueOrDefault();
                double limit = row.Limit.GetValueOrDefault();

                foreach (var calc in CalculateSlots(transponderBandwidth, transponderStartFrequency, transponderDownlinkStartFrequency, offset, step, bandwidth, limit))
                {
                    var slot = Initialize();
                    slot.TransponderPlan = transponderPlanId;
                    slot.Name = calc.SlotName;
                    slot.SlotStartFrequency = calc.StartFrequency;
                    slot.SlotEndFrequency = calc.StopFrequency;
                    slot.Bandwidth = bandwidth;
                    slot.UplinkFreq = calc.UplinkFrequency;
                    slot.DownlinkFreq = calc.DownlinkFrequency;
                    createdSlots.Add(CreateInternal(slot));
                }
            }

            return createdSlots;
        }

        public IReadOnlyCollection<TransponderSlot> GenerateSlots(TransponderPlan transponderPlan)
        {
            if (transponderPlan == null)
                throw new ArgumentNullException(nameof(transponderPlan));

            return GenerateSlots(transponderPlan.Id);
        }

        public IEnumerable<TransponderSlot> ReadByTransponderPlan(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            return Read(TransponderSlotExposers.TransponderPlan.Equal(transponderPlanId));
        }

        public void DeleteByTransponderPlan(Guid transponderPlanId)
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

        private static IEnumerable<SlotCalculation> CalculateSlots(
            double transponderBandwidth,
            double transponderStartFrequency,
            double transponderDownlinkStartFrequency,
            double offset,
            double step,
            double bandwidth,
            double limit)
        {
            if (bandwidth <= 0)
                yield break;

            int numberOfSlots = (int)Math.Round(transponderBandwidth / bandwidth, MidpointRounding.AwayFromZero);
            for (int i = 0; i < numberOfSlots; i++)
            {
                double startFreq = Math.Round(offset + (i * step), FrequencyPrecision, MidpointRounding.AwayFromZero);
                double stopFreq = Math.Round(startFreq + bandwidth, FrequencyPrecision, MidpointRounding.AwayFromZero);

                if ((limit > 0 && stopFreq - limit > FrequencyComparisonTolerance) ||
                    stopFreq - transponderBandwidth > FrequencyComparisonTolerance)
                    yield break;

                double midpoint = (startFreq + stopFreq) / 2;
                yield return new SlotCalculation
                {
                    SlotName = $"{GetAlphabetLetter(i)}{bandwidth}",
                    StartFrequency = startFreq,
                    StopFrequency = stopFreq,
                    UplinkFrequency = midpoint + transponderStartFrequency,
                    DownlinkFrequency = midpoint + transponderDownlinkStartFrequency,
                };
            }
        }

        private static string GetAlphabetLetter(int index)
        {
            const int alphabetLength = 26;
            if (index < alphabetLength)
                return ((char)('A' + index)).ToString();

            return $"{(char)('A' + (index / alphabetLength) - 1)}{(char)('A' + (index % alphabetLength))}";
        }

        private const int FrequencyPrecision = 12;
        private const double FrequencyComparisonTolerance = 1e-12;

        private sealed class SlotCalculation
        {
            public string SlotName { get; set; }

            public double StartFrequency { get; set; }

            public double StopFrequency { get; set; }

            public double UplinkFrequency { get; set; }

            public double DownlinkFrequency { get; set; }
        }
    }
}
