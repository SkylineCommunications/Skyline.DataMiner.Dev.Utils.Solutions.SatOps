namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderSlotRepositoryMiddleware : ITransponderSlotRepository, ITransponderSlotBuilder
    {
        private readonly ITransponderSlotRepository inner;
        private readonly IMiddlewareMarker<TransponderSlot> middleware;

        public TransponderSlotRepositoryMiddleware(ITransponderSlotRepository inner, IMiddlewareMarker<TransponderSlot> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public long Count()
        {
            return inner.Count();
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

        public IReadOnlyCollection<TransponderSlot> CreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate)
        {
            var slots = ExpandGenerationRequests(oToCreateOrUpdate);

            if (middleware is IBulkRepositoryMiddleware<TransponderSlot> bulkRepositoryMiddleware)
            {
                return bulkRepositoryMiddleware.OnCreateOrUpdate(slots, inner.CreateOrUpdate);
            }

            return inner.CreateOrUpdate(slots);
        }

        public void Delete(Guid apiObjectId)
        {
            inner.Delete(apiObjectId);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            inner.Delete(apiObjectIds);
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

        public IEnumerable<TransponderSlot> Read()
        {
            return inner.Read();
        }

        public TransponderSlot Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<TransponderSlot> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
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

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
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

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<TransponderSlot> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderSlot> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> BuildSlots(Guid transponderPlanId)
        {
            if (inner is ITransponderSlotBuilder builder)
            {
                return builder.BuildSlots(transponderPlanId);
            }

            throw new NotSupportedException($"The wrapped repository does not implement {nameof(ITransponderSlotBuilder)}.");
        }

        /// <summary>
        /// Replaces every slot generation request by the slots calculated for its transponder plan.
        /// The existing slots of that plan are removed first, otherwise every calculated slot would be
        /// reported as overlapping the persisted slot it replaces.
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

                var built = BuildSlots(transponderPlanId);
                inner.DeleteSlotsByTransponderPlan(transponderPlanId);
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
        public static ITransponderSlotRepository WithMiddleware(this ITransponderSlotRepository repository, IMiddlewareMarker<TransponderSlot> middleware)
        {
            return new TransponderSlotRepositoryMiddleware(repository, middleware);
        }
    }
}
