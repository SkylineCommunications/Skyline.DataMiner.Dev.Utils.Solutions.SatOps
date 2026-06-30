namespace Skyline.DataMiner.SDM.SatOps.Common.Logging
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

        public const string CannotCreateExistingTransponderPlan = "Cannot create an existing transponder plan.";
        public const string CannotUpdateNonExistingTransponderPlan = "Cannot update a transponder plan that does not exist.";
        public const string TransponderPlanWithIdWasNotFound = "Transponder plan with id '{0}' was not found.";

        public const string SatelliteNameIsRequired = "SatelliteName is required.";
        public const string SatelliteAbbreviationIsRequired = "SatelliteAbbreviation is required.";
        public const string BeamSatelliteIsRequired = "BeamSatellite is required.";
        public const string BeamSatelliteDoesNotExist = "Beam satellite with id '{0}' does not exist.";
    }
}
