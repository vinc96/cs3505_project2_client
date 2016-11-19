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
        /// Creates a new World object with the specified live snakes, food, and dimensions
        /// </summary>
        /// <param name="snakes"></param>
        /// <param name="food"></param>
        public World(Dictionary<int, Snake> snakes, Dictionary<int, Food> food, int width, int height)
        {
            this.snakes = snakes;
            this.food = food;

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
        /// Returns the global coordinates for the head of the snake specified by the playerID. 
        /// If the playerID doesn't correspond to a snake currently alive in the world, returns null.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Point getHead(int playerID)
        {

            if (snakes.ContainsKey(playerID))
            {
                return snakes[playerID].getHead();
            }

            return null;
        }

        public IEnumerable<Snake> getLiveSnakes()
        {
            return snakes.Values;
        }

        public IEnumerable<Food> getActiveFood()
        {
            return food.Values;
        }
    }
}
