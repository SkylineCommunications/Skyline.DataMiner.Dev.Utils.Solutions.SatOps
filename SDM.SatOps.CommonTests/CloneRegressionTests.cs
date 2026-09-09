namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using System;
    using System.Reflection;

    /// <summary>
    /// Regression tests for the premature required-field validation bug in DOM model *Instance.Clone().
    /// See release note for 0.0.6: "Creating a new Satellite (and equivalents) threw 'SatelliteName is required'
    /// due to premature validation during clone."
    /// </summary>
    [TestClass]
    public class CloneRegressionTests
    {
        private static readonly Assembly CommonAssembly = typeof(SatOpsApi).Assembly;

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.SatellitesInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderSlotsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderReservationsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.BeamsInstance")]
        public void Clone_OnFreshInstance_DoesNotThrow(string instanceTypeName)
        {
            var instanceType = CommonAssembly.GetType(instanceTypeName, throwOnError: true);
            var instance = Activator.CreateInstance(instanceType, nonPublic: true);
            var cloneMethod = instanceType.GetMethod("Clone", BindingFlags.Instance | BindingFlags.Public);

            object clone = null;
            try
            {
                clone = cloneMethod.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail(
                    "Cloning a freshly constructed '{0}' should not throw, but got: {1}",
                    instanceTypeName,
                    ex.InnerException);
            }

            Assert.IsNotNull(clone);
            Assert.IsInstanceOfType(clone, instanceType);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.SatellitesInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderSlotsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderReservationsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.BeamsInstance")]
        public void Duplicate_OnFreshInstance_DoesNotThrow(string instanceTypeName)
        {
            var instanceType = CommonAssembly.GetType(instanceTypeName, throwOnError: true);
            var instance = Activator.CreateInstance(instanceType, nonPublic: true);
            var duplicateMethod = instanceType.GetMethod("Duplicate", BindingFlags.Instance | BindingFlags.Public);

            object duplicate = null;
            try
            {
                duplicate = duplicateMethod.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail(
                    "Duplicating a freshly constructed '{0}' should not throw, but got: {1}",
                    instanceTypeName,
                    ex.InnerException);
            }

            Assert.IsNotNull(duplicate);
            Assert.IsInstanceOfType(duplicate, instanceType);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam")]
        public void DefaultConstructor_ForApiObject_IsPublicAndDoesNotThrow(string apiTypeName)
        {
            var apiType = CommonAssembly.GetType(apiTypeName, throwOnError: true);
            var constructor = apiType.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(constructor, "'{0}' must expose a public parameterless constructor.", apiTypeName);

            object result = null;
            try
            {
                result = constructor.Invoke(Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail(
                    "Invoking 'new {0}()' should not throw, but got: {1}",
                    apiTypeName,
                    ex.InnerException);
            }

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, apiType);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam")]
        public void DefaultConstructor_YieldsNonEmptyId(string apiTypeName)
        {
            var apiType = CommonAssembly.GetType(apiTypeName, throwOnError: true);
            var apiObject = Activator.CreateInstance(apiType);

            var idProperty = apiType.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(idProperty, "'Id' property not found on '{0}'.", apiTypeName);

            var id = (Guid)idProperty.GetValue(apiObject);
            Assert.AreNotEqual(
                Guid.Empty,
                id,
                "Freshly constructed '{0}' should have a non-empty Id.",
                apiTypeName);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.SatellitesInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderSlotsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderReservationsInstance")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.BeamsInstance")]
        public void Instance_DefaultConstructor_AssignsNonEmptyId(string instanceTypeName)
        {
            var instanceType = CommonAssembly.GetType(instanceTypeName, throwOnError: true);
            var instance = Activator.CreateInstance(instanceType, nonPublic: true);
            var idProperty = instanceType.GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(idProperty, "'ID' property not found on '{0}'.", instanceTypeName);

            var domInstanceId = idProperty.GetValue(instance);
            Assert.IsNotNull(domInstanceId, "'ID' should be assigned on '{0}'.", instanceTypeName);

            var innerId = (Guid)domInstanceId.GetType().GetProperty("Id").GetValue(domInstanceId);
            Assert.AreNotEqual(
                Guid.Empty,
                innerId,
                "'{0}' created via default constructor should have a non-empty DomInstanceId.",
                instanceTypeName);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", "Abbreviation", "TS")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", "Name", "TP-1")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", "Name", "Plan-1")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", "Name", "Slot-1")]
        [DataRow("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam", "Name", "Beam-1")]
        public void SetterValueIsVisibleToGetter_OnFreshInstance(string apiTypeName, string propertyName, object value)
        {
            var apiType = CommonAssembly.GetType(apiTypeName, throwOnError: true);
            var apiObject = Activator.CreateInstance(apiType);

            var property = apiType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(property, "Property '{0}' not found on '{1}'.", propertyName, apiTypeName);

            property.SetValue(apiObject, value);
            var readBack = property.GetValue(apiObject);

            Assert.AreEqual(
                value,
                readBack,
                "Property '{0}' on freshly initialized '{1}' should reflect the value set via the setter.",
                propertyName,
                apiTypeName);
        }
    }
}
