using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Geometries
{
    /*
     * Test spatial predicate optimizations for rectangles by
     * synthesizing an exhaustive set of test cases.
     */
    [TestFixture]
    public class RectanglePredicateSyntheticTest
    {
        private WKTReader rdr = new WKTReader();
        private GeometryFactory fact = new GeometryFactory();

        double baseX = 10;
        double baseY = 10;
        double rectSize = 20;
        double bufSize = 10;
        double testGeomSize = 10;
        double bufferWidth = 1.0;

        Envelope rectEnv;
        Geometry rect;

        public RectanglePredicateSyntheticTest()
        {
            rectEnv = new Envelope(baseX, baseX + rectSize, baseY, baseY + rectSize);
            rect = fact.ToGeometry(rectEnv);
        }

        [Test]
        public void TestLines()
        {
            //TestContext.WriteLine(rect);

            var testGeoms = getTestGeometries();
            foreach (var testGeom in testGeoms)
            {
                runRectanglePredicates(rect, testGeom);
            }
        }

        [Test]
        public void TestDenseLines()
        {
            //TestContext.WriteLine(rect);

            var testGeoms = getTestGeometries();
            foreach (var testGeom in testGeoms)
            {
                var densifier = new SegmentDensifier((LineString)testGeom);
                var denseLine = (LineString)densifier.Densify(testGeomSize / 400);

                runRectanglePredicates(rect, denseLine);
            }
        }

        [Test]
        public void TestPolygons()
        {
            var testGeoms = getTestGeometries();
            foreach (var testGeom in testGeoms)
            {
                runRectanglePredicates(rect, testGeom.Buffer(bufferWidth));
            }
        }

        private List<Geometry> getTestGeometries()
        {
            var testEnv = new Envelope(rectEnv.MinX - bufSize, rectEnv.MaxX + bufSize,
                                            rectEnv.MinY - bufSize, rectEnv.MaxY + bufSize);
            var testGeoms = CreateTestGeometries(testEnv, 5, testGeomSize);
            return testGeoms;
        }

        private void runRectanglePredicates(Geometry rect, Geometry testGeom)
        {
            bool intersectsValue = rect.Intersects(testGeom);
            bool relateIntersectsValue = rect.Relate(testGeom).IsIntersects();
            bool intersectsOK = intersectsValue == relateIntersectsValue;

            bool containsValue = rect.Contains(testGeom);
            bool relateContainsValue = rect.Relate(testGeom).IsContains();
            bool containsOK = containsValue == relateContainsValue;

            //System.out.println(testGeom);
            if (!intersectsOK || !containsOK)
            {
                TestContext.WriteLine(testGeom);
            }
            Assert.IsTrue(intersectsOK);
            Assert.IsTrue(containsOK);
        }

        public List<Geometry> CreateTestGeometries(Envelope env, double inc, double size)
        {
            var testGeoms = new List<Geometry>();

            for (double y = env.MinY; y <= env.MaxY; y += inc)
            {
                for (double x = env.MinX; x <= env.MaxX; x += inc)
                {
                    var baseCoord = new Coordinate(x, y);
                    testGeoms.Add(CreateAngle(baseCoord, size, 0));
                    testGeoms.Add(CreateAngle(baseCoord, size, 1));
                    testGeoms.Add(CreateAngle(baseCoord, size, 2));
                    testGeoms.Add(CreateAngle(baseCoord, size, 3));
                }
            }
            return testGeoms;
        }

        public Geometry CreateAngle(Coordinate baseCoord, double size, int quadrant)
        {
            int[,] factor = new int[,] {
                                    { 1, 0 },
                                    { 0, 1 },
                                    { -1, 0 },
                                    { 0, -1 }
                                    };

            int xFac = factor[quadrant, 0];
            int yFac = factor[quadrant, 1];

            var p0 = new Coordinate(baseCoord.X + xFac * size, baseCoord.Y + yFac * size);
            var p2 = new Coordinate(baseCoord.X + yFac * size, baseCoord.Y + (-xFac) * size);

            return fact.CreateLineString(new Coordinate[] { p0, baseCoord, p2 });
        }
    }
}