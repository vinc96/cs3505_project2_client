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
        private Dictionary<int, Snake> liveSnakes;

        /// <summary>
        /// A set of all snakes that have died since the last ToJson Call.
        /// </summary>
        private HashSet<Snake> deadSnakes;
        /// <summary>
        /// A dictionary of all the food we have in the world with the key being the id of each piece food.
        /// </summary>
        private Dictionary<int, Food> food;
        /// <summary>
        /// A set of food that's been eaten since the last ToJson Call. 
        /// </summary>
        private HashSet<Food> eatenFood;
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

        public Dimensions Size { get; private set; }

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
                OnGameUpdate = () => { };
            }

            public Dimensions BoardDimensions { get; private set; }
            /// <summary>
            /// The period at which we run game GameUpdates. 
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
            /// What the game needs to do on a per-GameUpdate basis. Should be set to a method that moves snakes, updates food, and takes 
            /// care of other gamerule specific on GameUpdate behavior. Functionally defines the rules of the snake game.
            /// </summary>
            public Action OnGameUpdate { get; internal set; }
        }



        /// <summary>
        /// The worldSettings for this world. Null for client worlds, as they never have to do any world simulations. 
        /// </summary>
        private WorldSettings worldSettings;

        /// <summary>
        /// The Random object for this world. Used to determine where snakes are placed, as well as food distribution.
        /// </summary>
        private Random random;
        /// <summary>
        /// An instance variable used to determine the next distributed ID for snakes or food.
        /// </summary>
        private int nextID;

        /// <summary>
        /// Creates a new World object of the given dimensions
        /// </summary>
        /// <param name="snakes"></param>
        /// <param name="food"></param>
        public World(int width, int height)
        {
            // Snakes And Food Should Be Empty When The World Is Made
            liveSnakes = new Dictionary<int, Snake>();
            food = new Dictionary<int, Food>();
            Size = new Dimensions(width, height);
            random = new Random();
        }
        /// <summary>
        /// Creates a world that's suitable for simulating game behavior.
        /// </summary>
        /// <param name=""></param>
        public World(WorldSettings worldSettings) : this(worldSettings.BoardDimensions.X, worldSettings.BoardDimensions.Y)
        {
            this.worldSettings = worldSettings;
            if (false) //This will eventually be "If some worldSetting is enabled, then we use our alternate OnGameUpdate method"
            {
                //Use our alternate OnGameUpdate method.
            }
            else
            {
                this.worldSettings.OnGameUpdate = DefaultGameRulesOnGameUpdate;
            }
        }

        /// <summary>
        /// Returns true if the specified playerID corresponds to a live snake, false otherwise.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool IsPlayerAlive(int playerID)
        {
            return liveSnakes.ContainsKey(playerID);
        }

        /// <summary>
        ///  Updates The World's Snakes by Adding, Updating Or Removing The Snake From The Game World
        ///  Used by the client only.
        /// </summary>
        /// <param name="s">The Snake To Add, Update Or Remove From The Game World</param>
        public void updateWorldSnakes(Snake s)
        {
            liveSnakes[s.ID] = s;

            Point snakeHead = s.getHead();
            bool snakeIsDead = (snakeHead.x == -1) && (snakeHead.y == -1);
            if (snakeIsDead)
            {
                liveSnakes.Remove(s.ID);
            }
        }

        /// <summary>
        ///  Updates The Worlds Active Food by Adding, Updating Or Removing The Food From The Game World.
        ///  Used by the client only.
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
            return liveSnakes.Values;
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
            if (liveSnakes.Keys.Contains(playerID))
            {
                return liveSnakes[playerID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Increments the world by one unit of time. If this object wasn't constructed with a WorldSettings struct, throws InvalidOperationException.
        /// </summary>
        public void GameUpdate()
        {
            if (ReferenceEquals(worldSettings, null))
            {
                throw new InvalidOperationException("You must construct a world with a WorldSettings struct to simulate the world.");
            }
            else
            {
                lock (this) //Since OnGameUpdate ought to be changing the world, we should lock it up.
                {
                    //Execute gamerule stuff we see fit to, based upon the defined rules of our server. 
                    worldSettings.OnGameUpdate();
                }
            }
        }
        /// <summary>
        /// The default game mode for the server. Corresponds to the actions taken to facilitate normal snake behavior. 
        /// Snakes move forward one cell per GameUpdate, food extends snakes by 1 cell, if a snake collides with a wall or another snake it dies.
        /// </summary>
        private void DefaultGameRulesOnGameUpdate()
        {
            //The sets that contain the snakes that die and grow in this frame.
            HashSet<Snake> dyingSnakes = new HashSet<Snake>();
            HashSet<Snake> digestingSnakes = new HashSet<Snake>();
            foreach (Snake s in liveSnakes.Values)
            {
                s.MoveHead();//Move the snake's head head one unit in the direction the snake is heading.
            }
            CheckCollisions(dyingSnakes, digestingSnakes);
            HandleDeath(dyingSnakes); //Death and eating should be mutually exclusive. 
            foreach (Snake s in liveSnakes.Values)
            {
                if (!digestingSnakes.Contains(s))
                {
                    s.RetractTail(); //Retract the tails of any snake that didn't eat.
                }
            }


        }



        /// <summary>
        ///Checks to see if a snake collides with either food or another snake. Takes the proper actions depending on what we colided with. 
        ///If a snake collided with a food, it's added to the digestingSnakes set. If a snake is due to die, then it's added to the dyingSnakes set.
        /// </summary>
        private void CheckCollisions(ISet<Snake> dyingSnakes, ISet<Snake> digestingSnakes)
        {
            foreach (Snake snake in liveSnakes.Values)
            {
                //Check to see if we're colliding with any snakes
                foreach (Snake otherSnake in liveSnakes.Values)
                {
                    if (snake.Equals(otherSnake)) //We can't collide with ourselves
                    {
                        continue;
                    }
                    else if (otherSnake.getHead().Equals(snake.getHead()))
                    {
                        dyingSnakes.Add(otherSnake); //If the snakes collide, kill both and continue
                        dyingSnakes.Add(snake);
                        continue;
                    }
                    else if (snake.Collides(otherSnake))
                    {
                        dyingSnakes.Add(snake); //If this snake is colliding with the other snake, this snake dies. Then continue.
                        continue;
                    }

                }
                //Check to see if we're colliding with any food
                foreach (Food f in food.Values)
                {
                    if (snake.getHead().Equals(f.loc))
                    {
                        //Make the snake digest, put the food in the eaten set, and remove it from the active food set.
                        digestingSnakes.Add(snake);
                        eatenFood.Add(f);
                        food.Remove(f.ID);
                    }
                }
            }
        }

        /// <summary>
        /// If a snake is in the dyingSnakes set, turn it into food and remove it from the liveSnakes set.
        /// </summary>
        private void HandleDeath(ISet<Snake> dyingSnakes)
        {
            
            foreach (Snake dyingSnake in dyingSnakes)
            {
                //Place the food randomly where the snake is.
                int foodAmount = (int) (dyingSnake.length * worldSettings.SnakeRecycleRate);
                int[] foodOffsets = new int[foodAmount];
                //Generate the offsets from the tail at which we're placing food
                for(int i = 0; i < foodOffsets.Length; i++)
                {
                    int foodOffset;

                    while (true)
                    {
                        foodOffset = random.Next(dyingSnake.length);
                        if (foodOffsets.Contains(foodOffset))
                        {
                            continue; //If we're already planning to place food here, reroll the number.
                        }
                        else
                        {
                            //Otherwise, add this to the list of offsets, and break.
                            foodOffsets[i] = foodOffset;
                            break;
                        }
                    }
                    
                }
                //Go through the snake's verticies, placing food.

                Point[] snakePoints = dyingSnake.getAllPoints().ToArray();
                for (int i = 0; i < snakePoints.Length; i++)
                {
                    if (foodOffsets.Contains(i))
                    {
                        Food f = new Food(GetNextFoodID(), snakePoints[i]);
                        food.Add(f.ID, f);
                    }
                }

                //Remove the snake from the liveSnakes set.
                liveSnakes.Remove(dyingSnake.ID);
            }
        }

        /// <summary>
        /// Returns an unsude ID for use in assigning IDs to food.
        /// </summary>
        /// <returns></returns>
        private int GetNextFoodID()
        {
            //Food and Snake IDs draw from same pool of IDs. Not by spec, but doesn't contradict current spec either.
            nextID++;
            return nextID;
        }

        /// <summary>
        /// Returns an unused ID for use in assigning IDs to snakes. 
        /// </summary>
        /// <returns></returns>
        private int GetNextSnakeID()
        {
            //Food and Snake IDs draw from same pool of IDs. Not by spec, but doesn't contradict current spec either.
            nextID++;
            return nextID; 
        }

        public string ToJson()
        {
            // Fill In Json Serialize Code, remember to clear deadSnakes and eatenFood after you record them.
            return "{}";
        }
    }
}
