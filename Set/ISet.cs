using System.Collections.Generic;

namespace SetLibrary
{
    /// <summary>
    /// Generic interface representing a mathematical set with standard operations
    /// </summary>
    /// <typeparam name="T">The type of elements in the set</typeparam>
    /// <remarks>
    /// This interface provides basic set operations similar to mathematical sets.
    /// Implementations should ensure that duplicate elements are not allowed.
    /// </remarks>
    public interface ISet<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the set
        /// </summary>
        /// <value>The number of elements in the set</value>
        int Count { get; }

        /// <summary>
        /// Adds an element to the current set
        /// </summary>
        /// <param name="element">The element to add to the set</param>
        /// <exception cref="System.ArgumentNullException">Thrown when element is null</exception>
        void Add(T element);

        /// <summary>
        /// Removes the first occurrence of a specific element from the set
        /// </summary>
        /// <param name="element">The element to remove from the set</param>
        /// <returns>
        /// true if element was successfully removed from the set; 
        /// otherwise, false. This method also returns false if element is not found in the set
        /// </returns>
        bool Remove(T element);

        /// <summary>
        /// Determines whether the set contains a specific element
        /// </summary>
        /// <param name="element">The element to locate in the set</param>
        /// <returns>true if the set contains the element; otherwise, false</returns>
        bool Contains(T element);

        /// <summary>
        /// Determines whether the set is empty (contains no elements)
        /// </summary>
        /// <returns>true if the set contains no elements; otherwise, false</returns>
        bool IsEmpty();

        /// <summary>
        /// Removes all elements from the set
        /// </summary>
        void Clear();

        /// <summary>
        /// Modifies the current set to contain all elements that are present in itself, 
        /// the specified set, or both (set union operation)
        /// </summary>
        /// <param name="other">The set to union with</param>
        /// <remarks>
        /// This operation modifies the current set and does not return a new set.
        /// After this operation, the set contains elements from both sets without duplicates.
        /// </remarks>
        void UnionWith(ISet<T> other);

        /// <summary>
        /// Modifies the current set to contain only elements that are present in both 
        /// itself and the specified set (set intersection operation)
        /// </summary>
        /// <param name="other">The set to intersect with</param>
        /// <remarks>
        /// This operation modifies the current set and does not return a new set.
        /// After this operation, the set contains only elements common to both sets.
        /// </remarks>
        void IntersectWith(ISet<T> other);

        /// <summary>
        /// Removes all elements in the specified set from the current set (set difference operation)
        /// </summary>
        /// <param name="other">The set of elements to remove</param>
        /// <remarks>
        /// This operation modifies the current set and does not return a new set.
        /// After this operation, the set contains only elements that were in the original set
        /// but not in the specified set.
        /// </remarks>
        void ExceptWith(ISet<T> other);

        /// <summary>
        /// Gets a value indicating whether the specified element is in the set
        /// </summary>
        /// <param name="element">The element to locate in the set</param>
        /// <value>true if the set contains the element; otherwise, false</value>
        bool this[T element] { get; }
    }
}