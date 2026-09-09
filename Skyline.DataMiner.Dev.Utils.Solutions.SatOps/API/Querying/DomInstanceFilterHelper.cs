namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;

    internal static class DomInstanceFilterHelper
    {
        public static FilterElement<DomInstance> BuildOrFilter(FieldDescriptorID field, IEnumerable<Guid> ids)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var filters = ids?
                .Select(id => DomInstanceExposers.FieldValues.DomInstanceField(field).Contains(id))
                .Cast<FilterElement<DomInstance>>()
                .ToList() ?? new List<FilterElement<DomInstance>>();

            if (filters.Count == 0)
            {
                return null;
            }

            if (filters.Count == 1)
            {
                return filters[0];
            }

            return new ORFilterElement<DomInstance>(filters.ToArray());
        }

        /// <summary>
        /// Builds a filter that matches the DOM instances with one of the supplied identifiers.
        /// </summary>
        /// <param name="ids">The identifiers to match. Duplicates are ignored.</param>
        /// <param name="moduleId">The DOM module the instances belong to.</param>
        /// <returns>
        /// A filter matching any of the supplied identifiers, or <see langword="null"/> when no identifier was supplied.
        /// </returns>
        /// <remarks>
        /// This keeps the identifier selection on the server instead of reading every instance of the module
        /// and discarding the non-matching ones client side.
        /// </remarks>
        public static FilterElement<DomInstance> BuildIdOrFilter(IEnumerable<Guid> ids, string moduleId)
        {
            var filters = ids?
                .Distinct()
                .Select(id => DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = moduleId }))
                .Cast<FilterElement<DomInstance>>()
                .ToList() ?? new List<FilterElement<DomInstance>>();

            if (filters.Count == 0)
            {
                return null;
            }

            if (filters.Count == 1)
            {
                return filters[0];
            }

            return new ORFilterElement<DomInstance>(filters.ToArray());
        }
    }
}
