namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Regression tests for SatOps Common 0.0.9:
    /// <c>TransponderRangeReservationRepository</c> derives the reservation's <c>SatelliteName</c>
    /// from the selected transponder's <c>TransponderSatellite</c> relationship. The transponder
    /// is the single source of truth for the satellite, so caller-supplied <c>SatelliteName</c>
    /// values are overwritten and a missing satellite relationship produces a clear validation
    /// error instead of a silently omitted capability.
    /// </summary>
    [TestClass]
    public class TransponderRangeReservationSatelliteDerivationTests
    {
        private static readonly Assembly CommonAssembly =
            typeof(Skyline.DataMiner.SDM.SatOps.Common.API.SatOpsApi).Assembly;

        private static readonly Type RepositoryType = CommonAssembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservationRepository",
            throwOnError: true);

        private static readonly Type TransponderType = CommonAssembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder",
            throwOnError: true);

        private static readonly Type SatelliteType = CommonAssembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite",
            throwOnError: true);

        [TestMethod]
        public void ResolveSatelliteName_ReturnsSatelliteNameFromLinkedSatellite()
        {
            var satelliteId = Guid.NewGuid();
            var transponder = BuildTransponder(satelliteId);
            var satellite = BuildSatellite(satelliteId, "Astra-19E");

            var name = InvokeResolve(transponder, id =>
            {
                Assert.AreEqual(satelliteId, id, "The repository must look up the satellite referenced by the transponder.");
                return satellite;
            });

            Assert.AreEqual("Astra-19E", name);
        }

        [TestMethod]
        public void ResolveSatelliteName_WhenTransponderHasNoLinkedSatellite_ThrowsArgumentException()
        {
            var transponder = BuildTransponder(satelliteId: null);

            var exception = InvokeResolveExpectingException(transponder, id =>
            {
                Assert.Fail("The satellite repository must not be queried when TransponderSatellite is missing.");
                return null;
            });

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException, "A missing TransponderSatellite must produce an ArgumentException.");
            Assert.AreEqual("reservation", argumentException.ParamName);
            StringAssert.Contains(argumentException.Message, "no linked satellite");
        }

        [TestMethod]
        public void ResolveSatelliteName_WhenTransponderSatelliteIsEmptyGuid_ThrowsArgumentException()
        {
            var transponder = BuildTransponder(satelliteId: Guid.Empty);

            var exception = InvokeResolveExpectingException(transponder, id => null);

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
        }

        [TestMethod]
        public void ResolveSatelliteName_WhenSatelliteCannotBeResolved_ThrowsArgumentException()
        {
            var satelliteId = Guid.NewGuid();
            var transponder = BuildTransponder(satelliteId);

            var exception = InvokeResolveExpectingException(transponder, id => null);

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
            StringAssert.Contains(argumentException.Message, "could not be resolved");
        }

        [TestMethod]
        public void ResolveSatelliteName_WhenSatelliteHasNoName_ThrowsArgumentException()
        {
            var satelliteId = Guid.NewGuid();
            var transponder = BuildTransponder(satelliteId);
            var satellite = BuildSatellite(satelliteId, name: "  ");

            var exception = InvokeResolveExpectingException(transponder, id => satellite);

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
            StringAssert.Contains(argumentException.Message, "has no name");
        }

        private static string InvokeResolve(object transponder, Func<Guid, object> lookup)
        {
            var method = GetResolveMethod();
            var lookupDelegate = BuildLookupDelegate(lookup);
            return (string)method.Invoke(null, new[] { transponder, lookupDelegate });
        }

        private static Exception InvokeResolveExpectingException(object transponder, Func<Guid, object> lookup)
        {
            var method = GetResolveMethod();
            var lookupDelegate = BuildLookupDelegate(lookup);
            try
            {
                method.Invoke(null, new[] { transponder, lookupDelegate });
                Assert.Fail("Expected ResolveSatelliteNameFromTransponder to throw.");
                return null;
            }
            catch (TargetInvocationException tie)
            {
                return tie.InnerException;
            }
        }

        private static MethodInfo GetResolveMethod()
        {
            var method = RepositoryType.GetMethod(
                "ResolveSatelliteNameFromTransponder",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(method, "TransponderRangeReservationRepository.ResolveSatelliteNameFromTransponder helper is missing.");
            return method;
        }

        private static Delegate BuildLookupDelegate(Func<Guid, object> lookup)
        {
            // Build Func<Guid, Satellite> that calls our Func<Guid, object> and casts the result.
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(Guid), SatelliteType);
            var idParameter = System.Linq.Expressions.Expression.Parameter(typeof(Guid), "id");
            var lookupConstant = System.Linq.Expressions.Expression.Constant(lookup);
            var invokeLookup = System.Linq.Expressions.Expression.Invoke(lookupConstant, idParameter);
            var cast = System.Linq.Expressions.Expression.Convert(invokeLookup, SatelliteType);
            return System.Linq.Expressions.Expression.Lambda(delegateType, cast, idParameter).Compile();
        }

        private static object BuildTransponder(Guid? satelliteId)
        {
            var domInstanceType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance", throwOnError: true);
            var original = Activator.CreateInstance(domInstanceType);
            var updated = Activator.CreateInstance(domInstanceType);

            if (satelliteId.HasValue)
            {
                SetNestedProperty(updated, "Transponder.TransponderSatellite", (Guid?)satelliteId.Value);
            }
            else
            {
                // Ensure updatedInstance.Transponder is instantiated so the property getter returns null (not NRE).
                SetNestedProperty(updated, "Transponder.TransponderName", (string)null);
            }

            var transponder = FormatterServices.GetUninitializedObject(TransponderType);
            SetPrivateField(transponder, "originalInstance", original);
            SetPrivateField(transponder, "updatedInstance", updated);
            return transponder;
        }

        private static object BuildSatellite(Guid id, string name)
        {
            var domInstanceType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SatellitesInstance", throwOnError: true);
            var original = Activator.CreateInstance(domInstanceType);
            var updated = Activator.CreateInstance(domInstanceType);

            SetNestedProperty(updated, "General.SatelliteName", name);

            var satellite = FormatterServices.GetUninitializedObject(SatelliteType);
            SetPrivateField(satellite, "originalInstance", original);
            SetPrivateField(satellite, "updatedInstance", updated);
            SetIdBackingField(satellite, id);
            return satellite;
        }

        private static void SetNestedProperty(object target, string path, object value)
        {
            var parts = path.Split('.');
            object current = target;

            for (int index = 0; index < parts.Length - 1; index++)
            {
                var property = current.GetType().GetProperty(parts[index], BindingFlags.Instance | BindingFlags.Public);
                Assert.IsNotNull(property, $"Property '{parts[index]}' not found on '{current.GetType().FullName}'.");
                var currentValue = property.GetValue(current);
                if (currentValue == null)
                {
                    currentValue = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(current, currentValue);
                }

                current = currentValue;
            }

            var leafProperty = current.GetType().GetProperty(parts[parts.Length - 1], BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(leafProperty, $"Property '{parts[parts.Length - 1]}' not found on '{current.GetType().FullName}'.");
            leafProperty.SetValue(current, value);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var currentType = target.GetType();
            while (currentType != null)
            {
                var field = currentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }

                currentType = currentType.BaseType;
            }

            Assert.Fail($"Private field '{fieldName}' not found on '{target.GetType().FullName}'.");
        }

        private static void SetIdBackingField(object apiObject, Guid id)
        {
            var currentType = apiObject.GetType();
            while (currentType != null)
            {
                var field = currentType.GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(apiObject, id);
                    return;
                }

                currentType = currentType.BaseType;
            }
        }
    }
}
