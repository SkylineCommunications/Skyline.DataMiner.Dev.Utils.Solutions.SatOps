namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying
{
    using System;

    internal static class UniversalComparer
    {
        internal static int Compare<T>(T x, T y)
        {
            if (Object.Equals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            Type type = typeof(T);
            Type underlying = Nullable.GetUnderlyingType(type);

            if (underlying != null)
            {
                // Nullable value type like int?
                object xValue = Convert.ChangeType(x, underlying);
                object yValue = Convert.ChangeType(y, underlying);

                return ((IComparable)xValue).CompareTo(yValue);
            }

            if (x is IComparable<T> comparableT)
            {
                return comparableT.CompareTo(y);
            }

            if (x is IComparable comparable)
            {
                return comparable.CompareTo(y);
            }

            throw new ArgumentException("Type " + type.FullName + " does not implement IComparable or IComparable<T>");
        }

        internal static bool Equals<T>(T x, T y)
        {
            if (Object.Equals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x is IEquatable<T> equatableT)
            {
                return equatableT.Equals(y);
            }

            // Fall back to Object.Equals if no better option is available
            return x.Equals(y);
        }
    }
}
