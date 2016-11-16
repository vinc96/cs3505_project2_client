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
        /// A list of all active snakes. 
        /// </summary>
        private List<Snake> snakes;
        /// <summary>
        /// A list of all the food we have in the world. 
        /// </summary>
        private List<Food> food;

        /// <summary>
        /// Creates a new World object with the specified live snakes, food, and dimensions
        /// </summary>
        /// <param name="snakes"></param>
        /// <param name="food"></param>
        public World(List<Snake> snakes, List<Food> food, int width, int height)
        {
            this.snakes = snakes;
            this.food = food;
        }

        /// <summary>
        /// Returns true if the specified playerID corresponds to a live snake, false otherwise.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool IsPlayerAlive(int playerID)
        {
            foreach (Snake s in snakes)
            {
                //If we find a snake with a matching ID, return true.
                if (s.getID() == playerID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the global coordinates for the head of the snake specified by the playerID. 
        /// If the playerID doesn't correspond to a snake currently alive in the world, returns null.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Point getHead(int playerID)
        {
            foreach (Snake s in snakes)
            {
                if (s.getID() == playerID)
                {
                    return s.getHead();
                }
            }
            throw new NotImplementedException();
        }

        public IEnumerable<Snake> getLiveSnakes()
        {
            //Protects the internals of this class, but just awful complexity (esp. considering how often it's used.) High priority to fix.
            return new List<Snake>(snakes);
        }
    }
}
