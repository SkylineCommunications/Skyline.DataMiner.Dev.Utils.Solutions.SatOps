namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlan
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using System;
    using System.Collections.Generic;
    using DomModel = DOM.Model;

    internal class TransponderPlanFilterTranslator : DomInstanceFilterTranslator<TransponderPlan>
    {
        private readonly FilterElement<DomInstance> transponderPlanDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.TransponderPlans.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderPlanExposers.PlanId.fieldName] = HandleGuid,
            [TransponderPlanExposers.PlanName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.PlanName), comparer, (string)value),
            [TransponderPlanExposers.StartTime.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.StartTime), comparer, (DateTime)value),
            [TransponderPlanExposers.EndTime.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.EndTime), comparer, (DateTime)value),
            [TransponderPlanExposers.IsPermanent.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.IsPermanent), comparer, (bool)value),
            [TransponderPlanExposers.DefaultSlotSize.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.DefaultSlotSize), comparer, (double)value),
            [TransponderPlanExposers.Transponder.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.TransponderPlan.Transponder), comparer, (Guid)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderPlanDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;
    }
}
