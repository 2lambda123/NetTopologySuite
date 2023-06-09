using System;
using NetTopologySuite.Algorithm;
using NetTopologySuite.IO;
using NetTopologySuite.Operation;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Operation
{
    /// <summary>
    /// Tests <see cref="BoundaryOp"/> with different <see cref="IBoundaryNodeRule"/>s.
    /// </summary>
    /// <author>Martin Davis</author>
    /// <version>1.7</version>
    public class BoundaryTest : GeometryTestCase
    {
        private readonly WKTReader _rdr = new WKTReader();

        /// <summary>
        /// For testing only.
        /// </summary>
        /// <exception cref="Exception" />
        [Test]
        public void Test1()
        {
            string a = "MULTILINESTRING ((0 0, 10 10), (10 10, 20 20))";
            // under MultiValent, the common point is the only point on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.MultivalentEndpointBoundaryRule,
                "POINT (10 10)");
        }

        [Test]
        public void Test2LinesTouchAtEndpoint2()
        {
            string a = "MULTILINESTRING ((0 0, 10 10), (10 10, 20 20))";

            // under Mod-2, the common point is not on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.Mod2BoundaryRule,
                "MULTIPOINT ((0 0), (20 20))");
            // under Endpoint, the common point is on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.EndpointBoundaryRule,
                "MULTIPOINT ((0 0), (10 10), (20 20))");
            // under MonoValent, the common point is not on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.MonoValentEndpointBoundaryRule,
                "MULTIPOINT ((0 0), (20 20))");
            // under MultiValent, the common point is the only point on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.MultivalentEndpointBoundaryRule,
                "POINT (10 10)");
        }

        [Test]
        public void Test3LinesTouchAtEndpoint2()
        {
            string a = "MULTILINESTRING ((0 0, 10 10), (10 10, 20 20), (10 10, 10 20))";

            // under Mod-2, the common point is on the boundary (3 mod 2 = 1)
            RunBoundaryTest(a, BoundaryNodeRules.Mod2BoundaryRule,
                "MULTIPOINT ((0 0), (10 10), (10 20), (20 20))");
            // under Endpoint, the common point is on the boundary (it is an endpoint)
            RunBoundaryTest(a, BoundaryNodeRules.EndpointBoundaryRule,
                "MULTIPOINT ((0 0), (10 10), (10 20), (20 20))");
            // under MonoValent, the common point is not on the boundary (it has valence > 1)
            RunBoundaryTest(a, BoundaryNodeRules.MonoValentEndpointBoundaryRule,
                "MULTIPOINT ((0 0), (10 20), (20 20))");
            // under MultiValent, the common point is the only point on the boundary
            RunBoundaryTest(a, BoundaryNodeRules.MultivalentEndpointBoundaryRule,
                "POINT (10 10)");
        }

        [Test]
        public void TestMultiLineStringWithRingTouchAtEndpoint()
        {
            string a = "MULTILINESTRING ((100 100, 20 20, 200 20, 100 100), (100 200, 100 100))";

            // under Mod-2, the ring has no boundary, so the line intersects the interior ==> not simple
            RunBoundaryTest(a, BoundaryNodeRules.Mod2BoundaryRule,
                "MULTIPOINT ((100 100), (100 200))");
            // under Endpoint, the ring has a boundary point, so the line does NOT intersect the interior ==> simple
            RunBoundaryTest(a, BoundaryNodeRules.EndpointBoundaryRule,
                "MULTIPOINT ((100 100), (100 200))");
        }

        [Test]
        public void TestRing()
        {
            string a = "LINESTRING (100 100, 20 20, 200 20, 100 100)";

            // rings are simple under all rules
            RunBoundaryTest(a, BoundaryNodeRules.Mod2BoundaryRule,
                "MULTIPOINT EMPTY");
            RunBoundaryTest(a, BoundaryNodeRules.EndpointBoundaryRule,
                "POINT (100 100)");
        }

        [Test]
        public void TestHasBoundaryPoint()
        {
            CheckHasBoundary("POINT (0 0)", false);
        }

        [Test]
        public void TestHasBoundaryPointEmpty()
        {
            CheckHasBoundary("POINT EMPTY", false);
        }

        [Test]
        public void TestHasBoundaryRingClosed()
        {
            CheckHasBoundary("LINESTRING (100 100, 20 20, 200 20, 100 100)", false);
        }

        [Test]
        public void TestHasBoundaryMultiLineStringClosed()
        {
            CheckHasBoundary("MULTILINESTRING ((0 0, 0 1), (0 1, 1 1, 1 0, 0 0))", false);
        }

        [Test]
        public void TestHasBoundaryMultiLineStringOpen()
        {
            CheckHasBoundary("MULTILINESTRING ((0 0, 0 2), (0 1, 1 1, 1 0, 0 0))");
        }

        [Test]
        public void TestHasBoundaryPolygon()
        {
            CheckHasBoundary("POLYGON ((1 9, 9 9, 9 1, 1 1, 1 9))");
        }

        [Test]
        public void TestHasBoundaryPolygonEmpty()
        {
            CheckHasBoundary("POLYGON EMPTY", false);
        }

        private void RunBoundaryTest(string wkt, IBoundaryNodeRule bnRule, string wktExpected)
        {
            var g = _rdr.Read(wkt);
            var expected = _rdr.Read(wktExpected);

            var op = new BoundaryOp(g, bnRule);
            var boundary = op.GetBoundary();
            boundary.Normalize();
            //    System.out.println("Computed Boundary = " + boundary);
            Assert.IsTrue(boundary.EqualsExact(expected));
        }


        private void CheckHasBoundary(string wkt)
        {
            CheckHasBoundary(wkt, BoundaryNodeRules.Mod2BoundaryRule, true);
        }

        private void CheckHasBoundary(string wkt, bool expected)
        {
            CheckHasBoundary(wkt, BoundaryNodeRules.Mod2BoundaryRule, expected);
        }

        private void CheckHasBoundary(string wkt, IBoundaryNodeRule bnRule, bool expected)
        {
            var g = Read(wkt);
            Assert.That(BoundaryOp.HasBoundary(g, bnRule), Is.EqualTo(expected));
        }
    }
}
