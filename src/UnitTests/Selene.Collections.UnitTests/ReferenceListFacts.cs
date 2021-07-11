using System;
using System.Linq;
using Xunit;

namespace Selene.Collections.UnitTests
{
    public class ReferenceListFacts
    {
        [Fact]
        public void InitializesEmptyReferenceList()
        {
            // Given
            
            // When
            var sut = new ReferenceList<int>();
            
            // Then
            Assert.Empty(sut);
        }

        [Fact]
        public void AddsItems()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 1000;
           
            // When
            for(var i=0; i < count; ++i)
                sut.Add(i);

            // Then
            Assert.NotEmpty(sut);
            Assert.Equal(count, sut.Count);
            Assert.Equal(
                count,
                Enumerable
                    .Range(0, count)
                    .Zip(sut, (expected, actual) => (expected, actual))
                    .Count(testCase => testCase.expected == testCase.actual));
        }

        [Fact]
        public void ClearsItems()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 1000;
            for(var i=0; i < count; ++i)
                sut.Add(i);
           
            // When
            sut.Clear();

            // Then
            Assert.Empty(sut);
        }

        [Fact]
        public void RemoveItemAtPosition()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i);

            // When
            sut.RemoveAt(count / 2);

            // Then
            Assert.Equal(count / 2 + 1, sut[count / 2]);
            Assert.Equal(count - 1, sut.Count);

            // When
            sut.RemoveAt(0);

            // Then
            Assert.Equal(1, sut.First());
            Assert.Equal(count - 2, sut.Count);

            // When
            sut.RemoveAt(sut.Count - 1);

            // Then
            Assert.Equal(8, sut.Last());
            Assert.Equal(count - 3, sut.Count);
        }

        [Fact]
        public void RemoveItems()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i);
            var first = sut.First();
            var middle = sut[count / 2];
            var last = sut.Last();

            // When
            var result = sut.Remove(middle);

            // Then
            Assert.True(result);
            Assert.Equal(count / 2 + 1, sut[count / 2]);
            Assert.Equal(count - 1, sut.Count);

            // When
            result = sut.Remove(first);

            // Then
            Assert.True(result);
            Assert.Equal(1, sut.First());
            Assert.Equal(count - 2, sut.Count);

            // When
            result = sut.Remove(last);

            // Then
            Assert.True(result);
            Assert.Equal(count - 2, sut.Last());
            Assert.Equal(count - 3, sut.Count);

            // When
            result = sut.Remove(int.MaxValue);

            // Then
            Assert.False(result);
            Assert.Equal(count - 3, sut.Count);
        }

        [Fact]
        public void FindsIndexOrNot()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i + 1);

            // When
            var actual = sut.IndexOf(sut[count / 2]);

            // Then
            Assert.Equal(count / 2, actual);

            // When
            actual = sut.IndexOf(int.MaxValue);

            // Then
            Assert.Equal(-1, actual);

            // When
            actual = sut.IndexOf(0);

            // Then
            Assert.Equal(-1, actual);
        }
    }
}