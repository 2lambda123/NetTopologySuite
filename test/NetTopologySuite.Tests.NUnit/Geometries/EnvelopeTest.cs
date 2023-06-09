using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Geometries
{
    [TestFixture]
    public class EnvelopeTest
    {
        private readonly GeometryFactory _geometryFactory;
        private readonly WKTReader _reader;

        public EnvelopeTest()
        {
            var gs = new NtsGeometryServices(PrecisionModel.Fixed.Value , 0);
            _geometryFactory = gs.CreateGeometryFactory();
            _reader = new WKTReader(gs);
        }

        [Test]
        public void TestEverything()
        {
            var e1 = new Envelope();
            Assert.IsTrue(e1.IsNull);
            Assert.AreEqual(0, e1.Width, 1E-3);
            Assert.AreEqual(0, e1.Height, 1E-3);
            e1.ExpandToInclude(100, 101);
            e1.ExpandToInclude(200, 202);
            e1.ExpandToInclude(150, 151);
            Assert.AreEqual(200, e1.MaxX, 1E-3);
            Assert.AreEqual(202, e1.MaxY, 1E-3);
            Assert.AreEqual(100, e1.MinX, 1E-3);
            Assert.AreEqual(101, e1.MinY, 1E-3);
            Assert.IsTrue(e1.Contains(120, 120));
            Assert.IsTrue(e1.Contains(120, 101));
            Assert.IsTrue(!e1.Contains(120, 100));
            Assert.AreEqual(101, e1.Height, 1E-3);
            Assert.AreEqual(100, e1.Width, 1E-3);
            Assert.IsTrue(!e1.IsNull);

            var e2 = new Envelope(499, 500, 500, 501);
            Assert.IsTrue(!e1.Contains(e2));
            Assert.IsTrue(!e1.Intersects(e2));
            e1.ExpandToInclude(e2);
            Assert.IsTrue(e1.Contains(e2));
            Assert.IsTrue(e1.Intersects(e2));
            Assert.AreEqual(500, e1.MaxX, 1E-3);
            Assert.AreEqual(501, e1.MaxY, 1E-3);
            Assert.AreEqual(100, e1.MinX, 1E-3);
            Assert.AreEqual(101, e1.MinY, 1E-3);

            var e3 = new Envelope(300, 700, 300, 700);
            Assert.IsTrue(!e1.Contains(e3));
            Assert.IsTrue(e1.Intersects(e3));

            var e4 = new Envelope(300, 301, 300, 301);
            Assert.IsTrue(e1.Contains(e4));
            Assert.IsTrue(e1.Intersects(e4));
        }

        [Test]
        public void TestDiscardNaN()
        {
            Assert.That(() => new Envelope(1d, double.NaN, 1d, double.NaN), Throws.ArgumentException);
            Assert.That(() => new  Envelope(double.NaN, 1d, double.NaN, 1d), Throws.ArgumentException);



            var env = new Envelope(new Coordinate(double.NegativeInfinity, double.NegativeInfinity), new Coordinate(double.PositiveInfinity, double.PositiveInfinity));
            Assert.That(env.IsNull, Is.False);

            env.Init();
            Assert.That(env.IsNull, Is.True);
            env.ExpandToInclude(double.NaN, 1d);
            Assert.That(env.IsNull, Is.True);
            env.ExpandToInclude(1d, double.NaN);
            Assert.That(env.IsNull, Is.True);
        }

        [Test]
        public void TestConstructorWithCoordinates()
        {
            var coords = new Coordinate[9];
            for (int i = 1; i <= coords.Length; i++)
                coords[i - 1] = new Coordinate(i, i);

            Envelope env = null;
            Assert.That(() => env = new Envelope(coords), Throws.Nothing);
            Assert.That(env, Is.Not.Null);
            Assert.That(env.MinX, Is.EqualTo(1d));
            Assert.That(env.MinY, Is.EqualTo(1d));
            Assert.That(env.MaxX, Is.EqualTo(9d));
            Assert.That(env.MaxY, Is.EqualTo(9d));
        }

        [TestCaseSource("CSFactories")]
        public void TestConstructorWithSequence(CoordinateSequenceFactory csFactory)
        {
            if (csFactory == null)
                Assert.Inconclusive();

            var cs = csFactory.Create(9, 2, 0);
            for (int i = 1; i <= cs.Count; i++)
            {
                cs.SetX(i - 1, i);
                cs.SetY(i - 1, i);
            }

            Envelope env = null;
            Assert.That(() => env = new Envelope(cs), Throws.Nothing);
            Assert.That(env, Is.Not.Null);
            Assert.That(env.MinX, Is.EqualTo(1d));
            Assert.That(env.MinY, Is.EqualTo(1d));
            Assert.That(env.MaxX, Is.EqualTo(9d));
            Assert.That(env.MaxY, Is.EqualTo(9d));
        }

        private static IEnumerable<CoordinateSequenceFactory> CSFactories
        {
            get
            {
                yield return CoordinateArraySequenceFactory.Instance;
                yield return PackedCoordinateSequenceFactory.DoubleFactory;
                yield return PackedCoordinateSequenceFactory.FloatFactory;
                yield return DotSpatialAffineCoordinateSequenceFactory.Instance;
                yield return new RawCoordinateSequenceFactory(new[] {Ordinates.XY});
            }
        }

        [Test]
        public void TestIntersects()
        {
            CheckIntersectsPermuted(1, 1, 2, 2, 2, 2, 3, 3, true);
            CheckIntersectsPermuted(1, 1, 2, 2, 3, 3, 4, 4, false);
        }

        [Test]
        public void TestIntersectsEmpty()
        {
            Assert.IsTrue(!new Envelope(-5, 5, -5, 5).Intersects(new Envelope()));
            Assert.IsTrue(!new Envelope().Intersects(new Envelope(-5, 5, -5, 5)));
            Assert.IsTrue(!new Envelope().Intersects(new Envelope(100, 101, 100, 101)));
            Assert.IsTrue(!new Envelope(100, 101, 100, 101).Intersects(new Envelope()));
        }

        [Test]
        public void TestDisjointEmpty()
        {
            Assert.IsTrue(new Envelope(-5, 5, -5, 5).Disjoint(new Envelope()));
            Assert.IsTrue(new Envelope().Disjoint(new Envelope(-5, 5, -5, 5)));
            Assert.IsTrue(new Envelope().Disjoint(new Envelope(100, 101, 100, 101)));
            Assert.IsTrue(new Envelope(100, 101, 100, 101).Disjoint(new Envelope()));
        }

        [Test]
        public void TestContainsEmpty()
        {
            Assert.IsTrue(!new Envelope(-5, 5, -5, 5).Contains(new Envelope()));
            Assert.IsTrue(!new Envelope().Contains(new Envelope(-5, 5, -5, 5)));
            Assert.IsTrue(!new Envelope().Contains(new Envelope(100, 101, 100, 101)));
            Assert.IsTrue(!new Envelope(100, 101, 100, 101).Contains(new Envelope()));
        }

        [Test]
        public void TestExpandToIncludeEmpty()
        {
            Assert.AreEqual(new Envelope(-5, 5, -5, 5), ExpandToInclude(new Envelope(-5,
                    5, -5, 5), new Envelope()));
            Assert.AreEqual(new Envelope(-5, 5, -5, 5), ExpandToInclude(new Envelope(),
                    new Envelope(-5, 5, -5, 5)));
            Assert.AreEqual(new Envelope(100, 101, 100, 101), ExpandToInclude(
                    new Envelope(), new Envelope(100, 101, 100, 101)));
            Assert.AreEqual(new Envelope(100, 101, 100, 101), ExpandToInclude(
                    new Envelope(100, 101, 100, 101), new Envelope()));
        }

        private static Envelope ExpandToInclude(Envelope a, Envelope b)
        {
            a.ExpandToInclude(b);
            return a;
        }

        [Test]
        public void TestEmpty()
        {
            Assert.AreEqual(0, new Envelope().Height, 0);
            Assert.AreEqual(0, new Envelope().Width, 0);
            Assert.AreEqual(new Envelope(), new Envelope());
            var e = new Envelope(100, 101, 100, 101);
            e.Init(new Envelope());
            Assert.AreEqual(new Envelope(), e);
        }

        [Test]
        public void TestAsGeometry()
        {
            Assert.IsTrue(_geometryFactory.CreatePoint((Coordinate)null).Envelope
                    .IsEmpty);

            var g = _geometryFactory.CreatePoint(new Coordinate(5, 6))
                    .Envelope;
            Assert.IsTrue(!g.IsEmpty);
            Assert.IsTrue(g is Point);

            var p = (Point)g;
            Assert.AreEqual(5, p.X, 1E-1);
            Assert.AreEqual(6, p.Y, 1E-1);

            var l = (LineString)_reader.Read("LINESTRING(10 10, 20 20, 30 40)");
            var g2 = l.Envelope;
            Assert.IsTrue(!g2.IsEmpty);
            Assert.IsTrue(g2 is Polygon);

            var poly = (Polygon)g2;
            poly.Normalize();
            Assert.AreEqual(5, poly.ExteriorRing.NumPoints);
            Assert.AreEqual(new Coordinate(10, 10), poly.ExteriorRing.GetCoordinateN(
                    0));
            Assert.AreEqual(new Coordinate(10, 40), poly.ExteriorRing.GetCoordinateN(
                    1));
            Assert.AreEqual(new Coordinate(30, 40), poly.ExteriorRing.GetCoordinateN(
                    2));
            Assert.AreEqual(new Coordinate(30, 10), poly.ExteriorRing.GetCoordinateN(
                    3));
            Assert.AreEqual(new Coordinate(10, 10), poly.ExteriorRing.GetCoordinateN(
                    4));
        }

        [Test]
        public void TestSetToNull()
        {
            var e1 = new Envelope();
            Assert.IsTrue(e1.IsNull);
            e1.ExpandToInclude(5, 5);
            Assert.IsTrue(!e1.IsNull);
            e1.SetToNull();
            Assert.IsTrue(e1.IsNull);
        }

        [Test]
        public void TestEquals()
        {
            var e1 = new Envelope(1, 2, 3, 4);
            var e2 = new Envelope(1, 2, 3, 4);
            Assert.AreEqual(e1, e2);
            Assert.AreEqual(e1.GetHashCode(), e2.GetHashCode());

            var e3 = new Envelope(1, 2, 3, 5);
            Assert.IsTrue(!e1.Equals(e3));
            Assert.IsTrue(e1.GetHashCode() != e3.GetHashCode());
            e1.SetToNull();
            Assert.IsTrue(!e1.Equals(e2));
            Assert.IsTrue(e1.GetHashCode() != e2.GetHashCode());
            e2.SetToNull();
            Assert.AreEqual(e1, e2);
            Assert.AreEqual(e1.GetHashCode(), e2.GetHashCode());
        }

        [Test]
        public void TestEquals2()
        {
            Assert.IsTrue(new Envelope().Equals(new Envelope()));
            Assert.IsTrue(new Envelope(1, 2, 1, 2).Equals(new Envelope(1, 2, 1, 2)));
            Assert.IsTrue(!new Envelope(1, 2, 1.5, 2).Equals(new Envelope(1, 2, 1, 2)));
        }

        [Test]
        public void TestCopyConstructor()
        {
            var e1 = new Envelope(1, 2, 3, 4);
            var e2 = new Envelope(e1);
            Assert.AreEqual(1, e2.MinX, 1E-5);
            Assert.AreEqual(2, e2.MaxX, 1E-5);
            Assert.AreEqual(3, e2.MinY, 1E-5);
            Assert.AreEqual(4, e2.MaxY, 1E-5);
        }


        [Test]
        public void TestCopyMethod()
        {
            var e1 = new Envelope(1, 2, 3, 4);
            var e2 = e1.Copy();
            Assert.AreEqual(1, e2.MinX, 1E-5);
            Assert.AreEqual(2, e2.MaxX, 1E-5);
            Assert.AreEqual(3, e2.MinY, 1E-5);
            Assert.AreEqual(4, e2.MaxY, 1E-5);

            Assert.That(ReferenceEquals(e1, e2), Is.False);

            var eNull = new Envelope();
            var eNullCopy = eNull.Copy();
            Assert.IsTrue(eNullCopy.IsNull);

        }

        [Test]
        public void TestGeometryFactoryCreateEnvelope()
        {
            checkExpectedEnvelopeGeometry("POINT (0 0)");
            checkExpectedEnvelopeGeometry("POINT (100 13)");
            checkExpectedEnvelopeGeometry("LINESTRING (0 0, 0 10)");
            checkExpectedEnvelopeGeometry("LINESTRING (0 0, 10 0)");

            string poly10 = "POLYGON ((0 10, 10 10, 10 0, 0 0, 0 10))";
            checkExpectedEnvelopeGeometry(poly10);

            checkExpectedEnvelopeGeometry("LINESTRING (0 0, 10 10)",
                    poly10);
            checkExpectedEnvelopeGeometry("POLYGON ((5 10, 10 6, 5 0, 0 6, 5 10))",
                    poly10);
        }

        [Test]
        public void TestMetrics()
        {
            var env = new Envelope(0, 4, 0, 3);
            Assert.That(env.Width, Is.EqualTo(4d));
            Assert.That(env.Height, Is.EqualTo(3d));
            Assert.That(env.Diameter, Is.EqualTo(5d));
        }

        [Test]
        public void TestEmptyMetrics()
        {
            var env = new Envelope();
            Assert.That(env.Width, Is.EqualTo(0d));
            Assert.That(env.Height, Is.EqualTo(0d));
            Assert.That(env.Diameter, Is.EqualTo(0d));
        }

        void checkExpectedEnvelopeGeometry(string wktInput)
        {
            checkExpectedEnvelopeGeometry(wktInput, wktInput);
        }

        void checkExpectedEnvelopeGeometry(string wktInput, string wktEnvGeomExpected)
        {
            var input = _reader.Read(wktInput);
            var envGeomExpected = _reader.Read(wktEnvGeomExpected);

            var env = input.EnvelopeInternal;
            var envGeomActual = _geometryFactory.ToGeometry(env);
            bool isEqual = envGeomActual.EqualsTopologically(envGeomExpected);
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void TestCompareTo()
        {
            CheckCompareTo(0, new Envelope(), new Envelope());
            CheckCompareTo(0, new Envelope(1, 2, 1, 2), new Envelope(1, 2, 1, 2));
            CheckCompareTo(1, new Envelope(2, 3, 1, 2), new Envelope(1, 2, 1, 2));
            CheckCompareTo(-1, new Envelope(1, 2, 1, 2), new Envelope(2, 3, 1, 2));
            CheckCompareTo(1, new Envelope(1, 2, 1, 3), new Envelope(1, 2, 1, 2));
            CheckCompareTo(1, new Envelope(2, 3, 1, 3), new Envelope(1, 3, 1, 2));
        }

        private static void CheckCompareTo(int expected, Envelope env1, Envelope env2)
        {
            Assert.IsTrue(expected == env1.CompareTo(env2), "expected == env1.CompareTo(env2)");
            Assert.IsTrue(-expected == env2.CompareTo(env1), "-expected == env2.CompareTo(env1)" );
        }


        [Test]
        public void TestToString()
        {
            TestToString(new Envelope(), "Env[Null]");
            TestToString(new Envelope(new Coordinate(10, 10)), "Env[10 : 10, 10 : 10]");
            TestToString(new Envelope(new Coordinate(10.1, 10.1)), "Env[10.1 : 10.1, 10.1 : 10.1]");
            TestToString(new Envelope(new Coordinate(10.1, 19.9), new Coordinate(19.9, 10.1)), "Env[10.1 : 19.9, 10.1 : 19.9]");
        }

        private static void TestToString(Envelope env, string envString)
        {
            string toString = env.ToString();
            Assert.AreEqual(envString, toString);
        }

        [Test]
        public void TestParse()
        {
            TestParse("Env[Null]", new Envelope());
            TestParse("Env[10 : 10, 10 : 10]", new Envelope(new Coordinate(10, 10)));
            TestParse("Env[10.1 : 10.1, 10.1 : 10.1]", new Envelope(new Coordinate(10.1, 10.1)));
            TestParse("Env[10.1 : 19.9, 10.1 : 19.9]", new Envelope(new Coordinate(10.1, 19.9), new Coordinate(19.9, 10.1)));
            Assert.Throws<ArgumentNullException>(() => TestParse(null, new Envelope()));
            Assert.Throws<ArgumentException>(() => TestParse("no envelope", new Envelope()));
            Assert.Throws<ArgumentException>(() => TestParse("Env[10.1 : 19.9, 10.1 : 19/9]", new Envelope()));
        }

        private static void TestParse(string envString, Envelope env)
        {
            var envFromString = Envelope.Parse(envString);
            Assert.IsTrue(env.Equals(envFromString),"{0} != {1}", env, envFromString);
        }


        private void CheckIntersectsPermuted(double a1x, double a1y, double a2x, double a2y, double b1x, double b1y, double b2x, double b2y, bool expected)
        {
            CheckIntersects(a1x, a1y, a2x, a2y, b1x, b1y, b2x, b2y, expected);
            CheckIntersects(a1x, a2y, a2x, a1y, b1x, b1y, b2x, b2y, expected);
            CheckIntersects(a1x, a1y, a2x, a2y, b1x, b2y, b2x, b1y, expected);
            CheckIntersects(a1x, a2y, a2x, a1y, b1x, b2y, b2x, b1y, expected);
        }

        private void CheckIntersects(double a1x, double a1y, double a2x, double a2y, double b1x, double b1y, double b2x, double b2y, bool expected)
        {
            var a = new Envelope(a1x, a2x, a1y, a2y);
            var b = new Envelope(b1x, b2x, b1y, b2y);
            Assert.AreEqual(expected, a.Intersects(b));

            var a1 = new Coordinate(a1x, a1y);
            var a2 = new Coordinate(a2x, a2y);
            var b1 = new Coordinate(b1x, b1y);
            var b2 = new Coordinate(b2x, b2y);
            Assert.AreEqual(expected, Envelope.Intersects(a1, a2, b1, b2));

            Assert.AreEqual(expected, a.Intersects(b1, b2));
        }


    }
}
