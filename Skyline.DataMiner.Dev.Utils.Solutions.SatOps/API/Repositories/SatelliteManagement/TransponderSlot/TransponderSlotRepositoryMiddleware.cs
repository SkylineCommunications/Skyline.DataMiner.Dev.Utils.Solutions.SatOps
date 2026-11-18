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
    using System.Threading;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;

    internal sealed class TransponderSlotRepositoryMiddleware : ITransponderSlotRepository
    {
        private static readonly object RegenerationGate = new object();
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
            var requests = oToCreate?.ToList();
            if (requests != null && requests.Any(IsGenerationRequest))
            {
                return Regenerate(requests);
            }

            if (middleware is IBulkCreatableMiddleware<TransponderSlot> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(requests, inner.Create);
            }

            return inner.Create(requests);
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
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            return Regenerate(new[] { new TransponderSlot { TransponderPlan = transponderPlanId } });
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderSlot> updatable)
                return updatable.OnUpdate(oToUpdate, inner.Update);

            return inner.Update(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            var slots = oToUpdate?.ToList();
            if (middleware is IBulkUpdatableMiddleware<TransponderSlot> updatable)
                return updatable.OnUpdate(slots, inner.Update);

            return inner.Update(slots);
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

        private IReadOnlyCollection<TransponderSlot> Regenerate(IEnumerable<TransponderSlot> requests)
        {
            if (slotGenerator == null)
                throw new NotSupportedException($"No {nameof(ITransponderSlotGenerator)} is configured for this repository.");

            // This process-wide guard is deliberately non-reentrant. Cross-process callers must coordinate externally.
            if (Monitor.IsEntered(RegenerationGate) || !Monitor.TryEnter(RegenerationGate))
                throw new SlotValidationException("Slot regeneration is already in progress. Wait for it to finish and try again.", isConflict: true);

            try
            {
                var expanded = new List<TransponderSlot>();
                var replacedPlans = new HashSet<Guid>();
                foreach (var request in requests)
                {
                    if (!IsGenerationRequest(request))
                    {
                        expanded.Add(request);
                        continue;
                    }

                    var planId = request.TransponderPlan.Value;
                    if (replacedPlans.Add(planId))
                        expanded.AddRange(slotGenerator.BuildSlots(planId));
                }

                return ValidateAndReplace(expanded, replacedPlans);
            }
            finally
            {
                Monitor.Exit(RegenerationGate);
            }
        }

        private IReadOnlyCollection<TransponderSlot> ValidateAndReplace(
            IReadOnlyCollection<TransponderSlot> slots, ISet<Guid> replacedPlans)
        {
            Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next = validated =>
            {
                var materialized = validated.ToList();
                if (inner is TransponderSlotRepositoryMiddleware wrapped)
                    return wrapped.ValidateAndReplace(materialized, replacedPlans);

                var stage = "Delete";
                try
                {
                    foreach (var planId in replacedPlans)
                        inner.DeleteSlotsByTransponderPlan(planId);

                    stage = "Create";
                    return inner.Create(materialized);
                }
                catch (Exception exception)
                {
                    throw new SlotRegenerationException(replacedPlans, stage, exception);
                }
            };

            if (middleware is SlotOverlapValidationMiddleware overlap)
                return overlap.OnReplace(slots, replacedPlans, next);
            if (middleware is IBulkCreatableMiddleware<TransponderSlot> validation)
                return validation.OnCreate(slots, next);

            return next(slots);
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

        public IEnumerable<TransponderSlot> ReadByTransponderPlans(IEnumerable<Guid> transponderPlanIds)
        {
            return inner.ReadByTransponderPlans(transponderPlanIds);
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
