namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the transponder resource cleanup selection logic used by
    /// <c>TransponderResourceCreationMiddleware</c> when a transponder is deleted.
    /// Deleting a transponder must also clean up the MediaOps Plan resource that was auto-provisioned
    /// for it (stored in <c>DOMResource</c>). <c>CollectResourceIds</c> selects which resource ids to
    /// deprecate/delete, ignoring transponders without a resource and de-duplicating shared ids.
    /// </summary>
    [TestClass]
    public class TransponderResourceCleanupTests
    {
        private static readonly Assembly CommonAssembly =
            typeof(Skyline.DataMiner.SDM.SatOps.Common.API.SatOpsApi).Assembly;

        private static readonly Type MiddlewareType = CommonAssembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Middleware.TransponderResourceCreationMiddleware",
            throwOnError: true);

        private static readonly Type TransponderType = CommonAssembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder",
            throwOnError: true);

        [TestMethod]
        public void CollectResourceIds_ReturnsResourceIdForEachTransponderWithResource()
        {
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            var result = InvokeCollectResourceIds(BuildTransponder(first), BuildTransponder(second));

            CollectionAssert.AreEquivalent(new[] { first, second }, result);
        }

        [TestMethod]
        public void CollectResourceIds_IgnoresTranspondersWithoutResource()
        {
            var withResource = Guid.NewGuid();

            var result = InvokeCollectResourceIds(
                BuildTransponder(withResource),
                BuildTransponder(null),
                BuildTransponder(Guid.Empty));

            CollectionAssert.AreEqual(new[] { withResource }, result);
        }

        [TestMethod]
        public void CollectResourceIds_DeduplicatesSharedResourceIds()
        {
            var shared = Guid.NewGuid();

            var result = InvokeCollectResourceIds(BuildTransponder(shared), BuildTransponder(shared));

            CollectionAssert.AreEqual(new[] { shared }, result);
        }

        [TestMethod]
        public void CollectResourceIds_WithNoTransponders_ReturnsEmpty()
        {
            var result = InvokeCollectResourceIds();

            Assert.AreEqual(0, result.Count);
        }

        private static List<Guid> InvokeCollectResourceIds(params object[] transponders)
        {
            var method = MiddlewareType.GetMethod(
                "CollectResourceIds",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(method, "TransponderResourceCreationMiddleware.CollectResourceIds helper is missing.");

            var typedArray = Array.CreateInstance(TransponderType, transponders.Length);
            for (var index = 0; index < transponders.Length; index++)
            {
                typedArray.SetValue(transponders[index], index);
            }

            var result = method.Invoke(null, new object[] { typedArray });
            return ((IEnumerable<Guid>)result).ToList();
        }

        private static object BuildTransponder(Guid? domResource)
        {
            var domInstanceType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.TranspondersInstance", throwOnError: true);
            var original = Activator.CreateInstance(domInstanceType);
            var updated = Activator.CreateInstance(domInstanceType);

            if (domResource.HasValue)
            {
                SetNestedProperty(updated, "Transponder.DOMResource", (Guid?)domResource.Value);
            }
            else
            {
                // Ensure updatedInstance.Transponder is instantiated so the DOMResource getter returns null (not NRE).
                SetNestedProperty(updated, "Transponder.TransponderName", (string)null);
            }

            var transponder = FormatterServices.GetUninitializedObject(TransponderType);
            SetPrivateField(transponder, "originalInstance", original);
            SetPrivateField(transponder, "updatedInstance", updated);
            return transponder;
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
    }
}
