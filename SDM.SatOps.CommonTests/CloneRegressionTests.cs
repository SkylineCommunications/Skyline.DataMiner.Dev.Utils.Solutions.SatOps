namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Reflection;

    /// <summary>
    /// Regression tests for the premature required-field validation bug in DOM model *Instance.Clone().
    /// See release note for 0.0.6: "Fixed: Satellites.Initialize() (and equivalents) threw 'SatelliteName is required'
    /// due to premature validation during clone."
    /// </summary>
    [TestClass]
    public class CloneRegressionTests
    {
        private static readonly Assembly CommonAssembly = typeof(Skyline.DataMiner.SDM.SatOps.Common.API.SatOpsApi).Assembly;

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SatellitesInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderSlotsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderReservationsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.BeamsInstance")]
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
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SatellitesInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderSlotsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderReservationsInstance")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.BeamsInstance")]
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
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", "CreateNewSatellite")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", "CreateNewTransponder")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", "CreateNewTransponderSlot")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", "CreateNewTransponderPlan")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", "CreateNewTransponderPlanRow")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", "CreateNew")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam", "CreateNewBeam")]
        public void Initialize_ForApiObject_DoesNotThrow(string apiTypeName, string factoryMethodName)
        {
            var apiType = CommonAssembly.GetType(apiTypeName, throwOnError: true);
            var factory = apiType.GetMethod(factoryMethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(factory, "Factory method '{0}' not found on '{1}'.", factoryMethodName, apiTypeName);

            object result = null;
            try
            {
                result = factory.Invoke(null, Array.Empty<object>());
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail(
                    "Invoking '{0}.{1}()' should not throw (Initialize path), but got: {2}",
                    apiTypeName,
                    factoryMethodName,
                    ex.InnerException);
            }

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, apiType);
        }

        [DataTestMethod]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", "CreateNewSatellite", "Name", "TestSat")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", "CreateNewSatellite", "Abbreviation", "TS")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", "CreateNewTransponder", "Name", "TP-1")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", "CreateNewTransponderPlan", "Name", "Plan-1")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", "CreateNewTransponderSlot", "Name", "Slot-1")]
        [DataRow("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam", "CreateNewBeam", "Name", "Beam-1")]
        public void SetterValueIsVisibleToGetter_OnFreshInstance(string apiTypeName, string factoryMethodName, string propertyName, object value)
        {
            var apiType = CommonAssembly.GetType(apiTypeName, throwOnError: true);
            var factory = apiType.GetMethod(factoryMethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var apiObject = factory.Invoke(null, Array.Empty<object>());

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
