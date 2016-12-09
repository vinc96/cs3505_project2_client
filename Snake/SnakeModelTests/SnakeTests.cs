using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeModel;
using System.Collections.Generic;

namespace SnakeModelTests
{
    [TestClass]
    public class SnakeTests
    {
        /// <summary>
        /// Ensures the snake constructor builds a snake with the values we've stated for it. (Up version)
        /// </summary>
        [TestMethod]
        public void TestSnakeConstructorUp()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.UP, 10);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("Mr.Snake", s.name);
            Assert.AreEqual(new Point(10, 10), s.getHead());
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 20), verts[0]);
            Assert.AreEqual(2, verts.Count);
            Assert.AreEqual(Snake.Direction.UP, s.CurrentDirection);
            Assert.AreEqual(Snake.Direction.UP, s.NextDirection);
            Assert.AreEqual(10, s.length);
        }

        /// <summary>
        /// Ensures the snake constructor builds a snake with the values we've stated for it. (Down version)
        /// </summary>
        [TestMethod]
        public void TestSnakeConstructorDown()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("Mr.Snake", s.name);
            Assert.AreEqual(new Point(10, 10), s.getHead());
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 0), verts[0]);
            Assert.AreEqual(2, verts.Count);
            Assert.AreEqual(Snake.Direction.DOWN, s.CurrentDirection);
            Assert.AreEqual(Snake.Direction.DOWN, s.NextDirection);
            Assert.AreEqual(10, s.length);
        }

        /// <summary>
        /// Ensures the snake constructor builds a snake with the values we've stated for it. (Right version)
        /// </summary>
        [TestMethod]
        public void TestSnakeConstructorRight()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("Mr.Snake", s.name);
            Assert.AreEqual(new Point(10, 10), s.getHead());
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(0, 10), verts[0]);
            Assert.AreEqual(2, verts.Count);
            Assert.AreEqual(Snake.Direction.RIGHT, s.CurrentDirection);
            Assert.AreEqual(Snake.Direction.RIGHT, s.NextDirection);
            Assert.AreEqual(10, s.length);
        }

        /// <summary>
        /// Ensures the snake constructor builds a snake with the values we've stated for it. (Left version)
        /// </summary>
        [TestMethod]
        public void TestSnakeConstructorLeft()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("Mr.Snake", s.name);
            Assert.AreEqual(new Point(10, 10), s.getHead());
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(20, 10), verts[0]);
            Assert.AreEqual(2, verts.Count);
            Assert.AreEqual(Snake.Direction.LEFT, s.CurrentDirection);
            Assert.AreEqual(Snake.Direction.LEFT, s.NextDirection);
            Assert.AreEqual(10, s.length);
        }

        /// <summary>
        /// Ensures that when we move the snake's head forward in the current direction, that the results we get are correct.
        /// Tests for the left direction
        /// </summary>
        [TestMethod]
        public void TestSnakeMoveHeadCurrentDirectionLeft()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            s.MoveHead();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(9, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(20, 10), verts[0]);
        }

        /// <summary>
        /// Ensures that when we move the snake's head forward in the current direction, that the results we get are correct.
        /// Tests for the Right direction
        /// </summary>
        [TestMethod]
        public void TestSnakeMoveHeadCurrentDirectionRight()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            s.MoveHead();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(11, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(0, 10), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }

        /// <summary>
        /// Ensures that when we move the snake's head forward in the current direction, that the results we get are correct.
        /// Tests for the Up direction
        /// </summary>
        [TestMethod]
        public void TestSnakeMoveHeadCurrentDirectionUp()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.UP, 10);
            s.MoveHead();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 9), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 20), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }

        /// <summary>
        /// Ensures that when we move the snake's head forward in the current direction, that the results we get are correct.
        /// Tests for the Down direction
        /// </summary>
        [TestMethod]
        public void TestSnakeMoveHeadCurrentDirectionDown()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            s.MoveHead();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 11), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 0), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }

        /// <summary>
        /// Tests to ensure that two snakes with the same ID will have the same color, regardless
        /// of other charactaristics
        /// </summary>
        [TestMethod]
        public void ColorAssociatesWithID()
        {
            Snake s1 = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            Snake s2 = new Snake(10, "Mr.Snake", new Point(10, 15), Snake.Direction.UP, 25);
            Assert.AreEqual(s1.Color, s2.Color);
        }    

        /// <summary>
        /// A trivial test that asserts that two snakes with different IDs will have different colors. 
        /// </summary>
        [TestMethod]
        public void ColorDifferentDifferentIDs()
        {
            Snake s1 = new Snake(1, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            Snake s2 = new Snake(2, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            Assert.AreNotEqual(s1.Color, s2.Color);
        }

        /// <summary>
        /// A test of the GetAllPoints enumerator, to ensure they deliver points properly. (UP version)
        /// </summary>
        [TestMethod]
        public void GetAllPointsUp()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.UP, 10);
            s.MoveHead();
            List<Point> points = new List<Point>();
            foreach (Point p in s.getAllPoints())
            {
                points.Add(p);
            }
            Assert.AreEqual(s.length, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                Assert.AreEqual(new Point(10, 20 - i), points[i]);
            }
        }

        /// <summary>
        /// A test of the GetAllPoints enumerator, to ensure they deliver points properly. (Down version)
        /// </summary>
        [TestMethod]
        public void GetAllPointsDown()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.DOWN, 10);
            s.MoveHead();
            List<Point> points = new List<Point>();
            foreach (Point p in s.getAllPoints())
            {
                points.Add(p);
            }
            Assert.AreEqual(s.length, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                Assert.AreEqual(new Point(10, 0 + i), points[i]);
            }
        }

        /// <summary>
        /// A test of the GetAllPoints enumerator, to ensure they deliver points properly. (Left version)
        /// </summary>
        [TestMethod]
        public void GetAllPointsLeft()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            s.MoveHead();
            List<Point> points = new List<Point>();
            foreach (Point p in s.getAllPoints())
            {
                points.Add(p);
            }
            Assert.AreEqual(s.length, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                Assert.AreEqual(new Point(20-i, 10), points[i]);
            }
        }

        /// <summary>
        /// A test of the GetAllPoints enumerator, to ensure they deliver points properly. (Right version)
        /// </summary>
        [TestMethod]
        public void GetAllPointsRight()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            s.MoveHead();
            List<Point> points = new List<Point>();
            foreach (Point p in s.getAllPoints())
            {
                points.Add(p);
            }
            Assert.AreEqual(s.length, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                Assert.AreEqual(new Point(i, 10), points[i]);
            }
        }

        /// <summary>
        /// Ensures that our snake returns the proper name and length, even after growing or shrinking
        /// </summary>
        [TestMethod]
        public void EnsureProperName()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            Assert.AreEqual("Mr.Snake: 10", s.ToString());
            s.MoveHead();
            Assert.AreEqual("Mr.Snake: 11", s.ToString());
            s.RetractTail();
            Assert.AreEqual("Mr.Snake: 10", s.ToString());
        }
        /// <summary>
        /// Tests to ensure that our snake collides when it's made vertically
        /// </summary>
        [TestMethod]
        public void TestVerticalCollision()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.UP, 10);
            Assert.IsTrue(s.Collides(new Point(10, 15)));
        }

        /// <summary>
        /// Tests to ensure that our snake collides when it's made horizontally
        /// </summary>
        [TestMethod]
        public void TestHorizontalCollision()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            Assert.IsTrue(s.Collides(new Point(15, 10)));
        }
        
        /// <summary>
        /// Tests to ensure that we can collide with ourselves, but only when we're actually colliding with ourselves.
        /// </summary>
        [TestMethod]
        public void TestSelfCollision()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.NextDirection = Snake.Direction.UP;
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.NextDirection = Snake.Direction.RIGHT;
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.NextDirection = Snake.Direction.DOWN;
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsFalse(s.IsCollidingWithSelf());
            s.MoveHead();
            Assert.IsTrue(s.IsCollidingWithSelf());

        }
        /// <summary>
        /// Ensures that we can retract the tail of the snake properly when the snake is vertical.
        /// </summary>
        [TestMethod]
        public void TestRetractTailVertical()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.UP, 10);
            s.RetractTail();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 19), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }


        /// <summary>
        /// Ensures that we can retract the tail of the snake properly when the snake is horizontal.
        /// </summary>
        [TestMethod]
        public void TestRetractTailHorizontal()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            s.RetractTail();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 10), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(1, 10), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }
        /// <summary>
        /// Ensures that when we kill the snake, the proper variables are set
        /// </summary>
        [TestMethod]
        public void TestKill()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.RIGHT, 10);
            s.Kill();
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(1, verts.Count);
            Assert.AreEqual(new Point(-1, -1), verts[0]);
        }
    }
}
