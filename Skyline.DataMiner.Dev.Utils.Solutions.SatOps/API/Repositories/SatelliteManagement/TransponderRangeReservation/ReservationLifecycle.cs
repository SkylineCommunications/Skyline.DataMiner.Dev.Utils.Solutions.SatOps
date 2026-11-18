namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;

    internal static class ReservationLifecycle
    {
        internal static T Execute<T>(Job job, string operation, Func<T> action)
        {
            try
            {
                return action();
            }
            catch (MediaOpsException exception) when (exception.TraceData.ErrorData.Any(IsLifecycleError))
            {
                var errors = exception.TraceData.ErrorData
                    .Select(error => new ReservationOperationError(MapErrorCode(error), error.ToString()))
                    .ToList();
                throw new ReservationOperationException(
                    job.Id, TransponderRangeReservation.MapState(job.State), operation, errors, exception);
            }
        }

        internal static void Execute(Job job, string operation, Action action)
        {
            Execute<object>(job, operation, () =>
            {
                action();
                return null;
            });
        }

        internal static void EnsureAllocationSettingsCanChange(
            Job job,
            JobResourceNode node,
            string operation,
            double? startFrequency,
            double? endFrequency,
            double? bandwidth,
            string satelliteName,
            bool applySatellite)
        {
            if (job.State != JobState.Running)
            {
                return;
            }

            var errors = new List<ReservationOperationError>();
            var range = node.OrchestrationSettings.Capacities.OfType<RangeCapacitySetting>()
                .FirstOrDefault(setting => setting.Id == PredefinedGuids.TransponderBandwidthGuid);
            if (range?.MinValue != ToDecimal(startFrequency) || range?.MaxValue != ToDecimal(endFrequency))
            {
                errors.Add(new ReservationOperationError(
                    ReservationOperationErrorCode.FrequencyRangeChangeNotAllowed,
                    "The frequency range cannot be changed because the booking is already Active."));
            }

            var size = node.OrchestrationSettings.Configurations.OfType<NumberConfigurationSetting>()
                .FirstOrDefault(setting => setting.Id == PredefinedGuids.BandwidthSizeGuid);
            if (size?.Value != ToDecimal(bandwidth))
            {
                errors.Add(new ReservationOperationError(
                    ReservationOperationErrorCode.BandwidthChangeNotAllowed,
                    "The bandwidth cannot be changed because the booking is already Active."));
            }

            var satellite = node.OrchestrationSettings.Capabilities
                .FirstOrDefault(setting => setting.Id == PredefinedGuids.SatelliteCapabilityGuid);
            if (applySatellite && !string.Equals(
                satellite?.Value,
                string.IsNullOrWhiteSpace(satelliteName) ? null : satelliteName,
                StringComparison.Ordinal))
            {
                errors.Add(new ReservationOperationError(
                    ReservationOperationErrorCode.SatelliteCapabilityChangeNotAllowed,
                    "The satellite capability cannot be changed because the booking is already Active."));
            }

            if (errors.Count != 0)
            {
                throw new ReservationOperationException(
                    job.Id, TransponderReservationState.Active, operation, errors);
            }
        }

        private static decimal? ToDecimal(double? value)
        {
            return value.HasValue ? (decimal?)(decimal)value.Value : null;
        }

        private static bool IsLifecycleError(MediaOpsErrorData error)
        {
            return error is JobInvalidStateError
                || error is JobTimingChangedNotAllowedError
                || error is JobNodeGraphChangedInPostRollNotAllowedError;
        }

        private static ReservationOperationErrorCode MapErrorCode(MediaOpsErrorData error)
        {
            if (error is JobStartChangedNotAllowedError)
                return ReservationOperationErrorCode.StartTimeChangeNotAllowed;
            if (error is JobPreRollStartChangedNotAllowedError)
                return ReservationOperationErrorCode.PreRollStartChangeNotAllowed;
            if (error is JobEndChangedNotAllowedError)
                return ReservationOperationErrorCode.EndTimeChangeNotAllowed;
            if (error is JobPostRollEndChangedNotAllowedError)
                return ReservationOperationErrorCode.PostRollEndChangeNotAllowed;
            if (error is JobInvalidStateError)
                return ReservationOperationErrorCode.InvalidBookingState;
            if (error is JobNodeSwappedInPostRollNotAllowedError)
                return ReservationOperationErrorCode.ResourceReassignmentNotAllowed;
            if (error is JobNodeGraphChangedInPostRollNotAllowedError)
                return ReservationOperationErrorCode.NodeGraphChangeNotAllowed;

            return ReservationOperationErrorCode.MediaOpsValidationFailure;
        }
    }
}
