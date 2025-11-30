using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SetLibrary.Tests
{
    public class SetTests
    {
        #region Constructor Tests
        [Fact]
        public void Constructor_EmptySet_IsEmpty()
        {
            var set = new Set();
            Assert.True(set.IsEmpty());
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Constructor_WithItems_ContainsItems()
        {
            var set = new Set(new object[] { 1, "test", 3.14 });
            Assert.Equal(3, set.Count);
            Assert.True(set.Contains(1));
            Assert.True(set.Contains("test"));
            Assert.True(set.Contains(3.14));
        }

        [Fact]
        public void Constructor_WithNullItems_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Set(new object[] { null! }));
        }

        [Fact]
        public void Constructor_CopyConstructor_CreatesDeepCopy()
        {
            var original = new Set(new object[] { 1, "test", new Set(new object[] { "nested" }) });
            var copy = new Set(original);

            Assert.Equal(original.Count, copy.Count);
            Assert.NotSame(original, copy);
        }

        [Fact]
        public void Constructor_CopyConstructorWithNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Set(null!));
        }
        #endregion

        #region FromString Tests
        [Fact]
        public void FromString_EmptySet_CreatesEmptySet()
        {
            var set = Set.FromString("{}");
            Assert.True(set.IsEmpty());
        }

        [Fact]
        public void FromString_SingleElement_CreatesCorrectSet()
        {
            var set = Set.FromString("{a}");
            Assert.Equal(1, set.Count);
            Assert.True(set.Contains("a"));
        }

        [Fact]
        public void FromString_MultipleElements_CreatesCorrectSet()
        {
            var set = Set.FromString("{a, b, c}");
            Assert.Equal(3, set.Count);
            Assert.True(set.Contains("a"));
            Assert.True(set.Contains("b"));
            Assert.True(set.Contains("c"));
        }

        [Fact]
        public void FromString_WithNumbers_ParsesCorrectly()
        {
            var set = Set.FromString("{1, 2.5, \"text\"}");
            Assert.Equal(3, set.Count);
            Assert.True(set.Contains(1));
            Assert.True(set.Contains(2.5));
            Assert.True(set.Contains("text"));
        }

        [Fact]
        public void FromString_WithQuotedStrings_ParsesCorrectly()
        {
            var set = Set.FromString("{\"hello world\", \"test\"}");
            Assert.Equal(2, set.Count);
            Assert.True(set.Contains("hello world"));
            Assert.True(set.Contains("test"));
        }

        [Fact]
        public void FromString_WithSingleQuotes_ParsesCorrectly()
        {
            var set = Set.FromString("{'hello world', 'test'}");
            Assert.Equal(2, set.Count);
            Assert.True(set.Contains("hello world"));
            Assert.True(set.Contains("test"));
        }

        [Fact]
        public void FromString_WithSpaces_ParsesCorrectly()
        {
            var set = Set.FromString("{  a  ,  b  ,  {  c  ,  d  }  }");
            Assert.Equal(3, set.Count);
            Assert.True(set.Contains("a"));
            Assert.True(set.Contains("b"));

            var nestedSet = set.OfType<Set>().First();
            Assert.True(nestedSet.Contains("c"));
            Assert.True(nestedSet.Contains("d"));
        }

        [Fact]
        public void FromString_ComplexNestedSet_CreatesCorrectStructure()
        {
            var set = Set.FromString("{a, b, c, {a, b}, {}, {a, {c}}}");

            Assert.Equal(6, set.Count);
            Assert.True(set.Contains("a"));
            Assert.True(set.Contains("b"));
            Assert.True(set.Contains("c"));

            var nestedSets = set.OfType<Set>().ToList();
            Assert.Equal(3, nestedSets.Count);

            var emptySet = nestedSets.First(s => s.IsEmpty());
            Assert.NotNull(emptySet);

            var setWithAB = nestedSets.First(s => s.Count == 2 && s.Contains("a") && s.Contains("b"));
            Assert.NotNull(setWithAB);
        }

        [Fact]
        public void FromString_InvalidFormat_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Set.FromString("invalid"));
            Assert.Throws<ArgumentException>(() => Set.FromString("{invalid"));
            Assert.Throws<ArgumentException>(() => Set.FromString("invalid}"));
        }
        #endregion

        #region Basic Operations Tests
        [Fact]
        public void Add_Element_SuccessfullyAdds()
        {
            var set = new Set();
            set.Add("test");
            Assert.Equal(1, set.Count);
            Assert.True(set.Contains("test"));
        }

        [Fact]
        public void Add_DuplicateElement_IgnoresDuplicate()
        {
            var set = new Set();
            set.Add("test");
            set.Add("test");
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Add_NullElement_ThrowsException()
        {
            var set = new Set();
            Assert.Throws<ArgumentNullException>(() => set.Add(null!));
        }

        [Fact]
        public void Add_NestedSet_AddsSuccessfully()
        {
            var innerSet = new Set(new object[] { 1, 2 });
            var mainSet = new Set();

            mainSet.Add(innerSet);

            Assert.True(mainSet.Contains(innerSet));
            Assert.Equal(1, mainSet.Count);
        }

        [Fact]
        public void Remove_ExistingElement_ReturnsTrue()
        {
            var set = new Set();
            set.Add("test");

            var result = set.Remove("test");

            Assert.True(result);
            Assert.Equal(0, set.Count);
            Assert.False(set.Contains("test"));
        }

        [Fact]
        public void Remove_NonExistingElement_ReturnsFalse()
        {
            var set = new Set();
            set.Add("test");

            var result = set.Remove("nonexistent");

            Assert.False(result);
            Assert.Equal(1, set.Count);
        }

        [Fact]
        public void Remove_NullElement_ReturnsFalse()
        {
            var set = new Set();
            var result = set.Remove(null!);
            Assert.False(result);
        }

        [Fact]
        public void Contains_ExistingElement_ReturnsTrue()
        {
            var set = new Set();
            set.Add("test");
            Assert.True(set.Contains("test"));
        }

        [Fact]
        public void Contains_NonExistingElement_ReturnsFalse()
        {
            var set = new Set();
            set.Add("test");
            Assert.False(set.Contains("nonexistent"));
        }

        [Fact]
        public void Contains_NullElement_ReturnsFalse()
        {
            var set = new Set();
            Assert.False(set.Contains(null!));
        }

        [Fact]
        public void Indexer_ExistingElement_ReturnsTrue()
        {
            var set = new Set();
            set.Add("test");

            Assert.True(set["test"]);
        }

        [Fact]
        public void Indexer_NonExistingElement_ReturnsFalse()
        {
            var set = new Set();
            set.Add("test");

            Assert.False(set["nonexistent"]);
        }

        [Fact]
        public void Indexer_NullElement_ReturnsFalse()
        {
            var set = new Set();
            Assert.False(set[null!]);
        }

        [Fact]
        public void Clear_NonEmptySet_BecomesEmpty()
        {
            var set = new Set(new object[] { 1, 2, 3 });
            set.Clear();
            Assert.True(set.IsEmpty());
            Assert.Equal(0, set.Count);
        }

        [Fact]
        public void Clear_EmptySet_RemainsEmpty()
        {
            var set = new Set();
            set.Clear();
            Assert.True(set.IsEmpty());
        }
        #endregion

        #region Union Operations Tests
        [Fact]
        public void Union_Operator_CombinesSets()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 3, 4, 5 });

            var result = set1 + set2;

            Assert.Equal(5, result.Count);
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(2));
            Assert.True(result.Contains(3));
            Assert.True(result.Contains(4));
            Assert.True(result.Contains(5));
        }

        [Fact]
        public void Union_Operator_WithNull_ThrowsException()
        {
            var set1 = new Set();
            Assert.Throws<ArgumentNullException>(() => set1 + null!);
            Assert.Throws<ArgumentNullException>(() => null! + set1);
        }

        [Fact]
        public void UnionWith_Method_ModifiesOriginal()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 2, 3 });

            set1.UnionWith(set2);

            Assert.Equal(3, set1.Count);
            Assert.True(set1.Contains(1));
            Assert.True(set1.Contains(2));
            Assert.True(set1.Contains(3));
        }

        [Fact]
        public void UnionWith_Method_WithCustomSet_DoesNothing()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var customSet = new CustomSet();
            set1.UnionWith(customSet);
            Assert.Equal(2, set1.Count);
        }
        #endregion

        #region Intersection Operations Tests
        [Fact]
        public void Intersection_Operator_ReturnsCommonElements()
        {
            var set1 = new Set(new object[] { 1, 2, 3, 4 });
            var set2 = new Set(new object[] { 3, 4, 5, 6 });

            var result = set1 * set2;

            Assert.Equal(2, result.Count);
            Assert.True(result.Contains(3));
            Assert.True(result.Contains(4));
        }

        [Fact]
        public void Intersection_Operator_WithNull_ThrowsException()
        {
            var set1 = new Set();
            Assert.Throws<ArgumentNullException>(() => set1 * null!);
            Assert.Throws<ArgumentNullException>(() => null! * set1);
        }

        [Fact]
        public void IntersectWith_Method_ModifiesOriginal()
        {
            var set1 = new Set(new object[] { 1, 2, 3, 4 });
            var set2 = new Set(new object[] { 3, 4, 5 });

            set1.IntersectWith(set2);

            Assert.Equal(2, set1.Count);
            Assert.True(set1.Contains(3));
            Assert.True(set1.Contains(4));
        }

        [Fact]
        public void IntersectWith_Method_WithCustomSet_ClearsSet()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var customSet = new CustomSet();
            set1.IntersectWith(customSet);
            Assert.True(set1.IsEmpty());
        }
        #endregion

        #region Difference Operations Tests
        [Fact]
        public void Difference_Operator_ReturnsUniqueElements()
        {
            var set1 = new Set(new object[] { 1, 2, 3, 4 });
            var set2 = new Set(new object[] { 3, 4, 5 });

            var result = set1 - set2;

            Assert.Equal(2, result.Count);
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(2));
        }

        [Fact]
        public void Difference_Operator_WithNull_ThrowsException()
        {
            var set1 = new Set();
            Assert.Throws<ArgumentNullException>(() => set1 - null!);
            Assert.Throws<ArgumentNullException>(() => null! - set1);
        }

        [Fact]
        public void ExceptWith_Method_ModifiesOriginal()
        {
            var set1 = new Set(new object[] { 1, 2, 3, 4 });
            var set2 = new Set(new object[] { 3, 4, 5 });

            set1.ExceptWith(set2);

            Assert.Equal(2, set1.Count);
            Assert.True(set1.Contains(1));
            Assert.True(set1.Contains(2));
        }

        [Fact]
        public void ExceptWith_Method_WithCustomSet_DoesNothing()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var customSet = new CustomSet();
            set1.ExceptWith(customSet);
            Assert.Equal(3, set1.Count);
        }
        #endregion

        #region Power Set Tests
        [Fact]
        public void PowerSet_EmptySet_ReturnsSetWithEmptySet()
        {
            var set = new Set();

            var powerSet = set.PowerSet();

            Assert.Equal(1, powerSet.Count);
            Assert.True(powerSet.Contains(new Set()));
        }

        [Fact]
        public void PowerSet_SingleElement_ReturnsTwoSubsets()
        {
            var set = new Set(new object[] { "a" });

            var powerSet = set.PowerSet();

            Assert.Equal(2, powerSet.Count);
            Assert.True(powerSet.Contains(new Set()));
            Assert.True(powerSet.Contains(new Set(new object[] { "a" })));
        }

        [Fact]
        public void PowerSet_ThreeElements_ReturnsEightSubsets()
        {
            var set = new Set(new object[] { 1, 2, 3 });

            var powerSet = set.PowerSet();

            Assert.Equal(8, powerSet.Count);
        }

        [Fact]
        public void PowerSet_PowerSetOperations_WorkCorrectly()
        {
            var set = new Set(new object[] { 1, 2 });
            var powerSet = set.PowerSet();

            Assert.False(powerSet.IsEmpty());
            Assert.Equal(4, powerSet.Count);
            Assert.True(powerSet[new Set()]);
            Assert.True(powerSet[new Set(new object[] { 1 })]);
            Assert.True(powerSet[new Set(new object[] { 2 })]);
            Assert.True(powerSet[new Set(new object[] { 1, 2 })]);
        }
        #endregion

        #region PowerSetWrapper Specific Tests
        [Fact]
        public void PowerSetWrapper_AddAndRemove_OperationsWorkCorrectly()
        {
            var set = new Set(new object[] { 1, 2 });
            var powerSet = set.PowerSet();

            var newSet = new Set(new object[] { 5, 6 });

            powerSet.Add(newSet);
            Assert.True(powerSet.Contains(newSet));
            Assert.Equal(5, powerSet.Count);

            var removed = powerSet.Remove(newSet);
            Assert.True(removed);
            Assert.False(powerSet.Contains(newSet));
            Assert.Equal(4, powerSet.Count);
        }

        [Fact]
        public void PowerSetWrapper_Clear_EmptiesTheSet()
        {
            var set = new Set(new object[] { 1 });
            var powerSet = set.PowerSet();

            Assert.False(powerSet.IsEmpty());
            Assert.Equal(2, powerSet.Count);

            powerSet.Clear();

            Assert.True(powerSet.IsEmpty());
            Assert.Equal(0, powerSet.Count);
        }

        [Fact]
        public void PowerSetWrapper_UnionWith_CombinesSets()
        {
            var set1 = new Set(new object[] { 1 });
            var powerSet1 = set1.PowerSet();

            var additionalSet = new Set(new object[] { 99 });
            var additionalPowerSet = additionalSet.PowerSet();

            powerSet1.UnionWith(additionalPowerSet);

            Assert.True(powerSet1.Contains(new Set()));
            Assert.True(powerSet1.Contains(new Set(new object[] { 1 })));
            Assert.True(powerSet1.Contains(new Set(new object[] { 99 })));
        }

        [Fact]
        public void PowerSetWrapper_IntersectWith_ReturnsCommonElements()
        {
            var set = new Set(new object[] { 1, 2 });
            var powerSet = set.PowerSet();

            var commonSets = new CustomSetWrapper();
            commonSets.Add(new Set());
            commonSets.Add(new Set(new object[] { 1 }));

            powerSet.IntersectWith(commonSets);

            Assert.Equal(2, powerSet.Count);
            Assert.True(powerSet.Contains(new Set()));
            Assert.True(powerSet.Contains(new Set(new object[] { 1 })));
            Assert.False(powerSet.Contains(new Set(new object[] { 2 })));
        }

        [Fact]
        public void PowerSetWrapper_ExceptWith_RemovesElements()
        {
            var set = new Set(new object[] { 1, 2 });
            var powerSet = set.PowerSet();

            var setsToRemove = new CustomSetWrapper();
            setsToRemove.Add(new Set(new object[] { 1, 2 }));

            powerSet.ExceptWith(setsToRemove);

            Assert.Equal(3, powerSet.Count);
            Assert.True(powerSet.Contains(new Set()));
            Assert.True(powerSet.Contains(new Set(new object[] { 1 })));
            Assert.True(powerSet.Contains(new Set(new object[] { 2 })));
            Assert.False(powerSet.Contains(new Set(new object[] { 1, 2 })));
        }

        [Fact]
        public void PowerSetWrapper_Indexer_ReturnsCorrectValue()
        {
            var set = new Set(new object[] { 1 });
            var powerSet = set.PowerSet();

            Assert.True(powerSet[new Set()]);
            Assert.True(powerSet[new Set(new object[] { 1 })]);
            Assert.False(powerSet[new Set(new object[] { 2 })]);
        }

        [Fact]
        public void PowerSetWrapper_Enumerator_IteratesAllElements()
        {
            var set = new Set(new object[] { 1 });
            var powerSet = set.PowerSet();

            var enumerated = new List<ISet<object>>();
            foreach (var subset in powerSet)
            {
                enumerated.Add(subset);
            }

            Assert.Equal(2, enumerated.Count);

            var emptySet = new Set();
            var singleElementSet = new Set(new object[] { 1 });

            Assert.Contains(emptySet, enumerated);
            Assert.Contains(singleElementSet, enumerated);
        }
        #endregion

        #region Equality and Comparison Tests
        [Fact]
        public void Equality_SameElements_ReturnsTrue()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.True(set1 == set2);
            Assert.True(set1.Equals(set2));
            Assert.True(set1.Equals((object)set2));
        }

        [Fact]
        public void Equality_DifferentElements_ReturnsFalse()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 1, 2, 4 });

            Assert.False(set1 == set2);
            Assert.False(set1.Equals(set2));
            Assert.False(set1.Equals((object)set2));
        }

        [Fact]
        public void Equality_WithNull_ReturnsFalse()
        {
            var set = new Set(new object[] { 1, 2, 3 });

            Assert.False(set == null);
            Assert.False(null == set);
            Assert.False(set.Equals(null));
        }

        [Fact]
        public void Equality_DifferentTypes_ReturnsFalse()
        {
            var set = new Set(new object[] { 1, 2, 3 });
            Assert.False(set.Equals("not a set"));
        }

        [Fact]
        public void Inequality_DifferentSets_ReturnsTrue()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 3, 4 });

            Assert.True(set1 != set2);
        }

        [Fact]
        public void GetHashCode_EqualSets_HaveSameHashCode()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.Equal(set1.GetHashCode(), set2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentSets_HaveDifferentHashCodes()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 4, 5, 6 });

            Assert.NotEqual(set1.GetHashCode(), set2.GetHashCode());
        }
        #endregion

        #region Static Properties Tests
        [Fact]
        public void SetCount_Static_CorrectlyTracksCreatedSets()
        {
            var initialCount = Set.SetCount;

            var set1 = new Set();
            var set2 = new Set();
            var set3 = new Set();

            Assert.Equal(initialCount + 3, Set.SetCount);
        }

        [Fact]
        public void Id_EachSet_HasUniqueId()
        {
            var set1 = new Set();
            var set2 = new Set();
            var set3 = new Set();

            Assert.NotEqual(set1.Id, set2.Id);
            Assert.NotEqual(set2.Id, set3.Id);
            Assert.NotEqual(set1.Id, set3.Id);
        }
        #endregion

        #region ToString Tests
        [Fact]
        public void ToString_EmptySet_ReturnsEmptyBraces()
        {
            var set = new Set();
            Assert.Equal("{}", set.ToString());
        }

        [Fact]
        public void ToString_SingleElement_ReturnsCorrectFormat()
        {
            var set = new Set(new object[] { "test" });
            Assert.Equal("{test}", set.ToString());
        }

        [Fact]
        public void ToString_MultipleElements_ReturnsCorrectFormat()
        {
            var set = new Set(new object[] { 1, "test", 3.14 });
            var result = set.ToString();

            Assert.Contains("1", result);
            Assert.Contains("test", result);
            Assert.Contains("3.14", result);
        }

        [Fact]
        public void ToString_NestedSets_ReturnsCorrectFormat()
        {
            var innerSet = new Set(new object[] { "b", "c" });
            var set = new Set(new object[] { "a", innerSet });

            var result = set.ToString();
            Assert.Equal("{a, {b, c}}", result);
        }

        [Fact]
        public void ToString_WithSpecialCharacters_FormatsCorrectly()
        {
            var set = new Set(new object[] { "hello, world", "test\nline", 3.14159 });
            var result = set.ToString();

            Assert.Contains("hello, world", result);
            Assert.Contains("test\nline", result);
            Assert.Contains("3.14159", result);
        }
        #endregion

        #region Complex Operations Tests
        [Fact]
        public void Set_ComplexNestedOperations_WorkCorrectly()
        {
            var set1 = Set.FromString("{1, 2, {3, 4}}");
            var set2 = Set.FromString("{2, {3, 4}, 5}");

            var intersection = set1 * set2;
            Assert.True(intersection.Contains(2));

            var nestedSets = intersection.OfType<Set>().ToList();
            Assert.Single(nestedSets);
            Assert.True(nestedSets[0].Contains(3));
            Assert.True(nestedSets[0].Contains(4));
        }

        [Fact]
        public void Set_DeepCopy_WithNestedSets_CreatesIndependentCopy()
        {
            var original = new Set(new object[] { 1, new Set(new object[] { "a", "b" }) });
            var copy = new Set(original);

            Assert.Equal(original.Count, copy.Count);
            Assert.NotSame(original, copy);

            var originalNested = original.OfType<Set>().First();
            var copyNested = copy.OfType<Set>().First();
            Assert.NotSame(originalNested, copyNested);
        }

        [Fact]
        public void Set_EdgeCases_HandleCorrectly()
        {
            var emptySet = new Set();
            Assert.True(emptySet.IsEmpty());
            Assert.Equal("{}", emptySet.ToString());

            var setWithDuplicates = new Set();
            setWithDuplicates.Add("test");
            setWithDuplicates.Add("test");
            Assert.Equal(1, setWithDuplicates.Count);
        }

        [Fact]
        public void Set_Operators_WithComplexStructures_WorkCorrectly()
        {
            var set1 = Set.FromString("{a, b, {x, y}}");
            var set2 = Set.FromString("{b, c, {x, z}}");

            var union = set1 + set2;
            Assert.Equal(5, union.Count);

            var intersection = set1 * set2;
            Assert.True(intersection.Contains("b"));

            var difference = set1 - set2;
            Assert.True(difference.Contains("a"));

            // ƒл€ сложных проверок используем Any с предикатом
            var setsWithY = difference.OfType<Set>().Where(s => s.Contains("y")).ToList();
            Assert.NotEmpty(setsWithY);
        }
        #endregion

        #region SetOperations Tests
        [Fact]
        public void IsSubset_EmptySet_IsSubsetOfAnySet()
        {
            var emptySet = new Set();
            var nonEmptySet = new Set(new object[] { 1, 2, 3 });

            Assert.True(SetOperations.IsSubset(emptySet, nonEmptySet));
        }

        [Fact]
        public void IsSubset_SetIsSubsetOfItself_ReturnsTrue()
        {
            var set = new Set(new object[] { 1, 2, 3 });
            Assert.True(SetOperations.IsSubset(set, set));
        }

        [Fact]
        public void IsSubset_ProperSubset_ReturnsTrue()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.True(SetOperations.IsSubset(set1, set2));
        }

        [Fact]
        public void IsSubset_NotSubset_ReturnsFalse()
        {
            var set1 = new Set(new object[] { 1, 2, 4 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.False(SetOperations.IsSubset(set1, set2));
        }

        [Fact]
        public void IsSubset_WithNull_ReturnsFalse()
        {
            var set = new Set();
            Assert.False(SetOperations.IsSubset(set, null!));
            Assert.False(SetOperations.IsSubset(null!, set));
            Assert.False(SetOperations.IsSubset(null!, null!));
        }

        [Fact]
        public void IsProperSubset_ProperSubset_ReturnsTrue()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.True(SetOperations.IsProperSubset(set1, set2));
        }

        [Fact]
        public void IsProperSubset_EqualSets_ReturnsFalse()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 1, 2, 3 });

            Assert.False(SetOperations.IsProperSubset(set1, set2));
        }

        [Fact]
        public void IsProperSubset_EmptySet_IsProperSubsetOfNonEmpty()
        {
            var emptySet = new Set();
            var nonEmptySet = new Set(new object[] { 1, 2, 3 });

            Assert.True(SetOperations.IsProperSubset(emptySet, nonEmptySet));
        }

        [Fact]
        public void SymmetricDifference_DisjointSets_ReturnsUnion()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 3, 4 });

            var result = SetOperations.SymmetricDifference(set1, set2);

            Assert.Equal(4, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
            Assert.Contains(3, result);
            Assert.Contains(4, result);
        }

        [Fact]
        public void SymmetricDifference_OverlappingSets_ReturnsCorrectDifference()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 2, 3, 4 });

            var result = SetOperations.SymmetricDifference(set1, set2);

            Assert.Equal(2, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(4, result);
            Assert.DoesNotContain(2, result);
            Assert.DoesNotContain(3, result);
        }

        [Fact]
        public void SymmetricDifference_WithNull_ThrowsException()
        {
            var set = new Set();
            Assert.Throws<ArgumentNullException>(() => SetOperations.SymmetricDifference(set, null!));
            Assert.Throws<ArgumentNullException>(() => SetOperations.SymmetricDifference(null!, set));
        }

        [Fact]
        public void AreDisjoint_DisjointSets_ReturnsTrue()
        {
            var set1 = new Set(new object[] { 1, 2 });
            var set2 = new Set(new object[] { 3, 4 });

            Assert.True(SetOperations.AreDisjoint(set1, set2));
        }

        [Fact]
        public void AreDisjoint_OverlappingSets_ReturnsFalse()
        {
            var set1 = new Set(new object[] { 1, 2, 3 });
            var set2 = new Set(new object[] { 3, 4, 5 });

            Assert.False(SetOperations.AreDisjoint(set1, set2));
        }

        [Fact]
        public void AreDisjoint_WithNull_ReturnsTrue()
        {
            var set = new Set();
            Assert.True(SetOperations.AreDisjoint(set, null!));
            Assert.True(SetOperations.AreDisjoint(null!, set));
            Assert.True(SetOperations.AreDisjoint(null!, null!));
        }

        [Fact]
        public void CreateSet_WithParams_CreatesCorrectSet()
        {
            var set = SetOperations.CreateSet(1, "test", 3.14);

            Assert.Equal(3, set.Count);
            Assert.Contains(1, set);
            Assert.Contains("test", set);
            Assert.Contains(3.14, set);
        }

        //[Fact]
        //public void CreateComplexSet_FromAssignmentExample_CreatesCorrectly()
        //{
        //    var complexSet = SetOperations.CreateComplexSet();

        //    Assert.Equal(6, complexSet.Count);
        //    Assert.Contains("a", complexSet);
        //    Assert.Contains("b", complexSet);
        //    Assert.Contains("c", complexSet);

        //    var nestedSets = complexSet.OfType<Set>().ToList();
        //    Assert.Equal(3, nestedSets.Count);

        //    var emptySet = nestedSets.First(s => s.IsEmpty());
        //    Assert.NotNull(emptySet);

        //    var setWithAB = nestedSets.First(s => s.Count == 2 && s.Contains("a") && s.Contains("b"));
        //    Assert.NotNull(setWithAB);

        //    var setWithAAndC = nestedSets.First(s => s.Count == 2 && s.Contains("a"));
        //    var innerSet = setWithAAndC.OfType<Set>().FirstOrDefault();
        //    Assert.NotNull(innerSet);
        //    Assert.Contains("c", innerSet);
        //}
        #endregion

        #region Enumerator Tests
        [Fact]
        public void Enumerator_IteratesOverAllElements()
        {
            var elements = new object[] { 1, "test", 3.14 };
            var set = new Set(elements);

            var enumerated = new List<object>();
            foreach (var element in set)
            {
                enumerated.Add(element);
            }

            Assert.Equal(elements.Length, enumerated.Count);

            Assert.Contains(1, enumerated);
            Assert.Contains("test", enumerated);
            Assert.Contains(3.14, enumerated);
        }
        #endregion

        #region Helper Classes
        private class CustomSet : ISet<object>
        {
            private readonly HashSet<object> elements = new HashSet<object>();

            public int Count => elements.Count;

            public bool this[object element] => elements.Contains(element);

            public void Add(object element) => elements.Add(element);

            public bool Remove(object element) => elements.Remove(element);

            public bool Contains(object element) => elements.Contains(element);

            public bool IsEmpty() => elements.Count == 0;

            public void Clear() => elements.Clear();

            public void UnionWith(ISet<object> other) { }

            public void IntersectWith(ISet<object> other) { }

            public void ExceptWith(ISet<object> other) { }

            public IEnumerator<object> GetEnumerator() => elements.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class CustomSetWrapper : ISet<ISet<object>>
        {
            private readonly HashSet<ISet<object>> elements = new HashSet<ISet<object>>();

            public int Count => elements.Count;

            public bool this[ISet<object> element] => elements.Contains(element);

            public void Add(ISet<object> element) => elements.Add(element);

            public bool Remove(ISet<object> element) => elements.Remove(element);

            public bool Contains(ISet<object> element) => elements.Contains(element);

            public bool IsEmpty() => elements.Count == 0;

            public void Clear() => elements.Clear();

            public void UnionWith(ISet<ISet<object>> other)
            {
                foreach (var element in other)
                {
                    elements.Add(element);
                }
            }

            public void IntersectWith(ISet<ISet<object>> other)
            {
                elements.IntersectWith(other);
            }

            public void ExceptWith(ISet<ISet<object>> other)
            {
                elements.ExceptWith(other);
            }

            public IEnumerator<ISet<object>> GetEnumerator() => elements.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }
        #endregion
    }
}