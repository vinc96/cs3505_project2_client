///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static SnakeModel.World;

namespace SnakeModel
{
    /// <summary>
    /// An object designed to contain world settings, built from an XML node scraped from the settings.xml file. 
    /// </summary>
    public class WorldSettings
    {
        /// <summary>
        /// Creates a new WorldSettings object from the specified XML node. If the node doesn't contain proper values, 
        /// intialzes with defaults.
        /// </summary>
        /// <param name="worldSettings"></param>
        public WorldSettings(XmlNode worldSettings)
        {
            if (worldSettings == null)
            {
                worldSettings = new XmlDocument();
            }

            int boardWidth = getXmlNodeValue(worldSettings["BoardWidth"], 150);
            int boardHeight = getXmlNodeValue(worldSettings["BoardHeight"], 150);

            BoardDimensions = new Dimensions(boardWidth, boardHeight);

            FoodDensity = getXmlNodeValue(worldSettings["FoodDensity"], 10);
            SnakeRecycleRate = getXmlNodeValue<double>(worldSettings["SnakeRecycleRate"], 0.5);
            SnakeStartingLength = getXmlNodeValue<int>(worldSettings["SnakeStartingLength"], 15);
            TronModeOn = (getXmlNodeValue(worldSettings["TronMode"], 0)==1);

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
        /// The starting length of snakes on the server.
        /// </summary>
        public int SnakeStartingLength { get; private set; }
        /// <summary>
        /// Indicates whether or not "TRON" mode is on for the server. That is to say, indicates whether or not the server spawns food,
        /// or if snakes just constantly grow, and you have to avoid your tail.
        /// </summary>
        public bool TronModeOn { get; private set;}

        /// <summary>
        /// What the game needs to do on a per-GameUpdate basis. Should be set to a method that moves snakes, updates food, and takes 
        /// care of other gamerule specific on GameUpdate behavior. Functionally defines the rules of the snake game.
        /// </summary>
        public Action OnGameUpdate { get; internal set; }
        /// <summary>
        /// A helper function to pull data out of an XML node. If the data exists, returns that data. If not, returns some default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T getXmlNodeValue<T>(XmlNode xmlNode, T defaultValue)
        {
            if (xmlNode == null)
            {
                return defaultValue;
            }

            T value = defaultValue;

            TypeConverter tConverter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                value = (T)tConverter.ConvertFromString(xmlNode.InnerText);
            }
            catch(Exception e)
            {
                Console.WriteLine("\nParsing Error\n"+e.ToString());
            }

            return value;
        }
    }
}
