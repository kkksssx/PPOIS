using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetLibrary
{
    /// <summary>
    /// Represents a mathematical set that can contain elements of any type including nested sets
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Set class implements standard mathematical set operations including union, intersection, 
    /// difference, and power set generation. It supports nested sets and can contain mixed element types.
    /// </para>
    /// <para>
    /// Each set instance has a unique ID and the class maintains a count of all created sets.
    /// The set uses value-based equality rather than reference equality.
    /// </para>
    /// </remarks>
    public class Set : ISet<object>, IEquatable<Set?>
    {
        private readonly HashSet<object> elements;
        private static int setCount = 0;
        private readonly int id;

        /// <summary>
        /// Gets the number of elements contained in the set
        /// </summary>
        /// <value>The number of elements in the set</value>
        public int Count => elements.Count;

        /// <summary>
        /// Gets the unique identifier of the set
        /// </summary>
        /// <value>The unique integer ID of the set</value>
        public int Id => id;

        /// <summary>
        /// Gets the total number of Set instances created
        /// </summary>
        /// <value>The total count of created Set instances</value>
        public static int SetCount => setCount;

        /// <summary>
        /// Initializes a new empty instance of the Set class
        /// </summary>
        public Set()
        {
            elements = new HashSet<object>();
            setCount++;
            id = setCount;
        }

        /// <summary>
        /// Initializes a new instance of the Set class that contains elements copied from the specified collection
        /// </summary>
        /// <param name="items">The collection whose elements are copied to the new set</param>
        public Set(IEnumerable<object>? items) : this()
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Set class that is a deep copy of the specified set
        /// </summary>
        /// <param name="other">The set to copy</param>
        /// <exception cref="System.ArgumentNullException">Thrown when other is null</exception>
        public Set(Set? other) : this()
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            foreach (var element in other.elements)
            {
                if (element is Set nestedSet)
                {
                    Add(new Set(nestedSet));
                }
                else
                {
                    Add(element);
                }
            }
        }

        /// <summary>
        /// Creates a new Set from a string representation
        /// </summary>
        /// <param name="str">String representation of the set in format "{element1, element2, {nestedSet}, ...}"</param>
        /// <returns>A new Set containing the parsed elements</returns>
        /// <exception cref="System.ArgumentException">Thrown when string format is invalid</exception>
        /// <example>
        /// <code>
        /// var set = Set.FromString("{1, 2, {a, b}}");
        /// </code>
        /// </example>
        public static Set FromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new Set();

            string trimmed = str.Trim();
            if (!trimmed.StartsWith("{") || !trimmed.EndsWith("}"))
                throw new ArgumentException("Invalid set format: must be enclosed in curly braces");

            var set = new Set();
            var content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            if (string.IsNullOrEmpty(content))
                return set;

            ParseElements(content, set);
            return set;
        }

        private static void ParseElements(string content, Set set)
        {
            int depth = 0;
            StringBuilder currentElement = new StringBuilder();

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];

                if (c == '{') depth++;
                if (c == '}') depth--;

                if (c == ',' && depth == 0)
                {
                    AddParsedElement(currentElement.ToString(), set);
                    currentElement.Clear();
                    continue;
                }

                currentElement.Append(c);
            }

            if (currentElement.Length > 0)
            {
                AddParsedElement(currentElement.ToString(), set);
            }
        }

        private static void AddParsedElement(string elementStr, Set set)
        {
            elementStr = elementStr.Trim();
            if (string.IsNullOrEmpty(elementStr))
                return;

            if (elementStr.StartsWith("{") && elementStr.EndsWith("}"))
            {
                set.Add(FromString(elementStr));
            }
            else
            {
                // Убираем возможные кавычки
                if ((elementStr.StartsWith("\"") && elementStr.EndsWith("\"")) ||
                    (elementStr.StartsWith("'") && elementStr.EndsWith("'")))
                {
                    elementStr = elementStr.Substring(1, elementStr.Length - 2);
                }

                // Пробуем разные числовые форматы
                if (int.TryParse(elementStr, out int intValue))
                {
                    set.Add(intValue);
                }
                else if (double.TryParse(elementStr, System.Globalization.NumberStyles.Any,
                                       System.Globalization.CultureInfo.InvariantCulture, out double doubleValue))
                {
                    set.Add(doubleValue);
                }
                else
                {
                    set.Add(elementStr);
                }
            }
        }

        /// <inheritdoc/>
        public void Add(object element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            elements.Add(element);
        }

        /// <inheritdoc/>
        public bool Remove(object element)
        {
            return elements.Remove(element);
        }

        /// <inheritdoc/>
        public bool Contains(object element)
        {
            return elements.Contains(element);
        }

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            return elements.Count == 0;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            elements.Clear();
        }

        /// <inheritdoc/>
        public bool this[object element] => Contains(element);

        /// <summary>
        /// Computes the union of two sets and returns a new set
        /// </summary>
        /// <param name="set1">The first set</param>
        /// <param name="set2">The second set</param>
        /// <returns>A new set that contains all elements from both sets</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when set1 or set2 is null</exception>
        public static Set operator +(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                throw new ArgumentNullException(set1 is null ? nameof(set1) : nameof(set2));

            var result = new Set(set1);
            foreach (var element in set2.elements)
            {
                result.Add(element);
            }
            return result;
        }

        /// <inheritdoc/>
        public void UnionWith(ISet<object> other)
        {
            if (other != null)
            {
                foreach (var element in other)
                {
                    Add(element);
                }
            }
        }

        /// <summary>
        /// Computes the intersection of two sets and returns a new set
        /// </summary>
        /// <param name="set1">The first set</param>
        /// <param name="set2">The second set</param>
        /// <returns>A new set that contains only elements common to both sets</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when set1 or set2 is null</exception>
        public static Set operator *(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                throw new ArgumentNullException(set1 is null ? nameof(set1) : nameof(set2));

            var result = new Set();
            foreach (var element in set1.elements)
            {
                if (set2.Contains(element))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public void IntersectWith(ISet<object> other)
        {
            if (other is Set otherSet)
            {
                var toRemove = new List<object>();
                foreach (var element in elements)
                {
                    if (!otherSet.Contains(element))
                    {
                        toRemove.Add(element);
                    }
                }
                foreach (var element in toRemove)
                {
                    Remove(element);
                }
            }
            else if (other != null)
            {
                // Для других реализаций ISet<object> используем их элементы
                var toRemove = new List<object>();
                foreach (var element in elements)
                {
                    if (!other.Contains(element))
                    {
                        toRemove.Add(element);
                    }
                }
                foreach (var element in toRemove)
                {
                    Remove(element);
                }
            }
            // Если other == null, ничего не делаем
        }

        /// <summary>
        /// Computes the difference between two sets and returns a new set
        /// </summary>
        /// <param name="set1">The set to subtract from</param>
        /// <param name="set2">The set to subtract</param>
        /// <returns>A new set that contains elements from set1 that are not in set2</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when set1 or set2 is null</exception>
        public static Set operator -(Set? set1, Set? set2)
        {
            if (set1 is null || set2 is null)
                throw new ArgumentNullException(set1 is null ? nameof(set1) : nameof(set2));

            var result = new Set();
            foreach (var element in set1.elements)
            {
                if (!set2.Contains(element))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public void ExceptWith(ISet<object> other)
        {
            if (other is Set otherSet)
            {
                foreach (var element in otherSet.elements)
                {
                    Remove(element);
                }
            }
            else if (other != null)
            {
                foreach (var element in other)
                {
                    Remove(element);
                }
            }
            // Если other == null, ничего не делаем
        }

        /// <summary>
        /// Determines whether two sets are equal
        /// </summary>
        /// <param name="set1">The first set to compare</param>
        /// <param name="set2">The second set to compare</param>
        /// <returns>true if the sets have the same elements; otherwise, false</returns>
        public static bool operator ==(Set? set1, Set? set2)
        {
            if (ReferenceEquals(set1, set2)) return true;
            if (set1 is null || set2 is null) return false;
            if (set1.Count != set2.Count) return false;

            foreach (var element in set1.elements)
            {
                if (!set2.Contains(element))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether two sets are not equal
        /// </summary>
        /// <param name="set1">The first set to compare</param>
        /// <param name="set2">The second set to compare</param>
        /// <returns>true if the sets have different elements; otherwise, false</returns>
        public static bool operator !=(Set? set1, Set? set2)
        {
            return !(set1 == set2);
        }

        /// <summary>
        /// Builds the power set (set of all subsets) of the current set
        /// </summary>
        /// <returns>
        /// A set containing all possible subsets of the current set, including the empty set
        /// </returns>
        /// <remarks>
        /// The power set contains 2^n elements where n is the number of elements in the original set.
        /// For example, a set with 3 elements has a power set with 8 subsets.
        /// </remarks>
        public ISet<ISet<object>> PowerSet()
        {
            var powerSet = new PowerSetWrapper();

            var elementsList = elements.ToList();
            int total = 1 << elementsList.Count;

            // Добавляем пустое множество
            powerSet.Add(new Set());

            for (int i = 1; i < total; i++)
            {
                var subset = new Set();
                for (int j = 0; j < elementsList.Count; j++)
                {
                    if ((i & (1 << j)) > 0)
                    {
                        subset.Add(elementsList[j]);
                    }
                }
                powerSet.Add(subset);
            }

            return powerSet;
        }

        /// <summary>
        /// Returns a string that represents the current set
        /// </summary>
        /// <returns>A string representation of the set in the format "{element1, element2, ...}"</returns>
        public override string ToString()
        {
            if (elements.Count == 0) return "{}";

            var elementStrings = elements.Select(e =>
            {
                if (e is Set set)
                    return set.ToString();
                else if (e is double || e is float)
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", e);
                else
                    return e.ToString();
            });
            return $"{{{string.Join(", ", elementStrings)}}}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current set
        /// </summary>
        /// <param name="obj">The object to compare with the current set</param>
        /// <returns>true if the specified object is equal to the current set; otherwise, false</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Set);
        }

        /// <summary>
        /// Determines whether the specified set is equal to the current set
        /// </summary>
        /// <param name="other">The set to compare with the current set</param>
        /// <returns>true if the specified set is equal to the current set; otherwise, false</returns>
        public bool Equals(Set? other)
        {
            return this == other;
        }

        /// <summary>
        /// Serves as the default hash function
        /// </summary>
        /// <returns>A hash code for the current set</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var element in elements.OrderBy(e => e?.GetHashCode() ?? 0))
            {
                hashCode.Add(element);
            }
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the set</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the set
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the set</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Internal wrapper class for power set implementation
        /// </summary>
        private class PowerSetWrapper : ISet<ISet<object>>
        {
            private readonly HashSet<ISet<object>> elements = new HashSet<ISet<object>>();

            /// <inheritdoc/>
            public int Count => elements.Count;

            /// <inheritdoc/>
            public void Add(ISet<object> element)
            {
                elements.Add(element);
            }

            /// <inheritdoc/>
            public bool Remove(ISet<object> element)
            {
                return elements.Remove(element);
            }

            /// <inheritdoc/>
            public bool Contains(ISet<object> element)
            {
                return elements.Contains(element);
            }

            /// <inheritdoc/>
            public bool IsEmpty()
            {
                return elements.Count == 0;
            }

            /// <inheritdoc/>
            public void Clear()
            {
                elements.Clear();
            }

            /// <inheritdoc/>
            public bool this[ISet<object> element] => Contains(element);

            /// <inheritdoc/>
            public void UnionWith(ISet<ISet<object>> other)
            {
                foreach (var element in other)
                {
                    Add(element);
                }
            }

            /// <inheritdoc/>
            public void IntersectWith(ISet<ISet<object>> other)
            {
                var toRemove = new List<ISet<object>>();
                foreach (var element in elements)
                {
                    if (!other.Contains(element))
                    {
                        toRemove.Add(element);
                    }
                }
                foreach (var element in toRemove)
                {
                    Remove(element);
                }
            }

            /// <inheritdoc/>
            public void ExceptWith(ISet<ISet<object>> other)
            {
                foreach (var element in other)
                {
                    Remove(element);
                }
            }

            /// <inheritdoc/>
            public IEnumerator<ISet<object>> GetEnumerator()
            {
                return elements.GetEnumerator();
            }

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}