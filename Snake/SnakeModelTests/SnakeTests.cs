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
        /// Ensures the snake constructor builds a snake with the values we've stated for it.
        /// </summary>
        [TestMethod]
        public void TestSnakeConstructor()
        {
            Snake s = new Snake(10, "Mr.Snake", new Point(10, 10), Snake.Direction.LEFT, 10);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("Mr.Snake", s.name);
            Assert.AreEqual(new Point(10, 10), s.getHead());
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
            List<Point> verts = new List<Point>();
            foreach (Point p in s.getVerticies())
            {
                verts.Add(p);
            }
            Assert.AreEqual(new Point(10, 11), verts[verts.Count - 1]);
            Assert.AreEqual(new Point(10, 0), verts[0]);
            Assert.AreEqual(2, verts.Count);
        }

    
    }
}
