namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using DomModel = DOM.Model;

    internal class TransponderRangeReservationFilterTranslator : DomInstanceFilterTranslator<TransponderRangeReservation>
    {
        private readonly FilterElement<DomInstance> transponderReservationDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal(DomModel.SlcSatellite_ManagementIds.Definitions.TransponderReservations.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderRangeReservationExposers.ReservationName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.ReservationName), comparer, (string)value),
            [TransponderRangeReservationExposers.Transponder.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.Transponder), comparer, (Guid)value),
            [TransponderRangeReservationExposers.RelativeStartFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.RelativeStartFrequency), comparer, (double)value),
            [TransponderRangeReservationExposers.RelativeEndFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.RelativeEndFrequency), comparer, (double)value),
            [TransponderRangeReservationExposers.StartTime.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.StartTime), comparer, (DateTime)value),
            [TransponderRangeReservationExposers.EndTime.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField(DomModel.SlcSatellite_ManagementIds.Sections.TransponderReservation.EndTime), comparer, (DateTime)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderReservationDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;
    }
}
