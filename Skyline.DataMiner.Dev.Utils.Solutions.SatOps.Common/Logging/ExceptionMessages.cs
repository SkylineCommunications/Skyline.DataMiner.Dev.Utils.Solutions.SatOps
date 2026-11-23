namespace Skyline.DataMiner.Solutions.SatOps.Common.Logging
{
    internal static class ExceptionMessages
    {
        public const string ValueCannotBeEmptyGuid = "The value cannot be an empty GUID.";
        public const string CollectionCannotContainNullItems = "Collection cannot contain null items.";
        public const string CollectionCannotContainEmptyGuidValues = "Collection cannot contain empty GUID values.";

        public const string CannotCreateExistingBeam = "Cannot create an existing beam.";
        public const string CannotUpdateNonExistingBeam = "Cannot update a beam that does not exist.";
        public const string BeamWithIdWasNotFound = "Beam with id '{0}' was not found.";

        public const string CannotCreateExistingSatellite = "Cannot create an existing satellite.";
        public const string CannotUpdateNonExistingSatellite = "Cannot update a satellite that does not exist.";
        public const string SatelliteWithIdWasNotFound = "Satellite with id '{0}' was not found.";

        public const string CannotCreateExistingTransponder = "Cannot create an existing transponder.";
        public const string CannotUpdateNonExistingTransponder = "Cannot update a transponder that does not exist.";
        public const string TransponderWithIdWasNotFound = "Transponder with id '{0}' was not found.";
        public const string TransponderNameAlreadyExists = "A transponder with name '{0}' already exists.";
        public const string DuplicateTransponderNameDetected = "Duplicate transponder name '{0}' detected within the same batch.";
        public const string TransponderResourceNameAlreadyExists = "A resource with name '{0}' already exists. Transponder resource names must be unique.";

        public const string CannotCreateExistingTransponderPlan = "Cannot create an existing transponder plan.";
        public const string CannotUpdateNonExistingTransponderPlan = "Cannot update a transponder plan that does not exist.";
        public const string TransponderPlanWithIdWasNotFound = "Transponder plan with id '{0}' was not found.";

        public const string CannotCreateExistingTransponderPlanRow = "Cannot create an existing transponder plan row.";
        public const string CannotUpdateNonExistingTransponderPlanRow = "Cannot update a transponder plan row that does not exist.";

        public const string CannotCreateExistingTransponderSlot = "Cannot create an existing transponder slot.";
        public const string CannotCreateExistingTransponderRangeReservation = "Cannot create an existing transponder range reservation.";
        public const string CannotUpdateNonExistingTransponderRangeReservation = "Cannot update a transponder range reservation that does not exist.";
        public const string ReservationIsNotValidTransponderRangeReservation = "The specified reservation is not a valid transponder range reservation.";
        public const string TransponderPlanHasNoAssociatedTransponder = "Transponder plan with id '{0}' has no associated transponder configured.";
        public const string TransponderForPlanWasNotFound = "Transponder with id '{0}' referenced by transponder plan '{1}' was not found.";
        public const string SlotOverlapDetected = "Slot '{0}' overlaps with slot '{1}' in the same transponder plan.";
        public const string DuplicateSlotNameDetected = "Duplicate slot name '{0}' detected within the same transponder plan.";
        public const string ReservationOverlapDetected = "Reservation '{0}' overlaps with reservation '{1}' for the same transponder.";
        public const string TransponderPlanNameIsRequired = "Plan name is required.";
        public const string TransponderPlanDefaultSlotSizeIsRequired = "Default slot size is required.";
        public const string TransponderPlanDefaultSlotSizeMustBeGreaterThanZero = "Default slot size must be greater than zero.";
        public const string TransponderPlanTransponderIsRequired = "Transponder is required.";
        public const string TransponderWithIdDoesNotExist = "Transponder with id '{0}' does not exist.";
        public const string NonPermanentTransponderPlanStartTimeIsRequired = "Start time is required for non-permanent transponder plans.";
        public const string NonPermanentTransponderPlanEndTimeIsRequired = "End time is required for non-permanent transponder plans.";
        public const string NonPermanentTransponderPlanTimeWindowInvalid = "End time must be greater than start time for non-permanent transponder plans.";
        public const string PermanentTransponderPlanAlreadyExists = "A permanent transponder plan already exists for transponder '{0}'.";
        public const string TransponderPlanTimeRangeOverlaps = "Transponder plan time range overlaps with existing plan '{0}'.";
        public const string TransponderPlanRowBandwidthIsRequired = "Bandwidth is required.";
        public const string TransponderPlanRowBandwidthMustBeGreaterThanZero = "Bandwidth must be greater than zero.";
        public const string TransponderPlanRowStepSizeIsRequired = "Step Size is required.";
        public const string TransponderPlanRowStepSizeMustBeGreaterThanZero = "Step Size must be greater than zero.";
        public const string TransponderPlanRowStepSizeMustBeGreaterThanOrEqualToBandwidth = "Step Size must be greater than or equal to Bandwidth.";
        public const string DuplicateTransponderPlanRowBandwidthDetected = "Duplicate bandwidth '{0}' detected within transponder plan '{1}'.";

        public const string SatelliteNameIsRequired = "Satellite name is required.";
        public const string SatelliteAbbreviationIsRequired = "Satellite abbreviation is required.";
        public const string BeamSatelliteIsRequired = "Beam satellite is required.";
        public const string BeamSatelliteDoesNotExist = "Beam satellite with id '{0}' does not exist.";

        public const string PageSizeNumberException = "Page size must be greater than zero.";
    }
}
