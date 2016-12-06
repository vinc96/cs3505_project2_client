using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static SnakeModel.World;

namespace SnakeModel
{

    public class WorldSettings
    {
        public WorldSettings(XmlDocument settings)
        {
            
        }
        public Dimensions BoardDimensions { get; private set; }

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
}
