namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderSlotRepositoryMiddleware : ITransponderSlotRepository
    {
        private readonly ITransponderSlotRepository inner;
        private readonly IMiddlewareMarker<TransponderSlot> middleware;
        private readonly ITransponderSlotGenerator slotGenerator;

        public TransponderSlotRepositoryMiddleware(ITransponderSlotRepository inner, IMiddlewareMarker<TransponderSlot> middleware)
            : this(inner, middleware, null)
        {
        }

        public TransponderSlotRepositoryMiddleware(ITransponderSlotRepository inner, IMiddlewareMarker<TransponderSlot> middleware, ITransponderSlotGenerator slotGenerator)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
            this.slotGenerator = slotGenerator;
        }

        public long Count(FilterElement<TransponderSlot> filter)
        {
            if (middleware is ICountableMiddleware<TransponderSlot> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<TransponderSlot> query)
        {
            if (middleware is ICountableMiddleware<TransponderSlot> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<TransponderSlot> Create(IEnumerable<TransponderSlot> oToCreate)
        {
            var slots = ExpandGenerationRequests(oToCreate);

            if (middleware is IBulkCreatableMiddleware<TransponderSlot> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(slots, inner.Create);
            }

            return inner.Create(slots);
        }

        public TransponderSlot Create(TransponderSlot oToCreate)
        {
            if (IsGenerationRequest(oToCreate))
            {
                return Create(new[] { oToCreate }).FirstOrDefault();
            }

            if (middleware is ICreatableMiddleware<TransponderSlot> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> Create(Guid transponderPlanId)
        {
            var slots = RegenerateSlots(transponderPlanId);

            if (middleware is IBulkCreatableMiddleware<TransponderSlot> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(slots, inner.Create);
            }

            return inner.Create(slots);
        }

        public void Delete(IEnumerable<TransponderSlot> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<TransponderSlot> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(TransponderSlot oToDelete)
        {
            if (middleware is IDeletableMiddleware<TransponderSlot> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<TransponderSlot> Read(FilterElement<TransponderSlot> filter)
        {
            if (middleware is IReadableMiddleware<TransponderSlot> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<TransponderSlot> Read(IQuery<TransponderSlot> query)
        {
            if (middleware is IReadableMiddleware<TransponderSlot> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        /// <summary>
        /// Calculates the slots of the given transponder plan and removes the slots that are currently persisted
        /// for that plan. The existing slots are removed before the calculated ones are validated, otherwise every
        /// calculated slot would be reported as overlapping the persisted slot it replaces.
        /// </summary>
        private IReadOnlyCollection<TransponderSlot> RegenerateSlots(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            if (slotGenerator == null)
                throw new NotSupportedException($"No {nameof(ITransponderSlotGenerator)} is configured for this repository.");

            var slots = slotGenerator.BuildSlots(transponderPlanId);
            inner.DeleteSlotsByTransponderPlan(transponderPlanId);

            return slots;
        }

        /// <summary>
        /// Replaces every slot generation request by the slots calculated for its transponder plan.
        /// </summary>
        private IEnumerable<TransponderSlot> ExpandGenerationRequests(IEnumerable<TransponderSlot> slots)
        {
            if (slots == null)
                return null;

            var materialized = slots.ToList();
            if (!materialized.Any(IsGenerationRequest))
                return materialized;

            var expanded = new List<TransponderSlot>();
            var handledPlans = new HashSet<Guid>();

            foreach (var slot in materialized)
            {
                if (!IsGenerationRequest(slot))
                {
                    expanded.Add(slot);
                    continue;
                }

                var transponderPlanId = slot.TransponderPlan.Value;
                if (!handledPlans.Add(transponderPlanId))
                    continue;

                var built = RegenerateSlots(transponderPlanId);
                expanded.AddRange(built);
            }

            return expanded;
        }

        /// <summary>
        /// Determines whether the given slot is a request to generate all slots of its transponder plan,
        /// which is the case when only the transponder plan is filled in.
        /// </summary>
        private static bool IsGenerationRequest(TransponderSlot slot)
        {
            return slot != null
                && slot.TransponderPlan.HasValue
                && slot.TransponderPlan.Value != Guid.Empty
                && string.IsNullOrEmpty(slot.Name)
                && !slot.SlotStartFrequency.HasValue
                && !slot.SlotEndFrequency.HasValue;
        }

        public IEnumerable<TransponderSlot> ReadByTransponderPlan(Guid transponderPlanId)
        {
            return inner.ReadByTransponderPlan(transponderPlanId);
        }

        public void DeleteSlotsByTransponderPlan(Guid transponderPlanId)
        {
            inner.DeleteSlotsByTransponderPlan(transponderPlanId);
        }
    }

    internal static class TransponderSlotRepositoryExtensions
    {
        public static ITransponderSlotRepository WithMiddleware(this ITransponderSlotRepository repository, IMiddlewareMarker<TransponderSlot> middleware, ITransponderSlotGenerator slotGenerator)
        {
            return new TransponderSlotRepositoryMiddleware(repository, middleware, slotGenerator);
        }
    }
}
