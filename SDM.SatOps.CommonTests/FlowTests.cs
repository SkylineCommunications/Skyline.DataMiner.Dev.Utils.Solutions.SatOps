namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
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

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", null);
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var resolver = BuildNullResolverDelegate(transponderType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenTransponderDoesNotExist_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var resolver = BuildNullResolverDelegate(transponderType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenDefaultSlotSizeIsNotPositive_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)0.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)false);
                    SetNestedProperty(dom, "TransponderPlan.StartTime", (DateTime?)DateTime.UtcNow);
                    SetNestedProperty(dom, "TransponderPlan.EndTime", (DateTime?)DateTime.UtcNow.AddHours(1));
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var resolver = BuildNullResolverDelegate(transponderType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenPermanentPlanAlreadyExists_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var existingPlanId = Guid.NewGuid();
            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)transponderId);
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)true);
                });
            SetApiObjectId(transponderPlan, planId);

            var existingPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-Existing");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)transponderId);
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)true);
                });
            SetApiObjectId(existingPlan, existingPlanId);

            var transponderResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var transponderResolver = BuildConstantResolverDelegate(transponderType, transponderResolverType, FormatterServices.GetUninitializedObject(transponderType));

            var planCollectionType = typeof(IEnumerable<>).MakeGenericType(transponderPlanType);
            var plansResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), planCollectionType);
            var plansResolver = BuildEnumerableResolverDelegate(transponderPlanType, plansResolverType, existingPlan);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { transponderResolver, plansResolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenTimeRangeOverlaps_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var existingPlanId = Guid.NewGuid();
            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)transponderId);
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)false);
                    SetNestedProperty(dom, "TransponderPlan.StartTime", (DateTime?)DateTime.UtcNow.AddHours(1));
                    SetNestedProperty(dom, "TransponderPlan.EndTime", (DateTime?)DateTime.UtcNow.AddHours(3));
                });
            SetApiObjectId(transponderPlan, planId);

            var existingPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-Existing");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)transponderId);
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)false);
                    SetNestedProperty(dom, "TransponderPlan.StartTime", (DateTime?)DateTime.UtcNow.AddHours(2));
                    SetNestedProperty(dom, "TransponderPlan.EndTime", (DateTime?)DateTime.UtcNow.AddHours(4));
                });
            SetApiObjectId(existingPlan, existingPlanId);

            var transponderResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var transponderResolver = BuildConstantResolverDelegate(transponderType, transponderResolverType, FormatterServices.GetUninitializedObject(transponderType));

            var planCollectionType = typeof(IEnumerable<>).MakeGenericType(transponderPlanType);
            var plansResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), planCollectionType);
            var plansResolver = BuildEnumerableResolverDelegate(transponderPlanType, plansResolverType, existingPlan);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { transponderResolver, plansResolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenTransponderPlanMissing_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", null);
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)0.1);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var resolver = BuildNullResolverDelegate(transponderPlanType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenTransponderPlanDoesNotExist_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)0.1);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var resolver = BuildNullResolverDelegate(transponderPlanType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenStepSizeLessThanBandwidth_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)2.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var resolver = BuildConstantResolverDelegate(transponderPlanType, resolverType, FormatterServices.GetUninitializedObject(transponderPlanType));
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenBandwidthIsDuplicate_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanId = Guid.NewGuid();
            var rowId = Guid.NewGuid();
            var existingRowId = Guid.NewGuid();
            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)transponderPlanId);
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });
            SetApiObjectId(transponderPlanRow, rowId);

            var existingRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)transponderPlanId);
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)1.5);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });
            SetApiObjectId(existingRow, existingRowId);

            var planResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var planResolver = BuildConstantResolverDelegate(transponderPlanType, planResolverType, FormatterServices.GetUninitializedObject(transponderPlanType));

            var rowCollectionType = typeof(IEnumerable<>).MakeGenericType(transponderPlanRowType);
            var rowsResolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), rowCollectionType);
            var rowsResolver = BuildEnumerableResolverDelegate(transponderPlanRowType, rowsResolverType, existingRow);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { planResolver, rowsResolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderSlotValidationFlow_WhenSlotNameMissing_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderSlotType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderSlotValidationMiddleware", throwOnError: true);

            var transponderSlot = CreateApiObject(
                transponderSlotType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderSlotsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderSlot.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderSlot.SlotName", null);
                    SetNestedProperty(dom, "TransponderSlot.SlotStartFrequency", (double?)1.0);
                    SetNestedProperty(dom, "TransponderSlot.SlotEndFrequency", (double?)2.0);
                    SetNestedProperty(dom, "TransponderSlot.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderSlot.UplinkFreq", (double?)14.0);
                    SetNestedProperty(dom, "TransponderSlot.DownlinkFreq", (double?)11.0);
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var resolver = BuildNullResolverDelegate(transponderPlanType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderSlotType, typeof(Func<,>).MakeGenericType(transponderSlotType, transponderSlotType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderSlot, BuildIdentityDelegate(transponderSlotType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderSlot", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderSlotValidationFlow_WhenTransponderPlanDoesNotExist_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderSlotType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderSlotValidationMiddleware", throwOnError: true);

            var transponderSlot = CreateApiObject(
                transponderSlotType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderSlotsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderSlot.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderSlot.SlotName", "Slot-1");
                    SetNestedProperty(dom, "TransponderSlot.SlotStartFrequency", (double?)1.0);
                    SetNestedProperty(dom, "TransponderSlot.SlotEndFrequency", (double?)2.0);
                    SetNestedProperty(dom, "TransponderSlot.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderSlot.UplinkFreq", (double?)14.0);
                    SetNestedProperty(dom, "TransponderSlot.DownlinkFreq", (double?)11.0);
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderPlanType);
            var resolver = BuildNullResolverDelegate(transponderPlanType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderSlotType, typeof(Func<,>).MakeGenericType(transponderSlotType, transponderSlotType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderSlot, BuildIdentityDelegate(transponderSlotType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderSlot", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderRangeReservationValidationFlow_WhenTransponderMissing_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderRangeReservationValidationMiddleware", throwOnError: true);

            var reservation = CreateApiObject(
                reservationType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderReservationsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderReservation.Transponder", null);
                    SetNestedProperty(dom, "TransponderReservation.RelativeStartFrequency", (double?)10.0);
                    SetNestedProperty(dom, "TransponderReservation.RelativeEndFrequency", (double?)20.0);
                    SetNestedProperty(dom, "TransponderReservation.StartTime", (DateTime?)DateTime.UtcNow);
                    SetNestedProperty(dom, "TransponderReservation.EndTime", (DateTime?)DateTime.UtcNow.AddHours(1));
                    SetNestedProperty(dom, "TransponderReservation.ReservationName", "Reservation-1");
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var resolver = BuildNullResolverDelegate(transponderType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { reservationType, typeof(Func<,>).MakeGenericType(reservationType, reservationType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, reservation, BuildIdentityDelegate(reservationType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderRangeReservationValidationFlow_WhenTimeWindowInvalid_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderRangeReservationValidationMiddleware", throwOnError: true);

            var reservation = CreateApiObject(
                reservationType,
                "Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TransponderReservationsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderReservation.Transponder", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderReservation.RelativeStartFrequency", (double?)10.0);
                    SetNestedProperty(dom, "TransponderReservation.RelativeEndFrequency", (double?)20.0);
                    SetNestedProperty(dom, "TransponderReservation.StartTime", (DateTime?)DateTime.UtcNow.AddHours(1));
                    SetNestedProperty(dom, "TransponderReservation.EndTime", (DateTime?)DateTime.UtcNow);
                    SetNestedProperty(dom, "TransponderReservation.ReservationName", "Reservation-2");
                });

            var resolverType = typeof(Func<,>).MakeGenericType(typeof(Guid), transponderType);
            var resolver = BuildNullResolverDelegate(transponderType, resolverType);
            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { resolver }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { reservationType, typeof(Func<,>).MakeGenericType(reservationType, reservationType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, reservation, BuildIdentityDelegate(reservationType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
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

        private static void SetApiObjectId(object apiObject, Guid id)
        {
            SetPrivateFieldRecursive(apiObject, "<Id>k__BackingField", id);
        }

        private static void SetPrivateFieldRecursive(object target, string fieldName, object value)
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

            throw new InvalidOperationException($"Field '{fieldName}' was not found on type '{target.GetType().FullName}'.");
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

        private static Delegate BuildConstantResolverDelegate(Type returnType, Type delegateType, object value)
        {
            var parameter = Expression.Parameter(typeof(Guid), "id");
            var body = Expression.Constant(value, returnType);
            return Expression.Lambda(delegateType, body, parameter).Compile();
        }

        private static Delegate BuildEnumerableResolverDelegate(Type itemType, Type delegateType, params object[] items)
        {
            var typedArray = Array.CreateInstance(itemType, items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                typedArray.SetValue(items[i], i);
            }

            var parameter = Expression.Parameter(typeof(Guid), "id");
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
            var body = Expression.Constant(typedArray, enumerableType);
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
