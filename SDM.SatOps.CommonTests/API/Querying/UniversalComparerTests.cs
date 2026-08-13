namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying;

    [TestClass]
    public class UniversalComparerTests
    {
        [TestMethod]
        public void Compare_EqualValues_ReturnsZero()
        {
            Assert.AreEqual(0, UniversalComparer.Compare("a", "a"));
        }

        [TestMethod]
        public void Compare_BothNull_ReturnsZero()
        {
            Assert.AreEqual(0, UniversalComparer.Compare<string>(null, null));
        }

        [TestMethod]
        public void Compare_XNull_ReturnsMinusOne()
        {
            Assert.AreEqual(-1, UniversalComparer.Compare<string>(null, "a"));
        }

        [TestMethod]
        public void Compare_YNull_ReturnsOne()
        {
            Assert.AreEqual(1, UniversalComparer.Compare<string>("a", null));
        }

        [TestMethod]
        public void Compare_NullableValueTypes_ComparesUnderlyingValues()
        {
            Assert.IsTrue(UniversalComparer.Compare<int?>(1, 2) < 0);
            Assert.IsTrue(UniversalComparer.Compare<int?>(3, 2) > 0);
        }

        [TestMethod]
        public void Compare_GenericComparable_UsesGenericCompareTo()
        {
            Assert.IsTrue(UniversalComparer.Compare(1, 2) < 0);
            Assert.IsTrue(UniversalComparer.Compare(new DateTime(2024, 1, 2), new DateTime(2024, 1, 1)) > 0);
        }

        [TestMethod]
        public void Compare_NonGenericComparableOnly_UsesNonGenericCompareTo()
        {
            var x = new OnlyComparable(1);
            var y = new OnlyComparable(2);

            Assert.AreEqual(-1, UniversalComparer.Compare(x, y));
            Assert.AreEqual(1, UniversalComparer.Compare(y, x));
        }

        [TestMethod]
        public void Compare_TypeNotComparable_ThrowsArgumentException()
        {
            var x = new NotComparable();
            var y = new NotComparable();

            Assert.ThrowsException<ArgumentException>(() => UniversalComparer.Compare(x, y));
        }

        [TestMethod]
        public void Equals_EqualValues_ReturnsTrue()
        {
            Assert.IsTrue(UniversalComparer.Equals("a", "a"));
            Assert.IsTrue(UniversalComparer.Equals<string>(null, null));
        }

        [TestMethod]
        public void Equals_OneNull_ReturnsFalse()
        {
            Assert.IsFalse(UniversalComparer.Equals<string>(null, "a"));
            Assert.IsFalse(UniversalComparer.Equals<string>("a", null));
        }

        [TestMethod]
        public void Equals_EquatableImplementation_UsesEquatable()
        {
            var x = new EquatableOnly(5);
            var y = new EquatableOnly(5);
            var z = new EquatableOnly(6);

            Assert.IsTrue(UniversalComparer.Equals(x, y));
            Assert.IsFalse(UniversalComparer.Equals(x, z));
        }

        [TestMethod]
        public void Equals_NoEquatable_FallsBackToObjectEquals()
        {
            var x = new NotComparable();
            var y = new NotComparable();

            Assert.IsFalse(UniversalComparer.Equals(x, y));
        }

        private class OnlyComparable : IComparable
        {
            private readonly int value;

            public OnlyComparable(int value)
            {
                this.value = value;
            }

            public int CompareTo(object obj)
            {
                return value.CompareTo(((OnlyComparable)obj).value);
            }
        }

        private class NotComparable
        {
        }

        private class EquatableOnly : IEquatable<EquatableOnly>
        {
            private readonly int value;

            public EquatableOnly(int value)
            {
                this.value = value;
            }

            public bool Equals(EquatableOnly other)
            {
                return other != null && other.value == value;
            }
        }
    }
}
