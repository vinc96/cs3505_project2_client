using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeModel;

namespace SnakeModelTests
{
    [TestClass]
    public class PointTests
    {
        /// <summary>
        /// Tests the point functionality at some trivial location (1,5)
        /// </summary>
        [TestMethod]
        public void TestPointTrivialLocation()
        {
            Point p = new Point(1, 5);
            Assert.AreEqual(1, p.X);
            Assert.AreEqual(5, p.Y);
        }

        /// <summary>
        /// Tests the point functionality at some trivial location with negative coordinates (-6,-156)
        /// </summary>
        [TestMethod]
        public void TestPointTrivialLocationNegatives()
        {
            Point p = new Point(-6, -156);
            Assert.AreEqual(-6, p.X);
            Assert.AreEqual(-156, p.Y);
        }

        /// <summary>
        /// Tests to ensure that null objects aren't equal to a point, with the corner case that the point is at 0,0 (default values for int)
        /// </summary>
        [TestMethod]
        public void TestPointEqualsNull()
        {
            Point p0 = new Point(0,0);
            Assert.IsFalse(p0.Equals(null));
        }
        
        /// <summary>
        /// Tests to ensure that a point can't equal an object of another class, with the corner case that the other class is a different kind of point,
        /// with the same coordinates (obviously wouldn't matter, but this is as close as you get to a corner case with this)
        /// </summary>
        [TestMethod]
        public void TestPointEqualsNonPoint()
        {
            Point p = new Point(13, 15);
            System.Drawing.Point notPoint = new System.Drawing.Point(13, 15);
            Assert.IsFalse(p.Equals(notPoint));
        }

        /// <summary>
        /// Ensures that sign matters when determining if two points are equal.
        /// </summary>
        [TestMethod]
        public void TestPointEqualsDifferentSign()
        {
            Point p = new Point(143, -15);
            Point negP = new Point(-143, 15);
            Assert.IsFalse(p.Equals(negP));
            Assert.IsFalse(negP.Equals(p));
        }
        /// <summary>
        /// Ensures that points are not the same when x coordinates differ
        /// </summary>
        [TestMethod]
        public void TestPointEqualsXDifferent()
        {
            Point p1 = new Point(10, 100);
            Point p2 = new Point(15, 100);
            Assert.IsFalse(p1.Equals(p2));
            Assert.IsFalse(p2.Equals(p1));
        }

        /// <summary>
        /// Ensures that points are not the same when y coordinates differ
        /// </summary>
        [TestMethod]
        public void TestPointEqualsYDifferent()
        {
            Point p1 = new Point(100, 156);
            Point p2 = new Point(100, 134);
            Assert.IsFalse(p1.Equals(p2));
            Assert.IsFalse(p2.Equals(p1));
        }

        /// <summary>
        /// Ensures that points are not the same when both coordinates differ
        /// </summary>
        [TestMethod]
        public void TestPointEqualsBothDifferent()
        {
            Point p1 = new Point(10, 566);
            Point p2 = new Point(15, 100);
            Assert.IsFalse(p1.Equals(p2));
            Assert.IsFalse(p2.Equals(p1));
        }

        /// <summary>
        /// A trivial test case to ensure that two points with identical coordinates are the same.
        /// </summary>
        [TestMethod]
        public void TestPointEqualsAllSame()
        {
            Point p1 = new Point(1134114, 1345666);
            Point p2 = new Point(1134114, 1345666);
            Assert.IsTrue(p1.Equals(p2));
            Assert.IsTrue(p2.Equals(p1));
        }

    }
}
