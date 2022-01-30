namespace Reginald.Tests
{
    using System;
    using Reginald.Core.Collections;
    using Xunit;

    public static class DequeTests
    {
        [Fact]
        public static void Deque_WhenGivenNegativeMaxLength_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Deque<int>(-1));
        }

        [Theory]
        [InlineData(new int[] { 1, 2, 3 }, 2)]
        [InlineData(new int[] { 1, 2, 3 }, 0)]
        [InlineData(new int[] { }, 2)]
        [InlineData(new int[] { }, 0)]
        public static void Deque_WhenGivenItemsAndMaxLength_ShouldReturnCorrectMaxLength(int[] array, int maxLength)
        {
            Deque<int> d = new(array, maxLength);
            Assert.Equal(maxLength, d.MaxLength);
        }

        [Theory]
        [InlineData(new int[] { })]
        [InlineData(new int[] { 1, 2 })]
        [InlineData(new int[] { 1, 2, 3 })]
        public static void Deque_WhenGivenItems_ShouldReturnCorrectMaxLength(int[] array)
        {
            int expected = -1;
            Deque<int> d = new(array);
            Assert.Equal(expected, d.MaxLength);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        public static void Deque_WhenGivenMaxLength_ShouldReturnCorrectMaxLength(int maxLength)
        {
            Deque<int> d = new(maxLength);
            Assert.Equal(maxLength, d.MaxLength);
        }

        [Fact]
        public static void Indexer_ShouldGetValue()
        {
            Deque<int> d = new(new int[] { 1, 2, 3 });
            Assert.Equal(2, d[1]);
        }

        [Fact]
        public static void Indexer_WhenIndexOutOfRange_ShouldThrow()
        {
            Deque<int> d = new(new int[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>(() => d[d.Count]);
        }

        [Fact]
        public static void Append_WhenItemsLengthAndMaxLengthAreEqual_ShouldAddItemToRightSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2, 3 }, size);
            int value = 4;
            d.Append(value);
            Assert.Equal(value, d[d.Count - 1]);
        }

        [Fact]
        public static void Append_WhenItemsLengthIsGreaterThanMaxLength_ShouldAddItemToRightSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2, 3, 4 }, size);
            int value = 5;
            d.Append(value);
            Assert.Equal(value, d[d.Count - 1]);
        }

        [Fact]
        public static void Append_WhenItemsLengthIsLessThanMaxLength_ShouldAddItemToRightSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2 }, size);
            int value = 3;
            d.Append(value);
            Assert.Equal(value, d[d.Count - 1]);
        }

        [Fact]
        public static void Append_WhenMaxLengthNotGiven_ShouldAddItemToRightSide()
        {
            int[] numbers = new int[] { 1, 2 };
            Deque<int> d = new(numbers);
            int value = 3;
            d.Append(value);
            Assert.Equal(value, d[d.Count - 1]);
        }

        [Fact]
        public static void Append_WhenNoItemsGiven_ShouldAddItemToRightSide()
        {
            Deque<int> d = new(1);
            int value = 1;
            d.Append(value);
            Assert.Equal(value, d[^1]);
        }

        [Fact]
        public static void Append_WhenGivenNoItemsAndAlreadyCalledAppend_ShouldAddItemToRightSide()
        {
            Deque<int> d = new(2);
            int value = 1;
            d.Append(value);
            d.Append(value + 1);
            Assert.Equal(value + 1, d[^1]);
        }

        [Fact]
        public static void Append_WhenMaxLengthIsZero_ShouldNotAddItem()
        {
            Deque<int> d = new(0);
            d.Append(1);
            Assert.True(d.Count == 0);
        }

        [Fact]
        public static void AppendLeft_WhenItemsLengthAndMaxLengthAreEqual_ShouldAddItemToLeftSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2, 3 }, size);
            int value = 4;
            d.AppendLeft(value);
            Assert.Equal(value, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenItemsLengthIsGreaterThanMaxLength_ShouldAddItemToLeftSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2, 3, 4 }, size);
            int value = 5;
            d.AppendLeft(value);
            Assert.Equal(value, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenItemsLengthIsLessThanMaxLength_ShouldAddItemToLeftSide()
        {
            int size = 3;
            Deque<int> d = new(new int[] { 1, 2 }, size);
            int value = 3;
            d.AppendLeft(value);
            Assert.Equal(value, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenMaxLengthNotGiven_ShouldAddItemToLeftSide()
        {
            int[] numbers = new int[] { 1, 2 };
            Deque<int> d = new(numbers);
            int value = 3;
            d.AppendLeft(value);
            Assert.Equal(value, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenNoItemsGiven_ShouldAddItemToLeftSide()
        {
            Deque<int> d = new(1);
            int value = 1;
            d.AppendLeft(value);
            Assert.Equal(value, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenGivenNoItemsAndAlreadyCalledAppendLeft_ShouldAddItemLeftSide()
        {
            Deque<int> d = new(2);
            int value = 1;
            d.AppendLeft(value);
            d.AppendLeft(value + 1);
            Assert.Equal(value + 1, d[0]);
        }

        [Fact]
        public static void AppendLeft_WhenMaxLengthIsZero_ShouldNotAddItem()
        {
            Deque<int> d = new(0);
            d.AppendLeft(1);
            Assert.True(d.Count == 0);
        }

        [Fact]
        public static void Pop_WhenItems_ShouldReturnLastItem()
        {
            int[] numbers = new int[] { 1, 2, 3 };
            Deque<int> d = new(numbers, 3);
            Assert.Equal(numbers[^1], d.Pop());
        }

        [Fact]
        public static void Pop_WhenItems_ShouldReduceDeque()
        {
            int[] numbers = new int[] { 1, 2, 3 };
            Deque<int> d = new(numbers, 3);
            _ = d.Pop();
            Assert.Equal(numbers.Length - 1, d.Count);
        }

        [Fact]
        public static void Pop_WhenNoItems_ShouldThrow()
        {
            Deque<int> d = new(0);
            Assert.Throws<InvalidOperationException>(() => d.Pop());
        }

        [Fact]
        public static void PopLeft_WhenItems_ShouldReturnFirstItem()
        {
            int[] numbers = new int[] { 1, 2, 3 };
            Deque<int> d = new(numbers, 3);
            Assert.Equal(numbers[0], d.PopLeft());
        }

        [Fact]
        public static void PopLeft_WhenItems_ShouldReduceDeque()
        {
            int[] numbers = new int[] { 1, 2, 3 };
            Deque<int> d = new(numbers, 3);
            _ = d.PopLeft();
            Assert.Equal(numbers.Length - 1, d.Count);
        }

        [Fact]
        public static void PopLeft_WhenNoItems_ShouldThrow()
        {
            Deque<int> d = new(0);
            Assert.Throws<InvalidOperationException>(() => d.PopLeft());
        }
    }
}
