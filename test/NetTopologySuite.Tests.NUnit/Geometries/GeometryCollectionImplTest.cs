using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Geometries
{
    [TestFixture]
    public class GeometryCollectionImplTest
    {
        private readonly NtsGeometryServices gs;
        private readonly WKTReader reader;

        public GeometryCollectionImplTest()
        {
            gs = new NtsGeometryServices(CoordinateArraySequenceFactory.Instance, new PrecisionModel(1000), 0);
            reader = new WKTReader(gs);
        }

        [Test]
        public void TestGetDimension()
        {
            var g = (GeometryCollection)reader.Read("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))");
            Assert.AreEqual(1, (int)g.Dimension);
        }

        [Test]
        public void TestGetCoordinates()
        {
            var g = (GeometryCollection)reader.Read("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))");
            var coordinates = g.Coordinates;
            Assert.AreEqual(4, g.NumPoints);
            Assert.AreEqual(4, coordinates.Length);
            Assert.AreEqual(new Coordinate(10, 10), coordinates[0]);
            Assert.AreEqual(new Coordinate(20, 20), coordinates[3]);
        }

        [Test]
        public void TestGeometryCollectionIterator()
        {
            var g = (GeometryCollection)reader.Read(
                  "GEOMETRYCOLLECTION (GEOMETRYCOLLECTION (POINT (10 10)))");
            var i = new GeometryCollectionEnumerator(g);
            //The NTS GeometryCollectionEnumerator does not have a HasNext property, and the interfaces is slightly different
            //assertTrue(i.hasNext());
            //assertTrue(i.next() instanceof GeometryCollection);
            //assertTrue(i.next() instanceof GeometryCollection);
            //assertTrue(i.next() instanceof Point);
            Assert.IsTrue(i.MoveNext());
            Assert.IsTrue(i.Current is GeometryCollection);
            Assert.IsTrue(i.MoveNext());
            Assert.IsTrue(i.Current is GeometryCollection);
            Assert.IsTrue(i.MoveNext());
            Assert.IsTrue(i.Current is Point);
        }

        [Test]
        public void TestGetLength()
        {
            var g = (GeometryCollection)new WKTReader().Read(
                  "MULTIPOLYGON("
                  + "((0 0, 10 0, 10 10, 0 10, 0 0), (3 3, 3 7, 7 7, 7 3, 3 3)),"
                  + "((100 100, 110 100, 110 110, 100 110, 100 100), (103 103, 103 107, 107 107, 107 103, 103 103)))");
            Assert.AreEqual(112, g.Length, 1E-15);
        }
    }
}
