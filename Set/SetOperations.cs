using System;

namespace SetLibrary
{
    /// <summary>
    /// Provides static methods for extended set operations and utility functions
    /// </summary>
    /// <remarks>
    /// This class contains operations that don't modify the original sets and return new sets instead.
    /// It also provides factory methods for creating common set configurations.
    /// </remarks>
    public static class SetOperations
    {
        /// <summary>
        /// Determines whether one set is a subset of another set
        /// </summary>
        /// <param name="set1">The set to check for being a subset</param>
        /// <param name="set2">The set to check for being a superset</param>
        /// <returns>
        /// true if set1 is a subset of set2; otherwise, false.
        /// Also returns true if both sets are null or if set1 is empty.
        /// </returns>
        /// <example>
        /// <code>
        /// var setA = new Set(new object[] { 1, 2 });
        /// var setB = new Set(new object[] { 1, 2, 3 });
        /// bool result = SetOperations.IsSubset(setA, setB); // returns true
        /// </code>
        /// </example>
        public static bool IsSubset(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                return false;

            foreach (var element in set1)
            {
                if (!set2.Contains(element))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether one set is a proper subset of another set
        /// </summary>
        /// <param name="set1">The set to check for being a proper subset</param>
        /// <param name="set2">The set to check for being a proper superset</param>
        /// <returns>
        /// true if set1 is a proper subset of set2 (subset but not equal); otherwise, false
        /// </returns>
        /// <remarks>
        /// A proper subset means that set1 is contained within set2 but set1 is not equal to set2.
        /// The empty set is a proper subset of every non-empty set.
        /// </remarks>
        public static bool IsProperSubset(Set? set1, Set? set2)
        {
            return IsSubset(set1, set2) && set1!.Count < set2!.Count;
        }

        /// <summary>
        /// Produces the symmetric difference of two sets
        /// </summary>
        /// <param name="set1">The first set</param>
        /// <param name="set2">The second set</param>
        /// <returns>
        /// A set that contains the elements that are in either set1 or set2, but not both
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when set1 or set2 is null</exception>
        /// <remarks>
        /// The symmetric difference is equivalent to: (set1 ∪ set2) - (set1 ∩ set2)
        /// </remarks>
        public static Set SymmetricDifference(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                throw new ArgumentNullException(set1 is null ? nameof(set1) : nameof(set2));

            return (set1 - set2) + (set2 - set1);
        }

        /// <summary>
        /// Determines whether two sets are disjoint (have no elements in common)
        /// </summary>
        /// <param name="set1">The first set to compare</param>
        /// <param name="set2">The second set to compare</param>
        /// <returns>
        /// true if the sets have no elements in common; otherwise, false.
        /// Returns true if either set is null or empty.
        /// </returns>
        public static bool AreDisjoint(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                return true;

            return (set1 * set2).IsEmpty();
        }

        /// <summary>
        /// Creates a new set containing the specified elements
        /// </summary>
        /// <param name="elements">The elements to add to the set</param>
        /// <returns>A new set containing the specified elements</returns>
        /// <example>
        /// <code>
        /// var set = SetOperations.CreateSet(1, "hello", 3.14);
        /// </code>
        /// </example>
        public static Set CreateSet(params object[] elements)
        {
            return new Set(elements);
        }

        /// <summary>
        /// Creates a complex set with nested sets as defined in the assignment example
        /// </summary>
        /// <returns>A set with structure: {a, b, c, {a, b}, {}, {a, {c}}}</returns>
        /// <remarks>
        /// This method creates a specific set configuration used for testing and demonstration purposes.
        /// It shows the capability of the Set class to handle nested sets and complex structures.
        /// </remarks>
        public static Set CreateComplexSet()
        {
            return Set.FromString("{a, b, c, {a, b}, {}, {a, {c}}}");
        }
    }
}