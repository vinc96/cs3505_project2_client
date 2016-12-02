///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnakeModel
{
    /// <summary>
    /// The class through which the SnakeModel interacts with the outside world. 
    /// Contains a representation of all the food, snakes, world size, etc. Contains
    /// methods to retrieve the current location of every entity in the game, as well 
    /// as to retrieve metadata for these entities (id's, names, etc. for snakes, etc.)
    /// </summary>
    public class World
    {
        /// <summary>
        /// A dictionary of all active snakes with the key being the id of each snake.
        /// </summary>
        private Dictionary<int, Snake> snakes;

        /// <summary>
        /// A list of all snakes that are in the process of dying. Should be populated and cleared inside of an OnTick call.
        /// </summary>
        private List<Snake> dyingSnakes;
        /// <summary>
        /// A list of all snakes that are in the process of growing. Should be populated and clearead inside of an OnTick call.
        /// </summary>
        private List<Snake> digestingSnakes;
        /// <summary>
        /// A dictionary of all the food we have in the world with the key being the id of each piece food.
        /// </summary>
        private Dictionary<int, Food> food;
        /// <summary>
        /// An struct that keeps track of world dimensions. Immutable.
        /// </summary>
        public struct Dimensions
        {
            public Dimensions(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
        }

        private Dimensions Size;

        /// <summary>
        /// A WorldSettings object. Contains the settings for this game world,
        /// parsed from a settings.xml file with the following layout:
        /// <SnakeSettings>
        ///     <BoardWidth>Width of the board, in cells</BoardWidth>
        ///     <BoardHeight>Height of the board, in cells</BoardHeight>
        ///     <MSPerFrame>Milliseconds per Frame</MSPerFrame>
        ///     <FoodDensity>Food per Snake</FoodDensity>
        ///     <SnakeRecycleRate>Snake recycle rate</SnakeRecycleRate>
        /// </SnakeSettings>
        /// </summary>
        public struct WorldSettings
        {
            /// <summary>
            /// Creates a new WorldSettings object read from the specified filepath, 
            /// throws a relevant IOException if there's a problem finding/reading the file.
            /// </summary>
            /// <param name="filePath"></param>
            public WorldSettings(string filePath)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                using (XmlReader reader = new XmlTextReader(filePath))
                {
                    //Read stuff, man. Parse it to the delegates of this struct when you've read it.


                }

                //REMOVE ONCE IMPLEMENTED. STUCK HERE TO PREVENT A COMPILER ERROR.
                BoardDimensions = new Dimensions(0, 0);
                MSPerFrame = 0;
                FoodDensity = 0;
                SnakeRecycleRate = 0;
                //Our default gamemode updates nothing.
                OnTick = () => { };
            }

            public Dimensions BoardDimensions { get; private set; }
            /// <summary>
            /// The period at which we run game ticks. 
            /// </summary>
            public int MSPerFrame { get; private set; }
            /// <summary>
            /// The number of food items that spawn per snake connected to the server.
            /// </summary>
            public int FoodDensity { get; private set; }
            /// <summary>
            /// The percentage rate at which snakes are recycled into food. A recycle rate of 1 means that the entire snake becomes
            /// food, a recycle rate of zero means that none of the snake turns into food. 
            /// </summary>
            public double SnakeRecycleRate { get; private set; }

            /// <summary>
            /// What the game needs to do on a per-tick basis. Should be set to a method that moves snakes, updates food, and takes 
            /// care of other gamerule specific on tick behavior. Functionally defines the rules of the snake game.
            /// </summary>
            public Action OnTick { get; internal set; }
        }



        /// <summary>
        /// The worldSettings for this world. Null for client worlds, as they never have to do any world simulations. 
        /// </summary>
        private WorldSettings worldSettings;

        /// <summary>
        /// Creates a new World object of the given dimensions
        /// </summary>
        /// <param name="snakes"></param>
        /// <param name="food"></param>
        public World(int width, int height)
        {
            // Snakes And Food Should Be Empty When The World Is Made
            snakes = new Dictionary<int, Snake>();
            food = new Dictionary<int, Food>();
            Size = new Dimensions(width, height);
        }
        /// <summary>
        /// Creates a world that's suitable for simulating game behavior.
        /// </summary>
        /// <param name=""></param>
        public World(WorldSettings worldSettings) : this(worldSettings.BoardDimensions.X, worldSettings.BoardDimensions.Y)
        {
            this.worldSettings = worldSettings;
            if (false) //This will eventually be "If some worldSetting is enabled, then we use our alternate OnTick method"
            {
                //Use our alternate OnTick method.
            }
            else
            {
                this.worldSettings.OnTick = DefaultGameRulesOnTick;
            }
        }

        /// <summary>
        /// Returns true if the specified playerID corresponds to a live snake, false otherwise.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool IsPlayerAlive(int playerID)
        {
            return snakes.ContainsKey(playerID);
        }

        /// <summary>
        ///  Updates The World's Snakes by Adding, Updating Or Removing The Snake From The Game World
        /// </summary>
        /// <param name="s">The Snake To Add, Update Or Remove From The Game World</param>
        public void updateWorldSnakes(Snake s)
        {
            snakes[s.ID] = s;

            Point snakeHead = s.getHead();
            bool snakeIsDead = (snakeHead.x == -1) && (snakeHead.y == -1);
            if (snakeIsDead)
            {
                snakes.Remove(s.ID);
            }
        }

        /// <summary>
        ///  Updates The Worlds Active Food by Adding, Updating Or Removing The Food From The Game World
        /// </summary>
        /// <param name="f">The Food To Add, Update Or Remove From The Game World</param>
        public void updateWorldFood(Food f)
        {
            food[f.ID] = f;

            Point foodLocation = f.loc;
            bool foodIsEaten = (foodLocation.x == -1) && (foodLocation.y == -1);
            if (foodIsEaten)
            {
                food.Remove(f.ID);
            }
        }
        /// <summary>
        /// Returns all currently live snakes, with no guarantee as to the order. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Snake> getLiveSnakes()
        {
            return snakes.Values;
        }
        /// <summary>
        /// Return the snakes, ordered by score from highest to lowest. 
        /// </summary>
        /// <returns></returns>
        public SortedList<int, Snake> getLiveSnakesOrdered()
        {
            //Create a new sorted list using snake length as a key (there will be duplicate keys, but we'll
            //always be enumerating over the Values anyway), defining the comparator to rank higher scores earler.
            SortedList<int, Snake> sortedSnakes = new SortedList<int, Snake>(Comparer<int>.Create((i, j) => { if (i == j) { return 1; } else { return j - i; } }));
            foreach (Snake s in getLiveSnakes())
            {
                sortedSnakes.Add(s.length, s); //Poor complexity I know, but doesn't seem to adversely impact performance too badly (O(# of Snakes))
            }
            return sortedSnakes;

        }
        /// <summary>
        /// Returns the active food (food that has not yet been eaten)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Food> getActiveFood()
        {
            return food.Values;
        }

        /// <summary>
        /// Returns the snake specified by the playerID. If the playerID doesn't correspond to a 
        /// snake currently alive in the world, returns null.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Snake getSnakeByID(int playerID)
        {
            if (snakes.Keys.Contains(playerID))
            {
                return snakes[playerID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Increments the world by one unit of time. If this object wasn't constructed with a WorldSettings struct, throws InvalidOperationException
        /// </summary>
        public void tick()
        {
            if (ReferenceEquals(worldSettings, null))
            {
                throw new InvalidOperationException("You must construct a world with a WorldSettings struct to simulate the world.");
            }
            else
            {
                lock (this) //Since OnTick ought to be changing the world, we should lock it up.
                {
                    //Execute gamerule stuff we see fit to, based upon the defined rules of our server. 
                    worldSettings.OnTick();
                }
            }
        }
        /// <summary>
        /// The default game mode for the server. Corresponds to the actions taken to facilitate normal snake behavior. 
        /// Snakes move forward one cell per tick, food extends snakes by 1 cell, if a snake collides with a wall or another snake it dies.
        /// </summary>
        private void DefaultGameRulesOnTick()
        {
            foreach (Snake s in snakes.Values)
            {
                s.MoveHead(1);
            }
            CheckCollisions();

        }

        /// <summary>
        ///Checks to see if a snake collides with either food or another snake. Takes the proper actions depending on what we colided with. 
        ///If a snake collided with a food, it's added to the DigestingSnakes list. If a snake is due to die, then it's added to the dyingSnakes list.
        /// </summary>
        private void CheckCollisions()
        {
            foreach (Snake snake in snakes.Values)
            {
                //Check to see if we're colliding with any snakes
                foreach (Snake otherSnake in snakes.Values)
                {
                    if (snake.Equals(otherSnake)) //We can't collide with ourselves
                    {
                        continue;
                    }
                    else if (otherSnake.getHead().Equals(snake.getHead()))
                    {
                        dyingSnakes.Add(otherSnake); //If the snakes collide, kill both and break
                        dyingSnakes.Add(snake);
                        break;
                    }
                    else if (snake.Collides(otherSnake))
                    {
                        dyingSnakes.Add(snake); //If this snake is colliding with the other snake, this snake dies. Then break.
                        break;
                    }

                }
                //Check to see if we're colliding with any food
                foreach (Food f in food.Values)
                {
                    if (snake.getHead().Equals(f.loc))
                    {
                        digestingSnakes.Add(snake);
                    }
                }
            }
        }
    }
}
