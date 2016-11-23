///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// A dictionary of all the food we have in the world with the key being the id of each piece food.
        /// </summary>
        private Dictionary<int, Food> food;
        /// <summary>
        /// An struct that keeps track of world dimensions. Immutable.
        /// </summary>
        public struct Dimensions
        {
            public Dimensions(int x,  int y) {
                X = x;
                Y = y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
        }

        public Dimensions Size;


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

            Point foodLocation= f.loc;
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
            SortedList<int, Snake> sortedSnakes =  new SortedList<int, Snake>(Comparer<int>.Create((i,j) => { if (i == j) { return 1; } else { return j - i; } }));
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
    }
}
