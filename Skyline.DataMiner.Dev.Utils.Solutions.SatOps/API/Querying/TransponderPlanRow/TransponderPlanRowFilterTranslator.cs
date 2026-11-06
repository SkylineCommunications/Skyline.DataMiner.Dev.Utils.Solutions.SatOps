namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using DomModel = DOM.Model;

    internal class TransponderPlanRowFilterTranslator : DomInstanceFilterTranslator<TransponderPlanRow>
    {
        private readonly FilterElement<DomInstance> transponderPlanRowDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.TransponderPlanRows.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderPlanRowExposers.Guid.fieldName] = HandleGuid,
            [TransponderPlanRowExposers.TransponderPlan.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.TransponderPlan), comparer, (Guid)value),
            [TransponderPlanRowExposers.Bandwidth.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Bandwidth), comparer, (double)value),
            [TransponderPlanRowExposers.StepSize.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.StepSize), comparer, (double)value),
            [TransponderPlanRowExposers.Offset.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Offset), comparer, (double)value),
            [TransponderPlanRowExposers.Limit.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Limit), comparer, (double)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderPlanRowDefinitionFilter;

        protected override IReadOnlyDictionary<string, FieldExposer> OrderFields { get; } = new Dictionary<string, FieldExposer>
        {
            [TransponderPlanRowExposers.Guid.fieldName] = DomInstanceExposers.Id,
            [TransponderPlanRowExposers.TransponderPlan.fieldName] = DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.TransponderPlan),
            [TransponderPlanRowExposers.Bandwidth.fieldName] = DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Bandwidth),
            [TransponderPlanRowExposers.StepSize.fieldName] = DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.StepSize),
            [TransponderPlanRowExposers.Offset.fieldName] = DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Offset),
            [TransponderPlanRowExposers.Limit.fieldName] = DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Limit),
        };

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;
    }
}
