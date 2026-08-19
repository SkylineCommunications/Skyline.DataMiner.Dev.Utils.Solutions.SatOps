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
    }
}
