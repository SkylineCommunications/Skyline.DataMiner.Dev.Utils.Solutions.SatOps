namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Shared helpers for the validation middlewares that resolve the objects referenced by a batch with as few
    /// repository reads as possible.
    /// </summary>
    internal static class ReferenceValidationHelper
    {
        /// <summary>
        /// Reads the objects referenced by a batch in one call and returns the identifiers that exist.
        /// </summary>
        /// <typeparam name="TItem">The type of the submitted objects.</typeparam>
        /// <typeparam name="TReference">The type of the referenced objects.</typeparam>
        /// <param name="items">The submitted objects. <see langword="null"/> entries are ignored.</param>
        /// <param name="referenceSelector">Selects the identifier of the referenced object.</param>
        /// <param name="repository">The repository holding the referenced objects.</param>
        /// <returns>The identifiers of the referenced objects that exist.</returns>
        /// <remarks>
        /// Items without a usable reference are skipped; they are rejected by the per-item validation, which keeps the
        /// original validation order and therefore the original error for a batch containing an invalid item.
        /// </remarks>
        public static HashSet<Guid> ReadExistingIds<TItem, TReference>(
            IEnumerable<TItem> items,
            Func<TItem, Guid?> referenceSelector,
            IRepository<TReference> repository)
            where TItem : class
            where TReference : class, IIdentifiable
        {
            if (referenceSelector == null)
                throw new ArgumentNullException(nameof(referenceSelector));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            var existingIds = new HashSet<Guid>();

            var referencedIds = (items ?? Enumerable.Empty<TItem>())
                .Where(item => item != null)
                .Select(referenceSelector)
                .Where(id => id.HasValue && id.Value != Guid.Empty)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            if (referencedIds.Count == 0)
                return existingIds;

            var references = repository.Read(referencedIds);
            if (references == null)
                return existingIds;

            foreach (var reference in references)
            {
                if (reference != null)
                    existingIds.Add(reference.Id);
            }

            return existingIds;
        }

        /// <summary>
        /// Returns the persisted objects related to <paramref name="key"/>, reading them at most once per key.
        /// </summary>
        /// <typeparam name="TValue">The type of the related objects.</typeparam>
        /// <param name="key">The identifier of the parent object.</param>
        /// <param name="cache">The cache for the current batch, or <see langword="null"/> to always read.</param>
        /// <param name="read">Reads the objects related to the supplied identifier.</param>
        /// <returns>The objects related to <paramref name="key"/>, never <see langword="null"/>.</returns>
        /// <remarks>
        /// Caching within a single bulk call is safe because validation completes before anything is persisted, so the
        /// underlying data cannot change while the batch is being validated.
        /// </remarks>
        public static IReadOnlyList<TValue> ReadCached<TValue>(
            Guid key,
            IDictionary<Guid, IReadOnlyList<TValue>> cache,
            Func<Guid, IEnumerable<TValue>> read)
        {
            if (read == null)
                throw new ArgumentNullException(nameof(read));

            if (cache != null && cache.TryGetValue(key, out var cachedValues))
                return cachedValues;

            var values = (read(key) ?? Enumerable.Empty<TValue>()).ToList();

            if (cache != null)
                cache[key] = values;

            return values;
        }
    }
}
