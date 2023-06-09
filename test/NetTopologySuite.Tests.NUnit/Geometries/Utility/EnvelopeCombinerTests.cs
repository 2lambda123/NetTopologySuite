﻿using System;
using System.Collections.Generic;
using System.Linq;

using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Tests.NUnit.Utilities;

using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Geometries.Utility
{
    public class EnvelopeCombinerTests
    {
        [TestCaseSource(typeof(PointwiseGeometryAggregationTestCases))]
        public void TestEnvelopeCombine(ICollection<Geometry> geoms)
        {
            var actual = EnvelopeCombiner.Combine(geoms);
            var actualGeometry = EnvelopeCombiner.CombineAsGeometry(geoms);

            // JTS doesn't usually bother doing anything special about nulls,
            // so our ports of their stuff will suffer the same.
            geoms = geoms?.Where(g => g != null).ToArray() ?? Array.Empty<Geometry>();

            var combinedGeometry = GeometryCombiner.Combine(geoms);

            // JTS also doesn't fear giving us nulls back from its algorithms.
            var expected = combinedGeometry?.EnvelopeInternal ?? new Envelope();

            Assert.AreEqual(expected, actual);

            if (expected.IsNull)
            {
                Assert.That(actualGeometry.IsEmpty, "Expected empty, but was: {0}", actualGeometry);
            }
            else
            {
                var expectedGeometry = GeometryFactory.Default.ToGeometry(expected);
                Assert.That(expectedGeometry.EqualsTopologically(actualGeometry), "{2}Expected: {0}{2}Actual  : {1}", expectedGeometry, actualGeometry, Environment.NewLine);
            }
        }
    }
}
