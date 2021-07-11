using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Selene.Collections.UnitTests
{
    public class ReferenceListFacts : IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        
        public ReferenceListFacts(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }
        
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

        [Fact]
        public void InsertsItems()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i);
            var previous = sut[count / 2 - 1];
            var next = sut[count / 2];
            var first = sut.First();
            var last = sut.Last();

            // When
            sut.Insert(count / 2, int.MaxValue);

            // Then
            Assert.Equal(int.MaxValue, sut[count / 2]);
            Assert.Equal(previous, sut[count / 2 - 1]);
            Assert.Equal(next, sut[count / 2 + 1]);
            Assert.Equal(count + 1, sut.Count);

            // When
            sut.Insert(0, int.MaxValue);

            // Then
            Assert.Equal(int.MaxValue, sut.First());
            Assert.Equal(first, sut[1]);
            Assert.Equal(count + 2, sut.Count);

            // When
            sut.Insert(sut.Count - 1, int.MaxValue);

            // Then
            Assert.Equal(int.MaxValue, sut[^2]);
            Assert.Equal(last, sut.Last());
            Assert.Equal(count + 3, sut.Count);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        [InlineData(1_000_000)]
        [InlineData(10_000_000)]
        public void PerformanceOfAdd(int counts)
        {
            // Given
            {
                var rl = new ReferenceList<int>();
                var l = new List<int>();
                for (var index = 0; index < counts; ++index)
                {
                    rl.Add(index);
                    l.Add(index);
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

            var sut = new ReferenceList<int>();
            var list = new List<int>();
            long sutDuration = 0, listDuration = 0;
            long start = 0;
            
            // When
            for (var index = 0; index < counts; ++index)
            {
                start = Stopwatch.GetTimestamp();
                sut.Add(index);
                sutDuration += Stopwatch.GetTimestamp() - start;
                
                start = Stopwatch.GetTimestamp();
                list.Add(index);
                listDuration += Stopwatch.GetTimestamp() - start;
            }
            
            // Then
            _outputHelper.WriteLine("sut:  {0:#0.00000} s", (double)sutDuration / Stopwatch.Frequency);
            _outputHelper.WriteLine("list: {0:#0.00000} s", (double)listDuration / Stopwatch.Frequency);
            _outputHelper.WriteLine("gap: {0:#0.00000} %", (((double)sutDuration / (double)listDuration) - 1.0) * 100.0);
            #if RELEASE
            Assert.True((double)sutDuration / listDuration - 1.0 < 0.1);
            #endif
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1_000)]
        [InlineData(10_000)]
        [InlineData(100_000)]
        public void PerformanceOfInsert(int count)
        {
            // Given
            {
                var rl = new ReferenceList<int>();
                var l = new List<int>();
                for (var index = 0; index < count; ++index)
                {
                    rl.Insert(rl.Count / 2, index);
                    l.Insert(l.Count / 2, index);
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

            var sut = new ReferenceList<int>();
            var list = new List<int>();
            long sutDuration = 0, listDuration = 0;
            long start = 0;
            
            // When
            for (var index = 0; index < count; ++index)
            {
                start = Stopwatch.GetTimestamp();
                sut.Insert(sut.Count / 2, index);
                sutDuration += Stopwatch.GetTimestamp() - start;
                
                start = Stopwatch.GetTimestamp();
                list.Insert(list.Count / 2, index);
                listDuration += Stopwatch.GetTimestamp() - start;
            }
            
            // Then
            _outputHelper.WriteLine("sut:  {0:#0.00000} s", (double)sutDuration / Stopwatch.Frequency);
            _outputHelper.WriteLine("list: {0:#0.00000} s", (double)listDuration / Stopwatch.Frequency);
            _outputHelper.WriteLine("gap: {0:#0.00000} %", (((double)sutDuration / (double)listDuration) - 1.0) * 100.0);
            #if RELEASE
            Assert.True((double)sutDuration / listDuration - 1.0 < 0.1);
            #endif
        }
        
        [Fact]
        public void ContainsItemOrNot()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i + 1);

            // When
            var actual = sut.Contains(sut[count / 2]);

            // Then
            Assert.True(actual);

            // When
            actual = sut.Contains(int.MaxValue);

            // Then
            Assert.False(actual);

            // When
            actual = sut.Contains(0);

            // Then
            Assert.False(actual);

            // Given
            sut.Remove(count);
            
            // When
            actual = sut.Contains(count);

            // Then
            Assert.False(actual);
        }
                
        [Fact]
        public void CopiesToArray()
        {
            // Given
            var sut = new ReferenceList<int>();
            const int count = 10;
            for (var i = 0; i < count; ++i)
                sut.Add(i + 1);
                
            var actual = new int[count];

            // When
            sut.CopyTo(actual, 0);

            // Then
            Assert.Empty(sut.Zip(actual).Where(entry => entry.First != entry.Second));

            // Given
            var expected = new int[count * 2];
            actual = new int[count * 2];
            Array.Fill(actual, int.MaxValue);
            Array.Fill(expected, int.MaxValue);
            for (var i = 0; i < count; ++i)
                expected[i] = i + 1;
            
            // When
            sut.CopyTo(actual, 0);

            // Then
            Assert.Empty(expected.Zip(actual).Where(entry => entry.First != entry.Second));

            // Given
            expected = new int[count * 2];
            actual = new int[count * 2];
            const int offset = 5;
            Array.Fill(actual, int.MaxValue);
            Array.Fill(expected, int.MaxValue);
            for (var i = 0; i < count; ++i)
                expected[i + offset] = i + 1;
            
            // When
            sut.CopyTo(actual, offset);

            // Then
            Assert.Empty(expected.Zip(actual).Where(entry => entry.First != entry.Second));
        }
                
        [Fact]
        public void IsNotReadOnly()
        {
            // Given
            var sut = new ReferenceList<int>();

            // When
            var actual = sut.IsReadOnly;

            // Then
            Assert.False(actual);
        }
    }
}