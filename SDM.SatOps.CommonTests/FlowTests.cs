namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    [TestClass]
    public class FlowTests
    {
        private static readonly Assembly CommonAssembly = typeof(SatOpsApi).Assembly;

        [TestMethod]
        public void SatelliteValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.SatelliteValidationMiddleware", throwOnError: true);

            var satellite = CreateApiObject(
                satelliteType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.SatellitesInstance",
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
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var beamType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.BeamValidationMiddleware", throwOnError: true);

            var beam = CreateApiObject(
                beamType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.BeamsInstance",
                dom => SetNestedProperty(dom, "Beam.BeamSatellite", (Guid?)Guid.NewGuid()));

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildSatelliteRepository(null) }, null);

            var onCreate = middlewareType.GetMethod("OnCreate", new[] { beamType, typeof(Func<,>).MakeGenericType(beamType, beamType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, beam, BuildIdentityDelegate(beamType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("beam", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderValidationMiddleware", throwOnError: true);

            var transponder = CreateApiObject(
                transponderType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance",
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

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildSatelliteRepository(null) }, null);

            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderType, typeof(Func<,>).MakeGenericType(transponderType, transponderType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponder, BuildIdentityDelegate(transponderType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponder", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenSatelliteDoesNotExist_ThrowsArgumentException()
        {
            var satelliteType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderValidationMiddleware", throwOnError: true);

            var transponder = CreateApiObject(
                transponderType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance",
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

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildSatelliteRepository(null) }, null);

            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderType, typeof(Func<,>).MakeGenericType(transponderType, transponderType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponder, BuildIdentityDelegate(transponderType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponder", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenNameAlreadyExists_ThrowsArgumentException()
        {
            var existing = CreateValidTransponder("TP-1", Guid.NewGuid());
            var transponder = CreateValidTransponder("TP-1", Guid.NewGuid());

            var exception = InvokeTransponderOnCreate(transponder, existing);

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponders", argumentException.ParamName);
            StringAssert.Contains(argumentException.Message, "TP-1");
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenNameAlreadyExistsWithDifferentCasing_ThrowsArgumentException()
        {
            var existing = CreateValidTransponder("TP-1", Guid.NewGuid());
            var transponder = CreateValidTransponder("tp-1", Guid.NewGuid());

            var exception = InvokeTransponderOnCreate(transponder, existing);

            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenNameIsUnique_DoesNotThrow()
        {
            var existing = CreateValidTransponder("TP-1", Guid.NewGuid());
            var transponder = CreateValidTransponder("TP-2", Guid.NewGuid());

            var result = InvokeTransponderOnCreateExpectingSuccess(transponder, existing);

            Assert.AreSame(transponder, result);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenUpdatingTransponderWithItsOwnName_DoesNotThrow()
        {
            var id = Guid.NewGuid();
            var existing = CreateValidTransponder("TP-1", id);
            var transponder = CreateValidTransponder("TP-1", id);

            var result = InvokeTransponderOnCreateExpectingSuccess(transponder, existing);

            Assert.AreSame(transponder, result);
        }

        [TestMethod]
        public void TransponderValidationFlow_WhenBatchContainsDuplicateNames_ThrowsArgumentException()
        {
            var first = CreateValidTransponder("TP-1", Guid.NewGuid());
            var second = CreateValidTransponder("TP-1", Guid.NewGuid());

            var transponderType = TransponderType;
            var middleware = CreateTransponderValidationMiddleware();
            var batch = Array.CreateInstance(transponderType, 2);
            batch.SetValue(first, 0);
            batch.SetValue(second, 1);

            var onCreate = middleware.GetType().GetMethod(
                "OnCreate",
                new[]
                {
                    typeof(IEnumerable<>).MakeGenericType(transponderType),
                    typeof(Func<,>).MakeGenericType(
                        typeof(IEnumerable<>).MakeGenericType(transponderType),
                        typeof(IReadOnlyCollection<>).MakeGenericType(transponderType)),
                });

            var exception = InvokeAndUnwrap(onCreate, middleware, batch, BuildBulkIdentityDelegate(transponderType));

            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            StringAssert.Contains(exception.Message, "TP-1");
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenNameMissing_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", null);
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderRepositoryForRead(null), BuildTransponderPlanRepository(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenTransponderDoesNotExist_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderRepositoryForRead(null), BuildTransponderPlanRepository(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenDefaultSlotSizeIsNotPositive_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-1");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)0.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)false);
                    SetNestedProperty(dom, "TransponderPlan.StartTime", (DateTime?)DateTime.UtcNow);
                    SetNestedProperty(dom, "TransponderPlan.EndTime", (DateTime?)DateTime.UtcNow.AddHours(1));
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderRepositoryForRead(null), BuildTransponderPlanRepository(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenPermanentPlanAlreadyExists_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var existingPlanId = Guid.NewGuid();
            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
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
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlan.PlanName", "TP-Plan-Existing");
                    SetNestedProperty(dom, "TransponderPlan.DefaultSlotSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlan.Transponder", (Guid?)transponderId);
                    SetNestedProperty(dom, "TransponderPlan.IsPermanent", (bool?)true);
                });
            SetApiObjectId(existingPlan, existingPlanId);

            var transponderRepository = BuildTransponderRepositoryForRead((Transponder)FormatterServices.GetUninitializedObject(transponderType));
            var plansRepository = BuildTransponderPlanRepository(null, existingPlan);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { transponderRepository, plansRepository }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanValidationFlow_WhenTimeRangeOverlaps_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanValidationMiddleware", throwOnError: true);

            var transponderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var existingPlanId = Guid.NewGuid();
            var transponderPlan = CreateApiObject(
                transponderPlanType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
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
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlansInstance",
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

            var transponderRepository = BuildTransponderRepositoryForRead((Transponder)FormatterServices.GetUninitializedObject(transponderType));
            var plansRepository = BuildTransponderPlanRepository(null, existingPlan);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { transponderRepository, plansRepository }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanType, typeof(Func<,>).MakeGenericType(transponderPlanType, transponderPlanType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlan, BuildIdentityDelegate(transponderPlanType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenTransponderPlanMissing_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", null);
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)0.1);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderPlanRepository(null), BuildTransponderPlanRowRepository() }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenTransponderPlanDoesNotExist_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)0.1);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderPlanRepository(null), BuildTransponderPlanRowRepository() }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenStepSizeLessThanBandwidth_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)Guid.NewGuid());
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)2.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderPlanRepository((TransponderPlan)FormatterServices.GetUninitializedObject(transponderPlanType)), BuildTransponderPlanRowRepository() }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderPlanRowValidationFlow_WhenBandwidthIsDuplicate_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderPlanRowType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow.TransponderPlanRow", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderPlanRowValidationMiddleware", throwOnError: true);

            var transponderPlanId = Guid.NewGuid();
            var rowId = Guid.NewGuid();
            var existingRowId = Guid.NewGuid();
            var transponderPlanRow = CreateApiObject(
                transponderPlanRowType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
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
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderPlanRowsInstance",
                dom =>
                {
                    SetNestedProperty(dom, "TransponderPlanRow.TransponderPlan", (Guid?)transponderPlanId);
                    SetNestedProperty(dom, "TransponderPlanRow.Bandwidth", (double?)1.0);
                    SetNestedProperty(dom, "TransponderPlanRow.StepSize", (double?)1.5);
                    SetNestedProperty(dom, "TransponderPlanRow.Offset", (double?)0.0);
                });
            SetApiObjectId(existingRow, existingRowId);

            var planRepository = BuildTransponderPlanRepository((TransponderPlan)FormatterServices.GetUninitializedObject(transponderPlanType));
            var rowsRepository = BuildTransponderPlanRowRepository(existingRow);

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { planRepository, rowsRepository }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderPlanRowType, typeof(Func<,>).MakeGenericType(transponderPlanRowType, transponderPlanRowType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderPlanRow, BuildIdentityDelegate(transponderPlanRowType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderPlanRow", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderSlotValidationFlow_WhenSlotNameMissing_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderSlotType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderSlotValidationMiddleware", throwOnError: true);

            var transponderSlot = CreateApiObject(
                transponderSlotType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderSlotsInstance",
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

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderPlanRepository(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderSlotType, typeof(Func<,>).MakeGenericType(transponderSlotType, transponderSlotType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderSlot, BuildIdentityDelegate(transponderSlotType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderSlot", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderSlotValidationFlow_WhenTransponderPlanDoesNotExist_ThrowsArgumentException()
        {
            var transponderPlanType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan", throwOnError: true);
            var transponderSlotType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot.TransponderSlot", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderSlotValidationMiddleware", throwOnError: true);

            var transponderSlot = CreateApiObject(
                transponderSlotType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TransponderSlotsInstance",
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

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderPlanRepository(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { transponderSlotType, typeof(Func<,>).MakeGenericType(transponderSlotType, transponderSlotType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, transponderSlot, BuildIdentityDelegate(transponderSlotType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("transponderSlot", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderRangeReservationValidationFlow_WhenTransponderMissing_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderRangeReservationValidationMiddleware", throwOnError: true);

            var reservation = CreateReservation(
                reservationType,
                transponder: (Guid?)null,
                relativeStart: 10.0,
                relativeEnd: 20.0,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow.AddHours(1),
                name: "Reservation-1");

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderRepositoryForRead(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { reservationType, typeof(Func<,>).MakeGenericType(reservationType, reservationType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, reservation, BuildIdentityDelegate(reservationType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
        }

        [TestMethod]
        public void TransponderRangeReservationValidationFlow_WhenTimeWindowInvalid_ThrowsArgumentException()
        {
            var transponderType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);
            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderRangeReservationValidationMiddleware", throwOnError: true);

            var reservation = CreateReservation(
                reservationType,
                transponder: (Guid?)Guid.NewGuid(),
                relativeStart: 10.0,
                relativeEnd: 20.0,
                startTime: DateTime.UtcNow.AddHours(1),
                endTime: DateTime.UtcNow,
                name: "Reservation-2");

            var middleware = Activator.CreateInstance(middlewareType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { BuildTransponderRepositoryForRead(null) }, null);
            var onCreate = middlewareType.GetMethod("OnCreate", new[] { reservationType, typeof(Func<,>).MakeGenericType(reservationType, reservationType) });

            var exception = InvokeAndUnwrap(onCreate, middleware, reservation, BuildIdentityDelegate(reservationType));

            var argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException);
            Assert.AreEqual("reservation", argumentException.ParamName);
        }

        private static object CreateReservation(
            Type reservationType,
            Guid? transponder,
            double? relativeStart,
            double? relativeEnd,
            DateTime? startTime,
            DateTime? endTime,
            string name)
        {
            // TransponderRangeReservation is a POCO (not backed by a DOM instance); construct it
            // directly and set the properties needed for validation.
            var reservation = Activator.CreateInstance(reservationType);

            reservationType.GetProperty("Name").SetValue(reservation, name);
            reservationType.GetProperty("Transponder").SetValue(reservation, transponder);
            reservationType.GetProperty("RelativeStartFrequency").SetValue(reservation, relativeStart);
            reservationType.GetProperty("RelativeEndFrequency").SetValue(reservation, relativeEnd);
            reservationType.GetProperty("StartTime").SetValue(reservation, startTime);
            reservationType.GetProperty("EndTime").SetValue(reservation, endTime);
            reservationType.GetProperty("SlotName")?.SetValue(reservation, "Slot-1");

            return reservation;
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

        private static ISatelliteRepository BuildSatelliteRepository(Satellite satellite)
        {
            var repository = new Mock<ISatelliteRepository>();
            repository.Setup(r => r.Read(It.IsAny<Guid>())).Returns(satellite);

            return repository.Object;
        }

        private static ITransponderRepository BuildTransponderRepositoryForRead(Transponder transponder)
        {
            var repository = new Mock<ITransponderRepository>();
            repository.Setup(r => r.Read(It.IsAny<Guid>())).Returns(transponder);

            return repository.Object;
        }

        private static ITransponderPlanRepository BuildTransponderPlanRepository(TransponderPlan plan, params object[] plansByTransponder)
        {
            var repository = new Mock<ITransponderPlanRepository>();
            repository.Setup(r => r.Read(It.IsAny<Guid>())).Returns(plan);
            repository.Setup(r => r.ReadByTransponder(It.IsAny<Guid>())).Returns(plansByTransponder.Cast<TransponderPlan>().ToList());

            return repository.Object;
        }

        private static ITransponderPlanRowRepository BuildTransponderPlanRowRepository(params object[] rows)
        {
            var repository = new Mock<ITransponderPlanRowRepository>();
            repository
                .Setup(r => r.Read(It.IsAny<Skyline.DataMiner.Net.Messages.SLDataGateway.FilterElement<TransponderPlanRow>>()))
                .Returns(rows.Cast<TransponderPlanRow>().ToList());

            return repository.Object;
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

        private static Type TransponderType =>
            CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder", throwOnError: true);

        private static Type SatelliteType =>
            CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite", throwOnError: true);

        /// <summary>
        /// Creates a transponder API object that satisfies every required-field rule, so that only name uniqueness can fail validation.
        /// </summary>
        private static object CreateValidTransponder(string name, Guid id)
        {
            var transponder = CreateApiObject(
                TransponderType,
                "Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model.TranspondersInstance",
                dom => ConfigureValidTransponderDom(dom, name));

            // The API getters read the updated instance, so it has to carry the same values.
            ConfigureValidTransponderDom(GetPrivateField(transponder, "updatedInstance"), name);
            SetApiObjectId(transponder, id);

            return transponder;
        }

        private static void ConfigureValidTransponderDom(object dom, string name)
        {
            SetNestedProperty(dom, "Transponder.TransponderName", name);
            SetNestedProperty(dom, "Transponder.TransponderSatellite", (Guid?)Guid.NewGuid());
            SetNestedProperty(dom, "Transponder.Bandwidth", (double?)1.0);
            SetNestedProperty(dom, "Transponder.StartFrequency", (double?)2.0);
            SetNestedProperty(dom, "Transponder.StopFrequency", (double?)3.0);
            SetNestedProperty(dom, "Transponder.DownlinkStartFreq", (double?)4.0);
            SetNestedProperty(dom, "Transponder.DownlinkEndFreq", (double?)5.0);
            SetNestedProperty(dom, "Transponder.HardEndDate", (DateTime?)DateTime.UtcNow.AddDays(1));
            SetNestedProperty(dom, "Transponder.DOMResource", (Guid?)Guid.NewGuid());
        }

        private static object CreateTransponderValidationMiddleware(params object[] existingTransponders)
        {
            var middlewareType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware.TransponderNameUniquenessMiddleware", throwOnError: true);
            var transponderRepository = BuildTransponderRepository(existingTransponders);

            return Activator.CreateInstance(
                middlewareType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object[] { transponderRepository },
                null);
        }

        private static Exception InvokeTransponderOnCreate(object transponder, params object[] existingTransponders)
        {
            var middleware = CreateTransponderValidationMiddleware(existingTransponders);
            var onCreate = middleware.GetType().GetMethod("OnCreate", new[] { TransponderType, typeof(Func<,>).MakeGenericType(TransponderType, TransponderType) });

            return InvokeAndUnwrap(onCreate, middleware, transponder, BuildIdentityDelegate(TransponderType));
        }

        private static object InvokeTransponderOnCreateExpectingSuccess(object transponder, params object[] existingTransponders)
        {
            var middleware = CreateTransponderValidationMiddleware(existingTransponders);
            var onCreate = middleware.GetType().GetMethod("OnCreate", new[] { TransponderType, typeof(Func<,>).MakeGenericType(TransponderType, TransponderType) });

            try
            {
                return onCreate.Invoke(middleware, new object[] { transponder, BuildIdentityDelegate(TransponderType) });
            }
            catch (TargetInvocationException invocationException)
            {
                Assert.Fail("Expected no exception, but got: {0}", invocationException.InnerException);
                return null;
            }
        }

        private static object GetPrivateField(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field.GetValue(target);
        }

        /// <summary>
        /// Builds a transponder repository stub that returns the supplied items regardless of the requested names.
        /// The middleware narrows the result down by name itself, so the requested names can be ignored here.
        /// </summary>
        private static ITransponderRepository BuildTransponderRepository(params object[] items)
        {
            var transponders = items.Cast<Transponder>().ToList();
            var repository = new Mock<ITransponderRepository>();
            repository.Setup(r => r.ReadByNames(It.IsAny<IEnumerable<string>>())).Returns(transponders);

            return repository.Object;
        }

        private static Delegate BuildBulkIdentityDelegate(Type apiType)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(apiType);
            var readOnlyCollectionType = typeof(IReadOnlyCollection<>).MakeGenericType(apiType);
            var parameter = Expression.Parameter(enumerableType, "values");
            var toListMethod = typeof(System.Linq.Enumerable).GetMethod("ToList").MakeGenericMethod(apiType);
            var body = Expression.Convert(Expression.Call(toListMethod, parameter), readOnlyCollectionType);
            var delegateType = typeof(Func<,>).MakeGenericType(enumerableType, readOnlyCollectionType);

            return Expression.Lambda(delegateType, body, parameter).Compile();
        }
    }
}
