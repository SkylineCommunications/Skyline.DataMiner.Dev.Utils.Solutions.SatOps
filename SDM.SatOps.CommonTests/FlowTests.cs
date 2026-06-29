namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    [TestClass]
    public class FlowTests
    {
        private static readonly Assembly CommonAssembly = typeof(Skyline.DataMiner.SDM.SatOps.Common.API.SatOpsApi).Assembly;

        [TestMethod]
        public void SatelliteValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.SatelliteValidationMiddleware", throwOnError: true);

            var satellite = CreateApiObject(
                satelliteType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SatellitesInstance",
                null);

            var middleware = Activator.CreateInstance(middlewareType, nonPublic: true);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { satelliteType, typeof(Func<,>).MakeGenericType(satelliteType, satelliteType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, satellite, BuildIdentityDelegate(satelliteType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("satellite", argumentException.ParamName);
        }

        [TestMethod]
        public void BeamValidationFlow_WhenSatelliteDoesNotExist_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var beamType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.BeamValidationMiddleware", throwOnError: true);

            var beam = CreateApiObject(
                beamType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.BeamsInstance",
                dom => SetNestedProperty(dom, "Beam.BeamSatellite", (Guid?)Guid.NewGuid()));

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), satelliteType);
            var resolver = BuildNullResolverDelegate(satelliteType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { beamType, typeof(Func<,>).MakeGenericType(beamType, beamType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, beam, BuildIdentityDelegate(beamType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("beam", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderValidationMiddleware", throwOnError: true);

            var transponder = CreateApiObject(
                transponderType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance",
                dom =>
                {
                    SetNestedProperty(dom, "Transponder.TransponderName", null);
                    SetNestedProperty(dom, "Transponder.TransponderSatellite", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "Transponder.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "Transponder.StartFrequency", (double?)2.0);
                    SetNestedProperty(dom, "Transponder.StopFrequency", (double?)3.0);
                    SetNestedProperty(dom, "Transponder.DownlinkStartFreq", (double?)4.0);
                    SetNestedProperty(dom, "Transponder.DownlinkEndFreq", (double?)5.0);
                    SetNestedProperty(dom, "Transponder.HardEndDate", (DateTime?)DateTime.UtcNow.AddDays(1));
                    SetNestedProperty(dom, "Transponder.DOMResource", (Guid?)Guid.NewGuid());
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), satelliteType);
            var resolver = BuildNullResolverDelegate(satelliteType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderType, typeof(Func<,>).MakeGenericType(transponderType, transponderType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponder, BuildIdentityDelegate(transponderType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponder", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenSatelliteDoesNotExist_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderValidationMiddleware", throwOnError: true);

            var transponder = CreateApiObject(
                transponderType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance",
                dom =>
                {
                    SetNestedProperty(dom, "Transponder.TransponderName", "TP-1");
                    SetNestedProperty(dom, "Transponder.TransponderSatellite", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "Transponder.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "Transponder.StartFrequency", (double?)2.0);
                    SetNestedProperty(dom, "Transponder.StopFrequency", (double?)3.0);
                    SetNestedProperty(dom, "Transponder.DownlinkStartFreq", (double?)4.0);
                    SetNestedProperty(dom, "Transponder.DownlinkEndFreq", (double?)5.0);
                    SetNestedProperty(dom, "Transponder.HardEndDate", (DateTime?)DateTime.UtcNow.AddDays(1));
                    SetNestedProperty(dom, "Transponder.DOMResource", (Guid?)Guid.NewGuid());
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), satelliteType);
            var resolver = BuildNullResolverDelegate(satelliteType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderType, typeof(Func<,>).MakeGenericType(transponderType, transponderType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponder, BuildIdentityDelegate(transponderType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponder", argumentException.ParamName);
        }

        private static object CreateApiObject(Type apiType, string domInstanceTypeName, Action<object> configureOriginalDom)
        {
            var domInstanceType = CommonAssembly.GetType(domInstanceTypeName, throwOnError: true);
            var apiObject = FormatterServices.GetUninitializedObject(apiType);

            var originalInstance = Activator.CreateInstance(domInstanceType);
            configureOriginalDom?.Invoke(originalInstance);
            var updatedInstance = Activator.CreateInstance(domInstanceType);

            SetPrivateField(apiObject, "originalInstance", originalInstance);
            SetPrivateField(apiObject, "updatedInstance", updatedInstance);

            return apiObject;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        private static void SetNestedProperty(object target, string path, object value)
        {
            var parts = path.Split('.');
            object current = target;

            for (int index = 0; index < parts.Length - 1; index++)
            {
                var property = current.GetType().GetProperty(parts[index], BindingFlags.Instance | BindingFlags.Public);
                var currentValue = property.GetValue(current);
                if (currentValue == null)
                {
                    currentValue = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(current, currentValue);
                }

                current = currentValue;
            }

            var leafProperty = current.GetType().GetProperty(parts[parts.Length - 1], BindingFlags.Instance | BindingFlags.Public);
            leafProperty.SetValue(current, value);
        }

        private static Delegate BuildIdentityDelegate(Type apiType)
        {
            var parameter = Expression.Parameter(apiType, "value");
            var delegateType = typeof(Func<,>).MakeGenericType(apiType, apiType);
            return Expression.Lambda(delegateType, parameter, parameter).Compile();
        }

        private static Delegate BuildNullResolverDelegate(Type returnType, Type delegateType)
        {
            var parameter = Expression.Parameter(typeof(Guid), "id");
            var body = Expression.Constant(null, returnType);
            return Expression.Lambda(delegateType, body, parameter).Compile();
        }

        private static Exception InvokeAndUnwrap(MethodInfo method, object target, object value, Delegate next)
        {
            try
            {
                method.Invoke(target, new object[] { value, next });
                Assert.Fail("Expected an exception to be thrown.");
                return null;
            }
            catch (TargetInvocationException invocationException)
            {
                Assert.IsNotNull(invocationException.InnerException);
                return invocationException.InnerException;
            }
        }
    }
}
