using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Corvalius.Common.Test
{

    public class SeriesExtensionsTest
    {
        public class IntRange
        {
            public int Start;
            public int End;

            public IntRange(int start, int end)
            {
                this.Start = start;
                this.End = end;
            }
        }

        [Fact]
        public void BasicExample()
        {
            var source = new[] { new IntRange(0, 5) };
            var range = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            var result = source.SplitByRange(range, x => x.Start, x => x.End, (s, x, y) => new IntRange(x, y), Comparer<int>.Default);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public void BasicExampleWithIntersection()
        {
            var source = new[] { new IntRange(0, 2), new IntRange(1, 3) };
            var range = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            var result = source.SplitByRange(range, x => x.Start, x => x.End, (s, x, y) => new IntRange(x, y), Comparer<int>.Default);
            Assert.Equal(6, result.Count());
        }

        public class DoubleRange
        {
            public double Start;
            public double End;

            public DoubleRange(double start, double end)
            {
                this.Start = start;
                this.End = end;
            }
        }

        [Fact]
        public void BasicExampleWithMiddleGround()
        {
            var source = new[] { new DoubleRange(0.5, 1.5) };
            var range = new[] { 0.0, 1, 2, 3, 4, 5, 6, 7, 8 };

            var result = source.SplitByRange(range, x => x.Start, x => x.End, (s, x, y) => new DoubleRange(x, y), Comparer<double>.Default);
            Assert.Equal(2, result.Count());
        }
    }
}