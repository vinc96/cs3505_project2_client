///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeModel;
using SnakeServer;

namespace SnakeModelTests
{
    [TestClass]
    public class WorldTests
    {
        GameServerSettings testSettings;
        [TestInitialize]
        public void Setup()
        {
            testSettings = new GameServerSettings(@"..\..\..\SnakeModelTests\TestGameSettings1.xml");
        }

        /// <summary>
        /// Tests that the default world constructor to ensure it sets all the default values.
        /// </summary>
        [TestMethod]
        public void WorldDefaultConstructor()
        {
            World w = new World(105, 200);
            Assert.AreEqual(105, w.Size.X);
            Assert.AreEqual(200, w.Size.Y);
            Assert.IsFalse(w.getLiveSnakes().GetEnumerator().MoveNext());
            Assert.IsFalse(w.getActiveFood().GetEnumerator().MoveNext());
            Assert.IsNull(w.getSnakeByID(0));
        }
        /// <summary>
        /// Tests the constructor that's initialized by some WorldSettings object. We'll use our test WorldSettings.
        /// </summary>
        [TestMethod]
        public void WorldWorldSettingsConstructor()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            Assert.AreEqual(testSettings.SnakeWorldSettings.BoardDimensions.X, w.Size.X);
            Assert.AreEqual(testSettings.SnakeWorldSettings.BoardDimensions.Y, w.Size.Y);
            Assert.IsFalse(w.getLiveSnakes().GetEnumerator().MoveNext());
            Assert.IsFalse(w.getActiveFood().GetEnumerator().MoveNext());
            Assert.IsNull(w.getSnakeByID(0));
            Assert.AreEqual("", w.ToJson());
            int ID = w.AllocateNewSnake("Don'tCare");
            Snake s = w.getSnakeByID(ID);
            Assert.AreEqual(testSettings.SnakeWorldSettings.SnakeStartingLength, s.length);
        }
        /// <summary>
        /// Tests the AllocateSnake method. The method should return an ID that referrs to a valid snake of the name specified.
        /// </summary>
        [TestMethod]
        public void TestAllocateSnake()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            int ID = w.AllocateNewSnake("Mr. Snake");
            Snake s = w.getSnakeByID(ID);
            Assert.AreEqual(ID, s.ID);
            Assert.AreEqual("Mr. Snake", s.name);
            Assert.AreEqual(15, s.length);
        }
        /// <summary>
        /// Creates a world and adds a bunch of snakes, then loops gameupdate for a bit, to make sure exceptions aren't thrown.
        /// </summary>

        [TestMethod]
        public void TestGameUpdateStressTest()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            w.AllocateNewSnake("Bla");
            w.AllocateNewSnake("Blaaa");//Allocate 2 snakes starting off
            for (int i = 0; i < 10; i++)
            {
                w.AllocateNewSnake("BlaBla"); //Allocate, update, allocate, update, etc.
                w.GameUpdate();
            }
            for (int i = 0; i < 1000; i++)
            {
                w.GameUpdate();
            }
        }
    }
}
