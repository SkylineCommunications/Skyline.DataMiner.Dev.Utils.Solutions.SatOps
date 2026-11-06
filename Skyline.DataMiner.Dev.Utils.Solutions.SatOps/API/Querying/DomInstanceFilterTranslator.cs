namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using SLDataGateway.API.Types.Querying;
    using SLDataGateway.API.Querying;

    internal abstract class DomInstanceFilterTranslator<T> : FilterTranslator<T, DomInstance> where T : ApiObject
    {
        protected DomInstanceFilterTranslator()
        {
        }

        protected abstract FilterElement<DomInstance> DomDefinitionFilter { get; }

        protected abstract IReadOnlyDictionary<string, FieldExposer> OrderFields { get; }

        public IQuery<DomInstance> TranslateQuery(IQuery<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var elements = query.Order.Elements.Select(element =>
            {
                if (!OrderFields.TryGetValue(element.Exposer.fieldName, out var exposer))
                {
                    throw new NotSupportedException($"Ordering by '{element.Exposer.fieldName}' is not supported.");
                }

                return element.WithFieldExposer(exposer);
            }).ToList();

            return query.WithFilter(Translate(query.Filter)).WithOrder(new OrderBy(elements)).WithLimit(query.Limit);
        }

        protected static FilterElement<DomInstance> HandleGuid(Comparer comparer, object value)
        {
            return FilterElementFactory.Create(DomInstanceExposers.Id, comparer, (Guid)value);
        }

        public override FilterElement<DomInstance> Translate(FilterElement<T> filter)
        {
            return base.Translate(filter).AND(DomDefinitionFilter);
        }
    }
}
