///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeModel;
using SnakeServer;
using System.Collections.Generic;

namespace SnakeModelTests
{
    [TestClass]
    public class WorldTests
    {
        GameServerSettings testSettings;
        [TestInitialize]
        public void Setup()
        {
            testSettings = new GameServerSettings(@"..\..\..\SnakeModelTests\TestGameSettings.xml");
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
        /// Ensures that the exception is thrown when we try to build a world that's too small for our defined snake length.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WorldWorldSettingsWorldTooSmall()
        {
            GameServerSettings brokenSettings = new GameServerSettings(@"..\..\..\SnakeModelTests\TestGameSettingsWorldTooSmall.xml");
            World w = new World(brokenSettings.SnakeWorldSettings);
        }
        /// <summary>
        /// Tests the constructor when Tron mode is turned on.
        /// </summary>
        [TestMethod]
        public void WorldWorldSettingsTronModeOn()
        {
            GameServerSettings tronModeOn = new GameServerSettings(@"..\..\..\SnakeModelTests\TestGameSettingsTronModeOn.xml");
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
            Assert.IsTrue(w.IsPlayerAlive(ID));
            Snake s = w.getSnakeByID(ID);
            Assert.AreEqual(ID, s.ID);
            Assert.AreEqual("Mr. Snake", s.name);
            Assert.AreEqual(15, s.length);
        }
        /// <summary>
        /// Creates a world and adds a bunch of snakes, then loops gameupdate for a bit, to make sure exceptions aren't thrown. (normal mode)
        /// </summary>
        [TestMethod]
        public void TestGameUpdateStressTestNormalMode()
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

        /// <summary>
        /// Creates a world and adds a bunch of snakes, then loops gameupdate for a bit, to make sure exceptions aren't thrown. (TRON mode)
        /// </summary>
        [TestMethod]
        public void TestGameUpdateStressTestTRONMode()
        {
            GameServerSettings tronModeOn = new GameServerSettings(@"..\..\..\SnakeModelTests\TestGameSettingsTronModeOn.xml");
            World w = new World(tronModeOn.SnakeWorldSettings);
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

        /// <summary>
        /// Ensure that we can properly change a snake's direction. Difficult to test, because of the server's random spawning mechanisms.
        /// </summary>
        [TestMethod]
        public void TestChangeSnakeDirection()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            int ID;
            Snake s;
            while (true)
            {
                ID = w.AllocateNewSnake("Mr. Snake");
                s = w.getSnakeByID(ID);
                if (s.CurrentDirection == Snake.Direction.UP)
                {
                    break; //Keep creating snakes until one is traveling north.
                }
                else
                {
                    //Manually kill the snakes that didn't work.
                    Snake clone = new Snake(ID, "Don'tCare", new Point(1, 1), Snake.Direction.UP, 10);
                    clone.Kill();
                    w.updateWorldSnakes(clone);//Ought to kill the snake we added. Kind of roundabout and nasty though.
                }
            }

            w.UpdateSnakeDirection(ID, (int) Snake.Direction.LEFT);
            w.GameUpdate();
            Assert.AreEqual(Snake.Direction.LEFT, w.getSnakeByID(ID).CurrentDirection);
            w.UpdateSnakeDirection(ID, (int)Snake.Direction.UP);
            w.GameUpdate();
            Assert.AreEqual(Snake.Direction.UP, w.getSnakeByID(ID).CurrentDirection);
            w.UpdateSnakeDirection(ID, (int)Snake.Direction.RIGHT);
            w.GameUpdate();
            Assert.AreEqual(Snake.Direction.RIGHT, w.getSnakeByID(ID).CurrentDirection);
            w.GameUpdate();
            w.UpdateSnakeDirection(ID, (int)Snake.Direction.DOWN);
            w.GameUpdate();
            Assert.AreEqual(Snake.Direction.DOWN, w.getSnakeByID(ID).CurrentDirection);


        }

        /// <summary>
        /// Tests the servers ToJson method by manually adding some snakes and food.
        /// </summary>
        [TestMethod]
        public void TestServerJSON()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            w.updateWorldSnakes(new Snake(10, "Yohan", new Point(50, 50), Snake.Direction.DOWN, 10));
            w.updateWorldFood(new Food(1, new Point(50, 100)));
            Assert.AreEqual("{\"vertices\":[{\"X\":50,\"Y\":40},{\"X\":50,\"Y\":50}],\"ID\":10,\"name\":\"Yohan\"}\n{\"ID\":1,\"loc\":{\"X\":50,\"Y\":100}}\n", w.ToJson());

        }
        /// <summary>
        /// Ensures that our GetLiveSnakesOrdered method returns snakes in order of score. 
        /// </summary>
        [TestMethod]
        public void TestGetLiveSnakesOrdered()
        {
            World w = new World(testSettings.SnakeWorldSettings);
            w.updateWorldSnakes(new Snake(10, "Yohan", new Point(50, 50), Snake.Direction.DOWN, 10));
            w.updateWorldSnakes(new Snake(11, "Jagger", new Point(51, 50), Snake.Direction.DOWN, 11));
            w.updateWorldSnakes(new Snake(12, "Fleetwood", new Point(52, 50), Snake.Direction.DOWN, 9));
            w.updateWorldSnakes(new Snake(13, "Tweedy", new Point(53, 50), Snake.Direction.DOWN, 12));
            w.updateWorldSnakes(new Snake(14, "Magnum", new Point(54, 50), Snake.Direction.DOWN, 20));
            List<Snake> snakes = new List<Snake>();
            foreach (Snake s in w.getLiveSnakesOrdered().Values)
            {
                snakes.Add(s);
            }
            Assert.AreEqual(14, snakes[0].ID);
            Assert.AreEqual(13, snakes[1].ID);
            Assert.AreEqual(11, snakes[2].ID);
            Assert.AreEqual(10, snakes[3].ID);
            Assert.AreEqual(12, snakes[4].ID);
        }
    }
}
