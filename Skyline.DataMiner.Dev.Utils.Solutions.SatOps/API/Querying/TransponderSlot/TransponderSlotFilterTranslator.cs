namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using DomModel = DOM.Model;

    internal class TransponderSlotFilterTranslator : DomInstanceFilterTranslator<TransponderSlot>
    {
        private readonly FilterElement<DomInstance> transponderSlotDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.TransponderSlots.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderSlotExposers.TransponderPlan.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.TransponderPlan), comparer, (Guid)value),
            [TransponderSlotExposers.SlotName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.SlotName), comparer, (string)value),
            [TransponderSlotExposers.SlotStartFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.SlotStartFrequency), comparer, (double)value),
            [TransponderSlotExposers.SlotEndFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.SlotEndFrequency), comparer, (double)value),
            [TransponderSlotExposers.Bandwidth.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.Bandwidth), comparer, (double)value),
            [TransponderSlotExposers.UplinkFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.UplinkFreq), comparer, (double)value),
            [TransponderSlotExposers.DownlinkFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderSlot.DownlinkFreq), comparer, (double)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderSlotDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;
    }
}
