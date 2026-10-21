namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class RelationshipReadHelper
    {
        private const int MaxIdsPerQuery = 50;

        public static IEnumerable<T> ReadByIds<T>(
            IEnumerable<Guid> ids,
            string parameterName,
            Func<Guid, FilterElement<T>> createFilter,
            Func<FilterElement<T>, IEnumerable<T>> read)
        {
            if (ids == null)
                throw new ArgumentNullException(parameterName);

            var uniqueIds = ids.Distinct().ToList();
            if (uniqueIds.Any(id => id == Guid.Empty))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, parameterName);

            return ReadByIdsIterator(uniqueIds, createFilter, read);
        }

        private static IEnumerable<T> ReadByIdsIterator<T>(
            IReadOnlyList<Guid> ids,
            Func<Guid, FilterElement<T>> createFilter,
            Func<FilterElement<T>, IEnumerable<T>> read)
        {
            for (var i = 0; i < ids.Count; i += MaxIdsPerQuery)
            {
                var filters = ids.Skip(i).Take(MaxIdsPerQuery).Select(createFilter).ToArray();
                var filter = filters.Length == 1 ? filters[0] : new ORFilterElement<T>(filters);
                foreach (var item in read(filter))
                    yield return item;
            }
        }
    }
}
